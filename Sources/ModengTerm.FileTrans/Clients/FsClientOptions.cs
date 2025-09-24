using ModengTerm.Base.DataModels.Ssh;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.Enumerations.Ssh;
using ModengTerm.FileTrans.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.FileTrans.Clients
{
    public abstract class FsClientOptions
    {
        public abstract FileSystemTypeEnum Type { get; }

        /// <summary>
        /// 初始目录
        /// TODO：考虑删除这个属性，由FsClientTransport维护初始目录
        /// </summary>
        public string InitialDirectory { get; set; }
    }

    public class SftpClientOptions : FsClientOptions
    {
        public override FileSystemTypeEnum Type => FileSystemTypeEnum.Sftp;

        /// <summary>
        /// 如果该会话是Ssh
        /// 指定要使用的Ssh验证方式
        /// </summary>
        public SSHAuthTypeEnum AuthenticationType { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string PrivateKeyId { get; set; }

        public string Passphrase { get; set; }

        public string ServerAddress { get; set; }

        public int ServerPort { get; set; }
    }

    public class LocalFsClientOptions : FsClientOptions
    {
        public override FileSystemTypeEnum Type => FileSystemTypeEnum.LocalFs;
    }
}
