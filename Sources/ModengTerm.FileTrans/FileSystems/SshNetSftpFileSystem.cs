using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations.Ssh;
using ModengTerm.Base.ServiceAgents;
using ModengTerm.FileTrans.DataModels;
using ModengTerm.FileTrans.Enumerations;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System.Text;

namespace ModengTerm.FileTrans.Clients
{
    public class SshNetSftpFileSystem : FileSystem
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("SshNetSftpClient");

        #endregion

        #region 实例变量

        private SftpClient client;

        #endregion

        #region FileSystem

        public override int Open()
        {
            SftpFileSystemOptions clientOptions = this.options as SftpFileSystemOptions;

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

        public override List<FsItemInfo> ListItems(string directory)
        {
            List<FsItemInfo> fsItems = new List<FsItemInfo>();

            IEnumerable<ISftpFile> fileList = this.client.ListDirectory(directory);

            foreach (ISftpFile file in fileList)
            {
                // 会多出来一个名字是.的目录，不知道是做什么的
                if (file.Name == "." || file.Name == "..")
                {
                    continue;
                }

                FsItemInfo fsItem = new FsItemInfo();
                fsItem.Name = file.Name;
                fsItem.FullPath = file.FullName;
                fsItem.Size = file.Length;
                fsItem.LastUpdateTime = file.LastWriteTime;
                fsItem.IsHidden = VTBaseUtils.IsUnixHiddenFile(file.Name);

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

        public override List<FsItemInfo> GetDirectoryChains(string directory)
        {
            List<FsItemInfo> fsItems = new List<FsItemInfo>();

            string[] strings = directory.Split(VTBaseConsts.SlashBackslashSplitters, StringSplitOptions.RemoveEmptyEntries);
            string fullPath = string.Empty;

            for (int i = 0; i < strings.Length; i++)
            {
                string dirPart = strings[i];

                if (i == 0)
                {
                    fullPath = string.Format("/{0}", dirPart);
                }
                else
                {
                    fullPath = string.Format("{0}/{1}", fullPath, dirPart);
                }

                FsItemInfo directoryItem = new FsItemInfo()
                {
                    Name = dirPart,
                    FullPath = fullPath,
                    Type = FsItemTypeEnum.Directory
                };

                fsItems.Add(directoryItem);

                fullPath = directoryItem.FullPath;
            }

            return fsItems;
        }

        public override List<FsItemInfo> ListRootItems()
        {
            return this.ListItems("/");
        }

        public override void ChangeDirectory(string directory)
        {
            this.client.ChangeDirectory(directory);
        }

        public override void CreateDirectory(string directory)
        {
            this.client.CreateDirectory(directory);
        }

        public override void DeleteFile(string filePath)
        {
            this.client.DeleteFile(filePath);
        }

        public override void DeleteDirectory(string directoryPath)
        {
            this.client.DeleteDirectory(directoryPath);
        }

        public override void RenameFile(string oldPath, string newPath)
        {
            this.client.RenameFile(oldPath, newPath);
        }

        public override void RenameDirectory(string oldPath, string newPath)
        {
            this.client.RenameFile(oldPath, newPath);
        }

        public override bool IsDirectoryExist(string directoryPath)
        {
            return this.client.Exists(directoryPath);
        }

        public override bool IsFileEixst(string filePath)
        {
            return this.client.Exists(filePath);
        }


        public override Stream OpenRead(string filePath)
        {
            return this.client.OpenRead(filePath);
        }

        public override Stream OpenWrite(string filePath)
        {
            return this.client.OpenWrite(filePath);
        }

        #endregion
    }
}
