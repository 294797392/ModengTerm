using ModengTerm.Base.Enumerations;
using ModengTerm.FileTrans.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.FileTrans.Clients
{
    public static class FsClientFactory
    {
        public static FsClientBase Create(FsClientOptions options)
        {
            switch (options.Type)
            {
                case FileSystemTypeEnum.Sftp: return new SshNetSftpClient();
                case FileSystemTypeEnum.LocalFs: return new LocalFsClient();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
