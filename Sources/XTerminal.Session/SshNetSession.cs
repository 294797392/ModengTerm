using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.Collections.Generic;
using XTerminal.Base;
using XTerminal.Session.Property;

namespace XTerminal.Session
{
    /// <summary>
    /// 使用Rench.SshNet库实现的ssh会话
    /// </summary>
    public class SshNetSession : SessionBase
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("SSHSocket");

        #endregion

        #region 实例变量

        private SshClient sshClient;
        private ShellStream stream;
        private SessionProperties sessionProperties;

        #endregion

        #region 属性

        #endregion

        #region 构造方法

        public SshNetSession(VTInitialOptions options) :
            base(options)
        {
        }

        #endregion

        #region 实例方法

        #endregion

        #region SessionBase

        public override int Connect()
        {
            this.sessionProperties = this.options.SessionProperties;
            var authentications = new List<AuthenticationMethod>();
            if (!string.IsNullOrEmpty(this.sessionProperties.KeyFilePath))
            {
                var privateKeyFile = new PrivateKeyFile(this.sessionProperties.KeyFilePath, this.sessionProperties.KeyFilePassphrase);
                authentications.Add(new PrivateKeyAuthenticationMethod(this.sessionProperties.UserName, privateKeyFile));
            }
            authentications.Add(new PasswordAuthenticationMethod(this.sessionProperties.UserName, this.sessionProperties.Password));
            ConnectionInfo connectionInfo = new ConnectionInfo(this.sessionProperties.ServerAddress, this.sessionProperties.ServerPort, this.sessionProperties.UserName, authentications.ToArray());
            this.sshClient = new SshClient(connectionInfo);
            this.sshClient.KeepAliveInterval = TimeSpan.FromSeconds(20);
            this.sshClient.Connect();

            Dictionary<TerminalModes, uint> terminalModeValues = new Dictionary<TerminalModes, uint>();
            //terminalModeValues[TerminalModes.ECHOCTL] = 1;
            //terminalModeValues[TerminalModes.IEXTEN] = 1;

            TerminalProperties terminalOptions = this.options.TerminalProperties;
            this.stream = this.sshClient.CreateShellStream(this.options.TerminalProperties.GetTerminalName(), (uint)terminalOptions.Columns, (uint)terminalOptions.Rows, 0, 0, this.options.ReadBufferSize, terminalModeValues);
            this.stream.DataReceived += this.Stream_DataReceived;

            return ResponseCode.SUCCESS;
        }

        public override void Disconnect()
        {
            this.stream.DataReceived -= this.Stream_DataReceived;
            this.sshClient.Disconnect();

            this.stream.Dispose();
            this.sshClient.Disconnect();
        }

        public override int Input(VTInputEvent ievt)
        {
            //byte[] bytes = this.keyboard.TranslateInput(ievt);
            //return this.Write(bytes);
            throw new NotImplementedException();
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

        #endregion

        #region 实例方法

        private void Stream_DataReceived(object sender, ShellDataEventArgs e)
        {
            base.NotifyDataReceived(e.Data);
        }

        #endregion
    }
}
