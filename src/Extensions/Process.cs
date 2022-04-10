using System;
using System.Security.Principal;
using HXE.Common;
using Microsoft.Win32.SafeHandles;
using PInvoke;
using Windows.Win32.Foundation;
using Windows.Win32.Security;
using static Windows.Win32.PInvoke;
using static Windows.Win32.Security.TOKEN_ACCESS_MASK;
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
                    DesiredAccess: TOKEN_QUERY,
                    TokenHandle: out SafeFileHandle tokenHandle))
                {
                    FileSystemCompression.ThrowWin32Exception(Kernel32.GetLastError());
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

            if (!OpenProcessToken(
                process.SafeHandle,
                TOKEN_QUERY,
                out SafeFileHandle processToken
            ))
            {
                Win32ErrorCode error = Kernel32.GetLastError();
                throw new Win32Exception(error, error.GetMessage());
            }

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
                FileSystemCompression.ThrowWin32Exception(Kernel32.GetLastError());
            }
            return pfResult != 0;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="process"></param>
        /// <param name="enable"></param>
        /// TODO: Works, but I don't know how it works. If the privilege isn't already present, how does one ADD it?
        public static void SetSeBackupPrivilege(this System.Diagnostics.Process process, bool enable = true)
        {

            if (!OpenProcessToken(process.SafeHandle,
                TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY,
                out SafeFileHandle accessTokenHandle))
            {
                FileSystemCompression.ThrowWin32Exception(Kernel32.GetLastError());
            }

            if (!LookupPrivilegeValue(null, SE_BACKUP_NAME, out LUID luidPrivilege))
            {
                FileSystemCompression.ThrowWin32Exception(Kernel32.GetLastError());
            }

            TOKEN_PRIVILEGES privileges;
            privileges.PrivilegeCount = 1;
            privileges.Privileges._0.Luid = luidPrivilege;
            privileges.Privileges._0.Attributes = SE_PRIVILEGE_ENABLED;

            unsafe
            {
                if (!AdjustTokenPrivileges(
                    accessTokenHandle,
                    false,
                    privileges,
                    0,
                    null,
                    null
                    ))
                {
                    FileSystemCompression.ThrowWin32Exception(Kernel32.GetLastError());
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
