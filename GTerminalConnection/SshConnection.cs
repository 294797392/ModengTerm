using ICare.Utility.Misc.DS;
using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.Collections.Generic;

namespace GTerminalConnection
{
    public class SSHConnection : IConnection
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("SshConnection");

        #endregion

        #region 事件

        public event Action<object, byte[]> DataReceived;

        #endregion

        #region 实例变量

        private SshClient sshClient;
        private ShellStream shellStream;
        private SshConnectionAuthorition authorition;
        private BufferQueue<byte> stremQueue;

        #endregion

        #region 属性

        public ConnectionProtocols Protocol
        {
            get { return ConnectionProtocols.SSH; }
        }

        public IConnectionAuthorition Authorition { get; set; }

        #endregion

        #region 实例方法

        #endregion

        #region 公开接口

        public bool Connect()
        {
            this.NotifyVTStreamStatus(VTStreamState.Init);

            this.authorition = this.Authorition as SshConnectionAuthorition;

            var authentications = new List<AuthenticationMethod>();
            if (!string.IsNullOrEmpty(this.authorition.KeyFilePath))
            {
                var privateKeyFile = new PrivateKeyFile(this.authorition.KeyFilePath, this.authorition.KeyFilePassphrase);
                authentications.Add(new PrivateKeyAuthenticationMethod(this.authorition.UserName, privateKeyFile));
            }
            authentications.Add(new PasswordAuthenticationMethod(this.authorition.UserName, this.authorition.Password));
            ConnectionInfo connectionInfo = new ConnectionInfo(this.authorition.ServerAddress, this.authorition.ServerPort, this.authorition.UserName, authentications.ToArray());

            try
            {
                this.sshClient = new SshClient(connectionInfo);
                this.sshClient.Connect();
                this.sshClient.KeepAliveInterval = TimeSpan.FromSeconds(20);
                this.shellStream = this.sshClient.CreateShellStream(this.authorition.TerminalName, this.authorition.TerminalColumns, this.authorition.TerminalRows, 0, 0, 1000);
                this.shellStream.DataReceived += this.ShellStream_DataReceived;
            }
            catch (Exception ex)
            {
                logger.Error("初始化SshConnection异常", ex);
                return false;
            }

            this.stremQueue = new BufferQueue<byte>(4096);

            this.NotifyVTStreamStatus(VTStreamState.Ready);

            return true;
        }

        public bool Disconnect()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IVTStream

        public event Action<object, VTStreamState> StatusChanged;

        public byte[] Read(int size)
        {
            byte[] buff = new byte[size];
            this.shellStream.Read(buff, 0, size);
            return buff;
        }

        public byte Read()
        {
            return this.stremQueue.Dequeue();
        }

        public bool Write(byte data)
        {
            this.shellStream.WriteByte(data);
            this.shellStream.Flush();
            return true;
        }

        public bool Write(byte[] data)
        {
            this.shellStream.Write(data, 0, data.Length);
            this.shellStream.Flush();
            return true;
        }

        public bool EOF
        {
            get
            {
                return this.shellStream == null
                    || this.sshClient == null
                    || !this.sshClient.IsConnected
                    //|| !this.shellStream.CanRead
                    ;
            }
        }

        private void NotifyVTStreamStatus(VTStreamState state)
        {
            if (this.StatusChanged != null)
            {
                this.StatusChanged(this, state);
            }
        }

        #endregion

        #region 实例方法

        private void ShellStream_DataReceived(object sender, ShellDataEventArgs e)
        {
            foreach (byte c in e.Data)
            {
                this.stremQueue.Enqueue(c);
            }
        }

        #endregion
    }
}
