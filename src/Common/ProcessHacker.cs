using System;
using System.Runtime.InteropServices;
using PInvoke;

namespace HXE.Common
{
    internal class ProcessHacker
    {
        internal enum KNOWN_WINDOWS_PROCESS_ID
        {
            /// <summary>The PID of the idle process.</summary>
            SYSTEM_IDLE_PROCESS_ID = 0,
            /// <summary>The PID of the system process.</summary>
            SYSTEM_PROCESS_ID = 4
        }

        internal static bool NT_SUCCESS(NTSTATUS.Code status) => status >= 0;

        internal unsafe struct _PH_TOKEN_ATTRIBUTES
        {
            SafeHandle TokenHandle;
            public struct enums
            {
                public const ulong Elevated = 1;
                public const ulong ElevationType = 2;
                public const ulong ReservedBits = 29;
            };
            ulong Reserved;
            //PSID TokenSid;
        }

        internal struct PH_TOKEN_ATTRIBUTES { }

        internal struct PPH_TOKEN_ATTRIBUTES { }

        // TODO: GetTokenInformation
        internal static TokenInformation GetTokenInformation(System.Diagnostics.Process process)
        {
            if (process == null)
                throw new ArgumentNullException(nameof(process));

            return new TokenInformation();
        }

        // TODO: struct TokenInformation
        [StructLayout(LayoutKind.Sequential)]
        internal struct TokenInformation
        {
            public PTOKEN_USER tokenUser;
            public TOKEN_ELEVATION_TYPE elevationType;
            public MANDATORY_LEVEL integrityLevel;
            public string integrityString;

            // User
            //if (NT_SUCCESS(PhOpenProcessToken(System.Diagnostics.Process.SafeHandle))){}

        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct PH_Process
        {

        }

        // TODO: struct PH_HASH_ENTRY
        [StructLayout(LayoutKind.Sequential)]
        internal struct PH_HashEntry { }

        [StructLayout(LayoutKind.Sequential)]
        internal struct PPH_ProcessRecord
        {
            LIST_ENTRY ListEntry;
            long RefCount;
            ulong Flags;

            SafeHandle ProcessId;
            SafeHandle ParentProcessId;
            uint SessionId;
            ulong ProcessSequenceNumber;
            ulong CreateTime;
            ulong ExitTime;

            PPH_STRING ProcessName;
            PPH_STRING FileName;
            PPH_STRING CommandLine;
            /*PPH_STRING UserName;*/
        }

        /*internal bool PhProcessProviderInitialization(void);*/

        internal struct PPH_STRING { }

        //internal NTSTATUS PhOpenProcessToken() { }
        // TODO: investigate TokenAccessLevels

        // TODO: struct PTOKEN_USER
        internal struct PTOKEN_USER { }

        //TODO: struct TOKEN_ELEVATION_TYPE
        internal struct TOKEN_ELEVATION_TYPE { }

        // TODO: struct MANDATORY_LEVEL
        internal struct MANDATORY_LEVEL { }
    }
}
