using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XTerminal.Connections;

namespace XTerminal.Simulators
{
    public abstract class AbstractSimulator : ITerminalSimulator
    {
        #region 事件

        public event Action<object, IEnumerable<AbstractTerminalCommand>> CommandReceived;
        public event Action<object, TerminalConnectionStatus> StatusChanged;

        #endregion

        #region 实例变量

        protected IConnection connection;

        #endregion

        #region 属性

        public ConnectionProtocols Protocols { get; set; }

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

        public abstract void HandleUserInput(string key);

        #endregion

        #region 事件处理器

        private void Connection_StatusChanged(object sender, TerminalConnectionStatus status)
        {
        }

        private void Connection_DataReceived(object sender, byte[] data)
        {
        }

        #endregion
    }
}