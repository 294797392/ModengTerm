using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using XTerminal.Connections;
using XTerminal.Terminal;

namespace XTerminal.Terminals
{
    /// <summary>
    /// 1.解析用户输入的原始字符，解析成VT100字符或控制字符，并发送给终端
    /// 2.解析远程主机发送过来的控制字符或普通字符，并转转成客户端可识别的动作
    /// </summary>
    public abstract class AbstractTerminal : IVideoTerminal
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("AbstractTerminal");

        #region 事件

        public event Action<object, IEnumerable<AbstractTerminalAction>, byte[]> CommandReceived;
        public event Action<object, TerminalConnectionStatus> StatusChanged;

        #endregion

        #region 实例变量

        protected IConnection connection;

        #endregion

        #region 属性

        public ConnectionProtocols Protocols { get; set; }

        /// <summary>
        /// 设置主机使用的编码方式
        /// </summary>
        public virtual CharacterCodedBits CodedBits { get { return CharacterCodedBits._8Bit; } }

        #endregion

        #region 公开接口

        public int ConnectionTerminal(IConnectionAuthorition authorition)
        {
            this.connection = ConnectionFactory.Create(this.Protocols);
            this.connection.Authorition = authorition;
            this.connection.DataReceived += Connection_DataReceived;
            this.connection.StatusChanged += Connection_StatusChanged;
            this.connection.Connect();

            return ResponseCode.Success;
        }

        public int DisconnectTerminal()
        {
            this.connection.Disconnect();
            return ResponseCode.Success;
        }

        public void SendData(byte[] data)
        {
            this.connection.SendData(data);
        }

        /// <summary>
        /// 把用户按下的按键按照每种终端的规范转换成终端要发送的ascii码
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected abstract byte[] TranslateKey(PressedKey key);

        public void ProcessInputKey(PressedKey key)
        {
            byte[] translatedByte = this.TranslateKey(key);
            if (translatedByte != null && translatedByte.Length > 0)
            {
                if (!this.connection.SendData(translatedByte))
                {
                    logger.Error("向主机发送数据失败");
                }
            }
        }

        protected abstract void ProcessReceivedData(byte[] data);

        #endregion

        #region 受保护方法

        protected void NotifyCommandReceived(IEnumerable<AbstractTerminalAction> commands, byte[] data)
        {
            if (this.CommandReceived != null)
            {
                this.CommandReceived(this, commands, data);
            }
        }

        #endregion

        #region 事件处理器

        private void Connection_StatusChanged(object sender, TerminalConnectionStatus status)
        {
        }

        private void Connection_DataReceived(object sender, byte[] data)
        {
            this.ProcessReceivedData(data);
            this.NotifyCommandReceived(null, data);
        }

        #endregion
    }
}