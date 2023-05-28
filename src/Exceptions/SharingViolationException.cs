/**
 * Copyright (c) 2023 Noah Sherwin
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
using System;
using System.Runtime.Serialization;
using PInvoke;
using Windows.Win32.Foundation;

namespace HXE.Exceptions;

[Serializable]
internal class SharingViolationException : System.IO.IOException
{
    public SharingViolationException()
    { }

    public SharingViolationException(string message) : base(message)
    { }

    public SharingViolationException(string message, Exception innerException) : base(message, innerException)
    { }

    protected SharingViolationException(SerializationInfo info, StreamingContext context) : base(info, context)
    { }

    public SharingViolationException(string message, int hresult) : base(message, hresult)
    { }

    public static SharingViolationException FromWin32ErrorCode(Win32ErrorCode win32ErrorCode)
    {
        if (win32ErrorCode is not Win32ErrorCode.ERROR_SHARING_VIOLATION)
            throw new ArgumentException($"This method can only be used with {Win32ErrorCode.ERROR_SHARING_VIOLATION}.", paramName: nameof(win32ErrorCode));

        var win32Exception = new Win32Exception(win32ErrorCode);
        return new SharingViolationException(win32Exception.Message, win32Exception);
    }

    public static SharingViolationException FromWin32ErrorCode(WIN32_ERROR win32ErrorCode)
    {
        if (win32ErrorCode is not WIN32_ERROR.ERROR_SHARING_VIOLATION)
            throw new ArgumentException($"This method can only be used with {Win32ErrorCode.ERROR_SHARING_VIOLATION}.", paramName: nameof(win32ErrorCode));

        var win32Exception = new Win32Exception((Win32ErrorCode)win32ErrorCode);
        return new SharingViolationException(win32Exception.Message, win32Exception);
    }
}
