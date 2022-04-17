/**
 * Copyright (c) 2022 Noah Sherwin
 *
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */
/// Compress a folder using NTFS compression in .NET
/// https://stackoverflow.com/a/624446
/// - Zack Elan https://stackoverflow.com/users/2461/zack-elan
/// - Goz https://stackoverflow.com/users/131140/goz
using PInvoke;

namespace HXE.Common
{
    internal class InfoWin32Exception : Win32Exception
    {
        /// <summary>
        ///     Initializes a new instance of the HXE.Common.InfoWin32Exception class.
        /// </summary>
        /// <param name="error">The Win32 error code associated with this exception. The Message of the exception is set to the error code's associate string if available.</param>
        public InfoWin32Exception(Win32ErrorCode error) : base(error, error.GetMessage())
        {
        }

        /// <summary>
        ///     Initializes a new instance of the HXE.Common.InfoWin32Exception class.
        /// </summary>
        /// <param name="error">The Win32 error code associated with this exception.</param>
        public InfoWin32Exception(int error) : this((Win32ErrorCode) error)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the HXE.Common.InfoWin32Exception class.
        /// </summary>
        /// <param name="error">The Win32 error code associated with this exception. The Message of the exception is set to the error code's associate string if available.</param>
        /// <param name="message">The message for this exception. This is appended to the system message associated with the error code.</param>
        public InfoWin32Exception(Win32ErrorCode error, string message) : base(error, error.GetMessage() + " : " + message)
        {
        }
    }
}
