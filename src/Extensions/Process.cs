using System;
using System.Security.Principal;
using HXE.Common;
using Microsoft.Win32.SafeHandles;
using PInvoke;
using Windows.Win32.Foundation;
using Windows.Win32.Security;
using static Windows.Win32.PInvoke;
using static Windows.Win32.Security.TOKEN_PRIVILEGES_ATTRIBUTES;

namespace HXE.Extensions
{
    public static class ExtProcess
    {
        /// <summary>
        /// See <see href="https://docs.microsoft.com/en-us/windows/win32/secauthz/well-known-sids">Well-known SIDs</see>.
        /// </summary>
        internal const int DOMAIN_GROUP_RID_ADMINS = 0x200;
        public static bool IsCurrentProcessElevated()
        {
            // WindowsPrincipal.IsInRole() secretly checks process security tokens

            WindowsIdentity identity = WindowsIdentity.GetCurrent(ifImpersonating: false);
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator)
                || principal.IsInRole(DOMAIN_GROUP_RID_ADMINS);
        }
        public static bool IsElevated(this System.Diagnostics.Process process)
        {
            if (process == null)
                throw new ArgumentNullException(nameof(process));

            if (process.SafeHandle != null && process.Id == 4)
            {
                // TODO: better exception Type
                throw new AccessViolationException("System (PID 4) token can't be opened");
            }
            else
            {
                if (!(bool)OpenProcessToken(
                    ProcessHandle: process.SafeHandle,
                    DesiredAccess: TOKEN_ACCESS_MASK.TOKEN_QUERY,
                    TokenHandle: out SafeFileHandle tokenHandle))
                {
                    throw new InfoWin32Exception(Kernel32.GetLastError());
                }

                TOKEN_ELEVATION_TYPE elevationType;

                using (tokenHandle)
                using (Kernel32.SafeObjectHandle objectHandle = new Kernel32.SafeObjectHandle(tokenHandle.DangerousGetHandle()))
                {
                    elevationType = (TOKEN_ELEVATION_TYPE)AdvApi32.GetTokenElevationType(objectHandle);
                }

                return elevationType == TOKEN_ELEVATION_TYPE.TokenElevationTypeFull;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        /// TODO: NOT TESTED!
        public static bool HasSeBackupPrivilege(this System.Diagnostics.Process process)
        {
            ///
            /// Initialize new TOKEN_PRIVILEGES instance for SE_BACKUP_NAME
            ///

            TOKEN_PRIVILEGES tokenPrivilege = new TOKEN_PRIVILEGES
            {
                PrivilegeCount = 1
            };
            tokenPrivilege.Privileges._0.Luid.LowPart = 0u;
            tokenPrivilege.Privileges._0.Luid.HighPart = 0;

            ///
            /// Get Process Token
            ///
            SafeFileHandle processToken = GetProcessToken(process, TokenAccessMask.Query);

            ///
            /// Get Locally Unique Idenifier (LUID) of SE_BACKUP_NAME
            ///

            if (!LookupPrivilegeValue(
                lpSystemName: null,
                lpName: SE_BACKUP_NAME,
                lpLuid: out tokenPrivilege.Privileges._0.Luid))
            {
                Win32ErrorCode error = Kernel32.GetLastError();
                throw new Win32Exception(error, error.GetMessage());
            }

            PRIVILEGE_SET requiredPrivileges = new PRIVILEGE_SET
            {
                PrivilegeCount = 1
            };
            requiredPrivileges.Privilege._0.Luid.LowPart = 0u;
            requiredPrivileges.Privilege._0.Luid.HighPart = 0;

            ///
            /// Check for process token for SE_BACKUP_NAME
            ///

            if (!PrivilegeCheck(
                processToken,
                ref requiredPrivileges,
                out int pfResult
                ))
            {
                throw new InfoWin32Exception(Kernel32.GetLastError());
            }
            return pfResult != 0;
        }



        [Flags]
        public enum TokenAccessMask : uint
        {
            None = 0,
            AssignPrimary = TokenAccessLevels.AssignPrimary,
            Duplicate = TokenAccessLevels.Duplicate,
            Impersonate = TokenAccessLevels.Impersonate,
            Query = TokenAccessLevels.Query,
            QuerySource = TokenAccessLevels.QuerySource,
            AdjustPrivileges = TokenAccessLevels.AdjustPrivileges,
            AdjustGroups = TokenAccessLevels.AdjustGroups,
            AdjustDefault = TokenAccessLevels.AdjustDefault,
            AdjustSessionId = TokenAccessLevels.AdjustSessionId,
            Delete = TOKEN_ACCESS_MASK.TOKEN_DELETE,
            ReadControl = TOKEN_ACCESS_MASK.TOKEN_READ_CONTROL, // STANDARD_RIGHTS_READ
            Read = Query | ReadControl,
            Write = ReadControl | AdjustPrivileges | AdjustGroups | AdjustDefault,
            WriteDac = TOKEN_ACCESS_MASK.TOKEN_WRITE_DAC,
            WriteOwner = TOKEN_ACCESS_MASK.TOKEN_WRITE_OWNER,
            // STANDARD_RIGHTS_REQUIRED = 0xF0000 // 983040U
            AllAccess = TokenAccessLevels.AllAccess,
            AccessSystemSecurity = TOKEN_ACCESS_MASK.TOKEN_ACCESS_SYSTEM_SECURITY,
            MaximumAllowed = TokenAccessLevels.MaximumAllowed
        }

        /// <summary>
        ///     Get a Win32 Process security token
        /// </summary>
        /// <param name="process">The process to get a token from.</param>
        /// <returns>A SafeFileHandle representing the process's security token with the given access privileges.</returns>
        public static SafeFileHandle GetProcessToken(this System.Diagnostics.Process process, TokenAccessMask access)
        {
            if (!OpenProcessToken(process.SafeHandle,
                (TOKEN_ACCESS_MASK)access,
                out SafeFileHandle processToken))
            {
                //TODO: improve exception handling. Create/Use new exception types
                throw new InfoWin32Exception(Kernel32.GetLastError());
            }

            return processToken;
        }

        /// AdjustTokenPrivileges(
        ///     System.Runtime.InteropServices.SafeHandle TokenHandle,
        ///     BOOL DisableAllPrivileges,
        ///     TOKEN_PRIVILEGES? NewState,
        ///     uint BufferLength,
        ///     TOKEN_PRIVILEGES* PreviousState,
        ///     uint* ReturnLength)
        //public static void AdjustTokenPrivileges(this SafeFileHandle processToken){}

        /// <summary>
        ///
        /// </summary>
        /// <param name="process"></param>
        /// <param name="enable"></param>
        /// TODO: Works, but I don't know how it works. If the privilege isn't already present, how does one ADD it?
        public static void SetSeBackupPrivilege(this System.Diagnostics.Process process, bool enable = true)
        {
            SafeFileHandle processToken = process.GetProcessToken(TokenAccessMask.AdjustPrivileges | TokenQuery);

            if (!LookupPrivilegeValue(null, SE_BACKUP_NAME, out LUID luidPrivilege))
            {
                throw new InfoWin32Exception(Kernel32.GetLastError());
            }

            TOKEN_PRIVILEGES privileges;
            privileges.PrivilegeCount = 1;
            privileges.Privileges._0.Luid = luidPrivilege;
            privileges.Privileges._0.Attributes = SE_PRIVILEGE_ENABLED;

            unsafe
            {
                if (!AdjustTokenPrivileges(
                    processToken,
                    false,
                    privileges,
                    0,
                    null,
                    null
                    ))
                {
                    throw new InfoWin32Exception(Kernel32.GetLastError());
                }
            }
        }
        /* Get TokenInformation as defined Type
        internal static TokenInformation GetTokenInformation(this System.Diagnostics.Process process)
        {
                        if (process == null)
                throw new ArgumentNullException(nameof(process));

            if (process.SafeHandle != null && process.Id == 4)
            {
                // TODO: better exception Type
                throw new AccessViolationException("System (PID 4) token can't be opened");
            }
            else
            {
                bool optSuccess = AdvApi32.OpenProcessToken(
                    processHandle: process.SafeHandle.DangerousGetHandle(),
                    desiredAccess: AdvApi32.TokenAccessRights.TOKEN_QUERY | AdvApi32.TokenAccessRights.TOKEN_DUPLICATE,
                    tokenHandle: out Kernel32.SafeObjectHandle tokenHandle);
                if (!optSuccess)
                {
                    Win32ErrorCode error = Kernel32.GetLastError();
                    throw new PInvoke.Win32Exception(error, message: error.GetMessage());
                }
                AdvApi32.GetTokenInformation(
                    TokenHandle: tokenHandle,
                    TokenInformationClass: AdvApi32.TOKEN_INFORMATION_CLASS.,
                    TokenInformation: null,
                    TokenInformationLength: null,
                    ReturnLength: out int length
                );
            }
        }*/
    }
}
