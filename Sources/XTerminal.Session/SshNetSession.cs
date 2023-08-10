using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using XTerminal.Base;
using XTerminal.Base.DataModels;
using XTerminal.Base.Enumerations;

namespace XTerminal.Session
{
    /// <summary>
    /// 使用Rench.SshNet库实现的ssh会话
    /// </summary>
    public class SshNetSession : SessionBase
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("SshNetSession");

        #endregion

        #region 实例变量

        private SshClient sshClient;
        private ShellStream stream;

        #endregion

        #region 属性

        #endregion

        #region 构造方法

        public SshNetSession(XTermSession options) :
            base(options)
        {
        }

        #endregion

        #region 实例方法

        #endregion

        #region SessionBase

        public override int Open()
        {
            #region 初始化身份验证方式

            SSHAuthTypeEnum authType = this.options.GetOption<SSHAuthTypeEnum>(OptionKeyEnum.SSH_AUTH_TYPE);
            string userName = this.options.GetOption<string>(OptionKeyEnum.SSH_SERVER_USER_NAME);
            string password = this.options.GetOption<string>(OptionKeyEnum.SSH_SERVER_PASSWORD);
            string privateKeyFile = this.options.GetOption<string>(OptionKeyEnum.SSH_SERVER_PRIVATE_KEY_FILE);
            string passphrase = this.options.GetOption<string>(OptionKeyEnum.SSH_SERVER_Passphrase);

            AuthenticationMethod authentication = null;
            switch (authType)
            {
                case SSHAuthTypeEnum.None:
                    {
                        authentication = new NoneAuthenticationMethod(userName);
                        break;
                    }

                case SSHAuthTypeEnum.Password:
                    {
                        authentication = new PasswordAuthenticationMethod(userName, password);
                        break;
                    }

                case SSHAuthTypeEnum.PrivateKey:
                    {
                        byte[] privateKey = File.ReadAllBytes(privateKeyFile);
                        using (MemoryStream ms = new MemoryStream(privateKey))
                        {
                            var keyFile = new PrivateKeyFile(ms, passphrase);
                            authentication = new PrivateKeyAuthenticationMethod(userName, keyFile);
                        }
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            #endregion

            #region 连接服务器

            string serverAddress = this.options.GetOption<string>(OptionKeyEnum.SSH_SERVER_ADDR);
            int serverPort = this.options.GetOption<int>(OptionKeyEnum.SSH_SERVER_PORT);
            ConnectionInfo connectionInfo = new ConnectionInfo(serverAddress, serverPort, userName, authentication);
            this.sshClient = new SshClient(connectionInfo);
            this.sshClient.KeepAliveInterval = TimeSpan.FromSeconds(20);
            this.sshClient.Connect();

            #endregion

            #region 创建终端

            string terminalType = this.options.GetOption<string>(OptionKeyEnum.SSH_TERM_TYPE);
            int columns = this.options.GetOption<int>(OptionKeyEnum.SSH_TERM_COL);
            int rows = this.options.GetOption<int>(OptionKeyEnum.SSH_TERM_ROW);
            int outputBufferSize = this.options.GetOption<int>(OptionKeyEnum.WRITE_BUFFER_SIZE);
            this.stream = this.sshClient.CreateShellStream(terminalType, (uint)columns, (uint)rows, 0, 0, outputBufferSize, null);

            #endregion

            return ResponseCode.SUCCESS;
        }

        public override void Close()
        {
            this.sshClient.Disconnect();

            this.stream.Dispose();
            this.sshClient.Disconnect();
        }

        public override int Write(byte[] bytes)
        {
            try
            {
                this.stream.Write(bytes, 0, bytes.Length);
                this.stream.Flush();
                return ResponseCode.SUCCESS;
            }
            catch (Exception ex)
            {
                logger.Error("ShellStream.Write异常", ex);
                return ResponseCode.FAILED;
            }
        }

        internal override int Read(byte[] buffer)
        {
            return this.stream.Read(buffer, 0, buffer.Length);
        }

        #endregion

        #region 实例方法

        #endregion
    }
}
