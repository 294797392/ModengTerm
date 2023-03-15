using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.Collections.Generic;
using VideoTerminal.Options;

namespace XTerminal.Channels
{
    public class SSHChannel : VTChannel
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("SSHSocket");

        #endregion

        #region 实例变量

        private SshClient sshClient;
        private ShellStream stream;
        private SSHChannelAuthorition authorition;

        #endregion

        #region 属性

        #endregion

        #region 构造方法

        public SSHChannel(VTInitialOptions options) :
            base(options)
        {
        }

        #endregion

        #region 实例方法

        private string GetTerminalName(TerminalTypes types)
        {
            switch (types)
            {
                case TerminalTypes.VT100: return "vt100";
                case TerminalTypes.VT220: return "vt220";
                case TerminalTypes.XTerm: return "xterm";
                case TerminalTypes.XTerm256Color: return "xterm-256color";
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region VTChannel

        public override bool Connect()
        {
            this.NotifyStatusChanged(VTChannelState.Connecting);

            this.authorition = this.options.Authorition as SSHChannelAuthorition;
            var authentications = new List<AuthenticationMethod>();
            if (!string.IsNullOrEmpty(this.authorition.KeyFilePath))
            {
                var privateKeyFile = new PrivateKeyFile(this.authorition.KeyFilePath, this.authorition.KeyFilePassphrase);
                authentications.Add(new PrivateKeyAuthenticationMethod(this.authorition.UserName, privateKeyFile));
            }
            authentications.Add(new PasswordAuthenticationMethod(this.authorition.UserName, this.authorition.Password));
            ConnectionInfo connectionInfo = new ConnectionInfo(this.authorition.ServerAddress, this.authorition.ServerPort, this.authorition.UserName, authentications.ToArray());

            this.sshClient = new SshClient(connectionInfo);
            this.sshClient.Connect();
            this.sshClient.KeepAliveInterval = TimeSpan.FromSeconds(20);

            Dictionary<TerminalModes, uint> terminalModeValues = new Dictionary<TerminalModes, uint>();
            //terminalModeValues[TerminalModes.ECHOCTL] = 1;
            terminalModeValues[TerminalModes.IEXTEN] = 1;

            TerminalOptions terminalOptions = this.options.TerminalOption;
            //this.stream = this.sshClient.CreateShellStream(this.GetTerminalName(terminalOptions.Type), (uint)terminalOptions.Columns, (uint)terminalOptions.Rows, 0, 0, this.options.ReadBufferSize, terminalModeValues);
            this.stream = this.sshClient.CreateShellStream("xterm", (uint)terminalOptions.Columns, (uint)terminalOptions.Rows, 0, 0, this.options.ReadBufferSize, terminalModeValues);
            this.stream.DataReceived += this.Stream_DataReceived;

            this.NotifyStatusChanged(VTChannelState.Connected);

            return true;
        }

        public override bool Disconnect()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] bytes)
        {
            long left = bytes.Length;
            int readed = 0;

            while (left > 0)
            {
                int readLen = left > DefaultReadBufferSize ? DefaultReadBufferSize : (int)left;
                int size = this.stream.Read(bytes, readed, readLen);
                if (size == 0)
                {
                    return readed;
                }
                readed += size;
                left -= readed;
            }

            return readed;
        }

        public override byte Read()
        {
            throw new NotImplementedException();
        }

        public override bool Write(byte data)
        {
            this.stream.WriteByte(data);
            this.stream.Flush();
            return true;
        }

        public override bool Write(byte[] data)
        {
            this.stream.Write(data, 0, data.Length);
            this.stream.Flush();
            return true;
        }

        #endregion

        #region 实例方法

        private void Stream_DataReceived(object sender, ShellDataEventArgs e)
        {
            base.NotifyDataReceived(e.Data);
        }

        #endregion
    }
}
