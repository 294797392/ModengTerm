using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using XTerminal.Base;
using XTerminal.Base.DataModels;
using XTerminal.Base.DataModels.Session;
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
        private ConnectionOptions sessionProperties;

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
            this.sessionProperties = this.options.ConnectionOptions;

            #region 初始化身份验证方式

            AuthenticationMethod authentication = null;
            switch ((SSHAuthTypeEnum)this.sessionProperties.SSHAuthType)
            {
                case SSHAuthTypeEnum.None:
                    {
                        authentication = new NoneAuthenticationMethod(this.sessionProperties.UserName);
                        break;
                    }

                case SSHAuthTypeEnum.Password:
                    {
                        authentication = new PasswordAuthenticationMethod(this.sessionProperties.UserName, this.sessionProperties.Password);
                        break;
                    }

                case SSHAuthTypeEnum.PrivateKey:
                    {
                        using (MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(this.sessionProperties.PrivateKey)))
                        {
                            var privateKeyFile = new PrivateKeyFile(ms, this.sessionProperties.Passphrase);
                            authentication = new PrivateKeyAuthenticationMethod(this.sessionProperties.UserName, privateKeyFile);
                        }
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            #endregion

            #region 连接服务器

            ConnectionInfo connectionInfo = new ConnectionInfo(this.sessionProperties.ServerAddress, this.sessionProperties.ServerPort, this.sessionProperties.UserName, authentication);
            this.sshClient = new SshClient(connectionInfo);
            this.sshClient.KeepAliveInterval = TimeSpan.FromSeconds(20);
            this.sshClient.Connect();

            #endregion

            #region 创建终端

            Dictionary<TerminalModes, uint> terminalModeValues = new Dictionary<TerminalModes, uint>();
            //terminalModeValues[TerminalModes.ECHOCTL] = 1;
            //terminalModeValues[TerminalModes.IEXTEN] = 1;

            TerminalOptions terminalOptions = this.options.TerminalOptions;
            this.stream = this.sshClient.CreateShellStream(this.options.TerminalOptions.GetTerminalName(), (uint)terminalOptions.Columns, (uint)terminalOptions.Rows, 0, 0, this.options.OutputBufferSize, terminalModeValues);

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
