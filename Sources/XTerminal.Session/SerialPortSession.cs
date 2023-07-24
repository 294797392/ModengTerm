using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base;
using XTerminal.Base.DataModels;
using XTerminal.Base.DataModels.Session;

namespace XTerminal.Session
{
    public class SerialPortSession : SessionBase
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("SerialPortSession");

        #endregion

        #region 实例变量

        private SerialPort serialPort;

        #endregion

        #region 构造方法

        public SerialPortSession(XTermSession options) :
            base(options)
        {
        }

        #endregion

        #region SessionBase

        public override int Open()
        {
            SessionProperties sessionProperties = this.options.SessionProperties;

            string portName = sessionProperties.ServerAddress;

            try
            {
                this.serialPort = new SerialPort(portName);
                this.serialPort.BaudRate = sessionProperties.BaudRate;
                this.serialPort.Open();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("串口打开失败, {0}, {1}", portName, ex);
                return ResponseCode.FAILED;
            }

            this.NotifyStatusChanged(SessionStatusEnum.Connected);

            return ResponseCode.SUCCESS;
        }

        public override void Close()
        {
            this.serialPort.Close();
            this.serialPort.Dispose();
        }

        public override int Write(byte[] bytes)
        {
            try
            {
                this.serialPort.Write(bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                this.NotifyStatusChanged(SessionStatusEnum.Disconnected);
                logger.Error("写入串口异常", ex);
                return ResponseCode.FAILED;
            }

            return ResponseCode.SUCCESS;
        }

        internal override int Read(byte[] buffer)
        {
            return this.serialPort.Read(buffer, 0, buffer.Length);
        }

        #endregion

        #region 事件处理器

        #endregion
    }
}
