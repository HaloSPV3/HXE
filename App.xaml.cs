using System;
using System.IO;
using System.Windows;
using PInvoke;
using static PInvoke.Kernel32;

namespace test_fody
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public void Dummy()
        {
            SafeObjectHandle hDirectory = CreateFile(
                filename: Environment.CurrentDirectory,
                access: Kernel32.FileAccess.FILE_GENERIC_READ | Kernel32.FileAccess.FILE_GENERIC_WRITE,
                share: Kernel32.FileShare.None,
                securityAttributes: SECURITY_ATTRIBUTES.Create(),
                creationDisposition: CreationDisposition.OPEN_EXISTING,
                flagsAndAttributes: CreateFileFlags.FILE_FLAG_BACKUP_SEMANTICS,
                templateFile: null
            ) ?? throw new NullReferenceException();
        }
    }
}
