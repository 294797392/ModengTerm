using ModengTerm.Base.Enumerations;
using ModengTerm.Ftp.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Ftp.FileSystems
{
    public static class FileSystemFactory
    {
        public static FileSystem Create(FileSystemOptions options)
        {
            switch (options.Type)
            {
                case FileSystemTypeEnum.Sftp: return new SshNetSftpFileSystem();
                case FileSystemTypeEnum.Win32: return new Win32FileSystem();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
