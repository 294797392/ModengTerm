using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.ServiceAgents;
using ModengTerm.Terminal.DataModels;
using ModengTerm.Terminal.Enumerations;
using Renci.SshNet;
using Renci.SshNet.Common;
using System.IO;
using System.Text;
using XTerminal.Base.Enumerations;

namespace ModengTerm.Terminal.Session
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
        private List<PortForwardState> portForwardStates;
        private ServiceAgent serviceAgent;

        #endregion

        #region 属性

        public SshClient SshClient { get { return this.sshClient; } }

        #endregion

        #region 构造方法

        public SshNetSession(XTermSession options) :
            base(options)
        {
            this.serviceAgent = VTermApp.Context.ServiceAgent;
        }

        #endregion

        #region 实例方法

        #endregion

        #region SessionDriver

        public override int Open()
        {
            #region 初始化身份验证方式

            SSHAuthTypeEnum authType = this.session.GetOption<SSHAuthTypeEnum>(OptionKeyEnum.SSH_AUTH_TYPE);
            string userName = this.session.GetOption<string>(OptionKeyEnum.SSH_USER_NAME);
            string password = this.session.GetOption<string>(OptionKeyEnum.SSH_PASSWORD);
            string privateKeyId = this.session.GetOption<string>(OptionKeyEnum.SSH_PRIVATE_KEY_FILE);
            string passphrase = this.session.GetOption<string>(OptionKeyEnum.SSH_Passphrase);

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
                        PrivateKey privateKey = this.serviceAgent.GetPrivateKey(privateKeyId);
                        if (privateKey == null) 
                        {
                            logger.ErrorFormat("登录失败, 密钥不存在");
                            return ResponseCode.PRIVATE_KEY_NOT_FOUND;
                        }

                        byte[] privateKeyData = Encoding.ASCII.GetBytes(privateKey.Content);
                        using (MemoryStream ms = new MemoryStream(privateKeyData))
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

            string serverAddress = this.session.GetOption<string>(OptionKeyEnum.SSH_ADDR);
            int serverPort = this.session.GetOption<int>(OptionKeyEnum.SSH_PORT);
            ConnectionInfo connectionInfo = new ConnectionInfo(serverAddress, serverPort, userName, authentication);
            this.sshClient = new SshClient(connectionInfo);
            this.sshClient.KeepAliveInterval = TimeSpan.FromSeconds(20);
            this.sshClient.Connect();

            #endregion

            #region 创建终端

            string terminalType = this.session.GetOption<string>(OptionKeyEnum.SSH_TERM_TYPE);
            int columns = this.session.GetOption<int>(OptionKeyEnum.SSH_TERM_COL);
            int rows = this.session.GetOption<int>(OptionKeyEnum.SSH_TERM_ROW);
            int readBufferSize = this.session.GetOption<int>(OptionKeyEnum.SSH_READ_BUFFER_SIZE);
            this.stream = this.sshClient.CreateShellStream(terminalType, (uint)columns, (uint)rows, 0, 0, readBufferSize, null);

            #endregion

            #region 初始化端口转发

            this.portForwardStates = new List<PortForwardState>();
            List<PortForward> portForwards = this.session.GetOption<List<PortForward>>(OptionKeyEnum.SSH_PORT_FORWARDS);
            foreach (PortForward portForward in portForwards)
            {
                ForwardedPort forwardedPort = this.CreateForwardPort(portForward);
                this.sshClient.AddForwardedPort(forwardedPort);

                if (portForward.AutoOpen)
                {
                    forwardedPort.Start();
                }

                portForwardStates.Add(new PortForwardState()
                {
                    Status = portForward.AutoOpen ? PortForwardStatusEnum.Opened : PortForwardStatusEnum.Closed,
                    DriverObject = forwardedPort,
                    SourceAddress = portForward.SourceAddress,
                    SourcePort = portForward.SourcePort,
                    DestinationAddress = portForward.DestinationAddress,
                    DestinationPort = portForward.DestinationPort,
                    AutoOpen = portForward.AutoOpen
                });
            }

            #endregion
            
            //Task.Factory.StartNew(() => 
            //{
            //    SshCommand sshCommand = this.sshClient.CreateCommand("tail -f ~/1");

            //    Stream stream = sshCommand.OutputStream;

            //    sshCommand.ExecuteAsync();

            //    while (true) 
            //    {
            //        byte[] buffer = new byte[1024];
            //        int n = stream.Read(buffer, 0, buffer.Length);
            //        byte[] buffer1 = new byte[n];
            //        Buffer.BlockCopy(buffer, 0, buffer1, 0, buffer1.Length);
            //        Console.WriteLine(Encoding.ASCII.GetString(buffer1));
            //    }
            //});

            return ResponseCode.SUCCESS;
        }

        public override void Close()
        {
            this.stream.Dispose();
            this.sshClient.Disconnect();

            #region 关闭端口转发

            foreach (PortForwardState forwardState in this.portForwardStates)
            {
                if (forwardState.Status == PortForwardStatusEnum.Opened) 
                {
                    ForwardedPort forwardedPort = forwardState.DriverObject as ForwardedPort;
                    forwardedPort.Stop();
                    forwardState.Status = PortForwardStatusEnum.Closed;
                }
            }

            #endregion
        }

        public override void Write(byte[] bytes)
        {
            this.stream.Write(bytes, 0, bytes.Length);
            this.stream.Flush();
        }

        internal override int Read(byte[] buffer)
        {
            return this.stream.Read(buffer, 0, buffer.Length);
        }

        public override void Resize(int row, int col)
        {
            this.stream.SendWindowChangeRequest((uint)col, (uint)row, 0, 0);
        }

        public override int Control(int command, object parameter, out object result)
        {
            result = null;

            switch (command)
            {
                case SSHControlCodes.GetForwardPortStates:
                    {
                        result = this.portForwardStates;
                        return ResponseCode.SUCCESS;
                    }

                default:
                    {
                        return ResponseCode.NOT_SUPPORTED;
                    }
            }
        }

        #endregion

        #region 实例方法

        private ForwardedPort CreateForwardPort(PortForward portForward)
        {
            switch ((PortForwardTypeEnum)portForward.Type)
            {
                case PortForwardTypeEnum.Local:
                    {
                        return new ForwardedPortLocal(portForward.SourceAddress, (uint)portForward.SourcePort, portForward.DestinationAddress, (uint)portForward.DestinationPort);
                    }

                case PortForwardTypeEnum.Remote:
                    {
                        return new ForwardedPortRemote(portForward.SourceAddress, (uint)portForward.SourcePort, portForward.DestinationAddress, (uint)portForward.DestinationPort);
                    }

                case PortForwardTypeEnum.Dynamic:
                    {
                        return new ForwardedPortDynamic(portForward.SourceAddress, (uint)portForward.SourcePort);
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region 事件处理器

        #endregion
    }
}
