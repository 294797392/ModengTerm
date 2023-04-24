using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminal.Session.Property
{
    /// <summary>
    /// 连接SSH服务器的身份验证方式
    /// </summary>
    public enum SSHAuthEnum
    {
        /// <summary>
        /// 不需要验证，直接输入IP地址和端口号就可以
        /// </summary>
        None,

        /// <summary>
        /// 密码验证
        /// </summary>
        Password,

        /// <summary>
        /// 公钥验证
        /// </summary>
        PulicKey
    }

    public class SSHSessionProperties : SessionProperties
    {
        public string KeyFilePath { get; set; }

        public string KeyFilePassphrase { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string ServerAddress { get; set; }

        public int ServerPort { get; set; }

        public SSHSessionProperties()
        {
            //this.TerminalColumns = DefaultValues.TerminalColumns;
            //this.TerminalRows = DefaultValues.TerminalRows;
            //this.TerminalName = DefaultValues.TerminalName;
        }
    }
}
