using log4net.Repository.Hierarchy;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations.Ssh;
using ModengTerm.Base.ServiceAgents;
using ModengTerm.Base;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModengTerm.FileTrans.DataModels;
using Renci.SshNet.Sftp;
using ModengTerm.FileTrans.Enumerations;
using ModengTerm.FileTrans.Clients.Channels;

namespace ModengTerm.FileTrans.Clients
{
    public class SshNetSftpClient : FsClientBase
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("SshNetSftpClient");

        #endregion

        #region 实例变量

        private SftpClient client;

        #endregion

        #region FsClientBase

        public override int Open()
        {
            SftpClientOptions clientOptions = this.options as SftpClientOptions;

            #region 初始化身份验证方式

            AuthenticationMethod authentication = null;
            switch (clientOptions.AuthenticationType)
            {
                case SSHAuthTypeEnum.None:
                    {
                        authentication = new NoneAuthenticationMethod(clientOptions.UserName);
                        break;
                    }

                case SSHAuthTypeEnum.Password:
                    {
                        authentication = new PasswordAuthenticationMethod(clientOptions.UserName, clientOptions.Password);
                        break;
                    }

                case SSHAuthTypeEnum.PrivateKey:
                    {
                        ServiceAgent serviceAgent = ServiceAgentFactory.Get();

                        PrivateKey privateKey = serviceAgent.GetPrivateKey(clientOptions.PrivateKeyId);
                        if (privateKey == null)
                        {
                            logger.ErrorFormat("登录失败, 密钥不存在");
                            return ResponseCode.PRIVATE_KEY_NOT_FOUND;
                        }

                        byte[] privateKeyData = Encoding.ASCII.GetBytes(privateKey.Content);
                        using (MemoryStream ms = new MemoryStream(privateKeyData))
                        {
                            var keyFile = new PrivateKeyFile(ms, clientOptions.Passphrase);
                            authentication = new PrivateKeyAuthenticationMethod(clientOptions.UserName, keyFile);
                        }
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            #endregion

            ConnectionInfo connectionInfo = new ConnectionInfo(clientOptions.ServerAddress, clientOptions.ServerPort, clientOptions.UserName, authentication);
            SftpClient sftpClient = new SftpClient(connectionInfo);
            sftpClient.Connect();

            this.client = sftpClient;

            return ResponseCode.SUCCESS;
        }

        public override void Close()
        {
            if (this.client != null)
            {
                this.client.Disconnect();
                this.client.Dispose();
            }
        }

        public override List<FsItemInfo> ListFiles(string directory)
        {
            List<FsItemInfo> fsItems = new List<FsItemInfo>();

            IEnumerable<ISftpFile> fileList = this.client.ListDirectory(directory);

            foreach (ISftpFile file in fileList)
            {
                FsItemInfo fsItem = new FsItemInfo();
                fsItem.Name = file.Name;
                fsItem.FullPath = file.FullName;
                fsItem.Size = file.Length;
                fsItem.LastUpdateTime = file.LastWriteTime;

                if (file.IsDirectory)
                {
                    fsItem.Type = FsItemTypeEnum.Directory;
                }
                else
                {
                    fsItem.Type = FsItemTypeEnum.File;
                }

                fsItems.Add(fsItem);
            }

            return fsItems;
        }

        public override void ChangeDirectory(string directory)
        {
            this.client.ChangeDirectory(directory);
        }

        public override bool CreateDirectory(string directory)
        {
            throw new NotImplementedException();
        }

        public override FsUploadChannel CreateUploadChannel()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
