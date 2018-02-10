using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XTerminal.Terminal
{
    public class SSHTerminal : ITerminal
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("SSHTerminal");

        #endregion

        #region 事件

        public event Action<object, byte[], string> DataReceived;

        #endregion

        #region 实例变量

        private SshClient sshClient;
        private ShellStream shellStream;
        private SSHTerminalAuthorition authorition;
        private BinaryWriter writer;
        private IEscapeSequencesParser parser;

        #endregion

        #region 属性

        public ITerminalAuthorition Authorition { get; set; }

        #endregion

        #region 实例方法

        protected void NotifyDataReceived(byte[] data, string line)
        {
            if (this.DataReceived != null)
            {
                this.DataReceived(this, data, line);
            }
        }

        #endregion

        #region 公开接口

        public bool Connect()
        {
            this.authorition = this.Authorition as SSHTerminalAuthorition;

            var authentications = new List<AuthenticationMethod>();
            if (!string.IsNullOrEmpty(this.authorition.KeyFilePath))
            {
                var privateKeyFile = new PrivateKeyFile(this.authorition.KeyFilePath, this.authorition.KeyFilePassphrase);
                authentications.Add(new PrivateKeyAuthenticationMethod(this.authorition.UserName, privateKeyFile));
            }
            authentications.Add(new PasswordAuthenticationMethod(this.authorition.UserName, this.authorition.Password));
            ConnectionInfo connectionInfo = new ConnectionInfo(this.authorition.ServerAddress, this.authorition.ServerPort, this.authorition.UserName, authentications.ToArray());

            this.parser = new AnsiEscapeSequencesParser();

            try
            {
                this.sshClient = new SshClient(connectionInfo);
                this.sshClient.Connect();
                this.sshClient.KeepAliveInterval = TimeSpan.FromSeconds(20);
                this.shellStream = this.sshClient.CreateShellStream(this.authorition.TerminalName, this.authorition.TerminalColumns, this.authorition.TerminalRows, 0, 0, 1000);
                this.shellStream.DataReceived += this.ShellStream_DataReceived;
                this.writer = new BinaryWriter(this.shellStream, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                logger.Error("初始化SSHClient异常", ex);
                return false;
            }

            return true;
        }

        public bool Disconnect()
        {
            throw new NotImplementedException();
        }

        public bool Initialize()
        {
            throw new NotImplementedException();
        }

        public bool Release()
        {
            throw new NotImplementedException();
        }

        public bool Send(string text)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(text);
                this.writer.Write(data);
                this.writer.Flush();
                return true;
            }
            catch (Exception ex)
            {
                logger.Error("SSHTerminal Send异常", ex);
                return false;
            }
        }

        #endregion

        #region 事件处理器

        private void ShellStream_DataReceived(object sender, Renci.SshNet.Common.ShellDataEventArgs e)
        {
            if (this.DataReceived != null)
            {
                this.DataReceived(this, e.Data, e.Line);
            }
            var list = this.parser.Parse(e.Data);
            if (list.Count > 2)
            {
                Console.WriteLine("s");
            }

            Console.WriteLine();
        }

        #endregion
    }
}