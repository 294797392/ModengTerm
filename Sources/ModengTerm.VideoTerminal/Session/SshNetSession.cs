using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.IO;
using XTerminal.Base;
using XTerminal.Base.DataModels;
using XTerminal.Base.Enumerations;

namespace XTerminal.Session
{
    /// <summary>
    /// 使用Rench.SshNet库实现的ssh会话
    /// </summary>
    public class SshNetSession : SessionDriver
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("SshNetSession");

        #endregion

        #region 实例变量

        private SshClient sshClient;
        private ShellStream stream;

        #endregion

        #region 属性

        public EventHandler<ShellDataEventArgs> DataReceived { get; set; }

        public EventHandler<ExceptionEventArgs> ErrorOccurred { get; set; }

        #endregion

        #region 构造方法

        public SshNetSession(XTermSession options) :
            base(options)
        {
        }

        #endregion

        #region 实例方法

        #endregion

        #region SessionDriver

        public override int Open()
        {
            #region 初始化身份验证方式

            SSHAuthTypeEnum authType = this.session.GetOption<SSHAuthTypeEnum>(OptionKeyEnum.SSH_SERVER_AUTH_TYPE);
            string userName = this.session.GetOption<string>(OptionKeyEnum.SSH_SERVER_USER_NAME);
            string password = this.session.GetOption<string>(OptionKeyEnum.SSH_SERVER_PASSWORD);
            string privateKeyFile = this.session.GetOption<string>(OptionKeyEnum.SSH_SERVER_PRIVATE_KEY_FILE);
            string passphrase = this.session.GetOption<string>(OptionKeyEnum.SSH_SERVER_Passphrase);

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

            string serverAddress = this.session.GetOption<string>(OptionKeyEnum.SSH_SERVER_ADDR);
            int serverPort = this.session.GetOption<int>(OptionKeyEnum.SSH_SERVER_PORT);
            ConnectionInfo connectionInfo = new ConnectionInfo(serverAddress, serverPort, userName, authentication);
            this.sshClient = new SshClient(connectionInfo);
            this.sshClient.KeepAliveInterval = TimeSpan.FromSeconds(20);
            this.sshClient.Connect();

            #endregion

            #region 创建终端

            string terminalType = this.session.GetOption<string>(OptionKeyEnum.SSH_TERM_TYPE);
            int columns = this.session.GetOption<int>(OptionKeyEnum.SSH_TERM_COL);
            int rows = this.session.GetOption<int>(OptionKeyEnum.SSH_TERM_ROW);
            int outputBufferSize = this.session.GetOption<int>(OptionKeyEnum.WRITE_BUFFER_SIZE);
            this.stream = this.sshClient.CreateShellStream(terminalType, (uint)columns, (uint)rows, 0, 0, outputBufferSize, null);
            this.stream.DataReceived += this.DataReceived;
            this.stream.ErrorOccurred += this.ErrorOccurred;
            this.stream.Open();

            #endregion

            return ResponseCode.SUCCESS;
        }

        public override void Close()
        {
            this.stream.DataReceived -= this.DataReceived;
            this.stream.ErrorOccurred -= this.ErrorOccurred;
            this.stream.Dispose();
            this.sshClient.Disconnect();
        }

        public override int Write(byte[] bytes)
        {
            this.stream.Write(bytes, 0, bytes.Length);
            this.stream.Flush();
            return ResponseCode.SUCCESS;
        }

        internal override int Read(byte[] buffer)
        {
            // SshNet必须使用异步方式，不可以同步读取
            throw new InvalidOperationException();
            return this.stream.Read(buffer, 0, buffer.Length);
        }

        public override void Resize(int row, int col)
        {
            this.stream.SendWindowChangeRequest((uint)col, (uint)row, 0, 0);
        }

        #endregion

        #region 实例方法

        #endregion

        #region 事件处理器

        #endregion
    }
}
