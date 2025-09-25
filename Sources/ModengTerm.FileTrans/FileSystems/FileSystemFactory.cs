using ModengTerm.Base.Enumerations;
using ModengTerm.FileTrans.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.FileTrans.Clients
{
    public static class FileSystemFactory
    {
        public static FileSystem Create(FileSystemOptions options)
        {
            switch (options.Type)
            {
                case FileSystemTypeEnum.Sftp: return new SshNetSftpFileSystem();
                case FileSystemTypeEnum.LocalFs: return new LocalFileSystem();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
