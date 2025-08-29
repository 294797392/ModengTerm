using ModengTerm.Addon.Interactive;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.DataModels.Ssh;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.Enumerations.Ssh;
using ModengTerm.Base.ServiceAgents;
using Renci.SshNet;
using Renci.SshNet.Common;
using System.IO;
using System.Text;

namespace ModengTerm.Terminal.Engines
{
    /// <summary>
    /// 使用Rench.SshNet库实现的ssh会话
    /// </summary>
    public class SshNetChannel : ChannelBase, ISshChannel
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("SshNetSession");

        #endregion

        #region 实例变量

        private SshClient sshClient;
        private ShellStream stream;
        private List<PortForwardState> portForwardStates;

        private SshCommand sshCommand;

        #endregion

        #region 属性

        public SshClient SshClient { get { return this.sshClient; } }

        #endregion

        #region 构造方法

        public SshNetChannel()
        {
        }

        #endregion

        #region 实例方法

        #endregion

        #region ChannelBase

        public override int Open()
        {
            SshChannelOptions channelOptions = this.options as SshChannelOptions;

            #region 初始化身份验证方式

            AuthenticationMethod authentication = null;
            switch (channelOptions.AuthenticationType)
            {
                case SSHAuthTypeEnum.None:
                    {
                        authentication = new NoneAuthenticationMethod(channelOptions.UserName);
                        break;
                    }

                case SSHAuthTypeEnum.Password:
                    {
                        authentication = new PasswordAuthenticationMethod(channelOptions.UserName, channelOptions.Password);
                        break;
                    }

                case SSHAuthTypeEnum.PrivateKey:
                    {
                        ServiceAgent serviceAgent = ServiceAgentFactory.Get();

                        PrivateKey privateKey = serviceAgent.GetPrivateKey(channelOptions.PrivateKeyId);
                        if (privateKey == null)
                        {
                            logger.ErrorFormat("登录失败, 密钥不存在");
                            return ResponseCode.PRIVATE_KEY_NOT_FOUND;
                        }

                        byte[] privateKeyData = Encoding.ASCII.GetBytes(privateKey.Content);
                        using (MemoryStream ms = new MemoryStream(privateKeyData))
                        {
                            var keyFile = new PrivateKeyFile(ms, channelOptions.Passphrase);
                            authentication = new PrivateKeyAuthenticationMethod(channelOptions.UserName, keyFile);
                        }
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            #endregion

            #region 连接服务器

            ConnectionInfo connectionInfo = new ConnectionInfo(channelOptions.ServerAddress, channelOptions.ServerPort, channelOptions.UserName, authentication);
            this.sshClient = new SshClient(connectionInfo);
            this.sshClient.HostKeyReceived += SshClient_HostKeyReceived;
            this.sshClient.ServerIdentificationReceived += SshClient_ServerIdentificationReceived;
            this.sshClient.KeepAliveInterval = TimeSpan.FromSeconds(20);
            this.sshClient.Connect();

            #endregion

            #region 创建终端

            string terminalType = channelOptions.TerminalType;
            int columns = channelOptions.Column;
            int rows = channelOptions.Row;
            int recvBufferSize = channelOptions.ReceiveBufferSize;
            this.stream = this.sshClient.CreateShellStream(terminalType, (uint)columns, (uint)rows, 0, 0, recvBufferSize, null);

            #endregion

            #region 初始化端口转发

            this.portForwardStates = new List<PortForwardState>();
            List<PortForward> portForwards = channelOptions.PortForwards;
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

            this.sshCommand = this.sshClient.CreateCommand(string.Empty);

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
            this.sshClient.HostKeyReceived -= SshClient_HostKeyReceived;
            this.sshClient.ServerIdentificationReceived -= SshClient_ServerIdentificationReceived;

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

        private void SshClient_ServerIdentificationReceived(object? sender, SshIdentificationEventArgs e)
        {
        }

        private void SshClient_HostKeyReceived(object? sender, HostKeyEventArgs e)
        {
            e.CanTrust = true;
        }

        #endregion

        #region ISshEngine

        public string ExecuteScript(string script)
        {
            return this.sshCommand.Execute(script);
        }

        #endregion
    }
}
