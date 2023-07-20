using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base;
using XTerminal.Session.Property;

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

        public SerialPortSession(VTInitialOptions options) :
            base(options)
        {
        }

        #endregion

        #region SessionBase

        public override int Connect()
        {
            SessionProperties sessionProperties = this.options.SessionProperties;

            string portName = sessionProperties.ServerAddress;

            try
            {
                this.serialPort = new SerialPort(portName);
                this.serialPort.BaudRate = sessionProperties.BaudRate;
                this.serialPort.DataReceived += SerialPort_DataReceived;
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

        public override void Disconnect()
        {
            this.serialPort.DataReceived -= this.SerialPort_DataReceived;
            this.serialPort.Close();
            this.serialPort.Dispose();
        }

        public override int Input(VTInputEvent ievt)
        {
            throw new NotImplementedException();
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

        #endregion

        #region 事件处理器

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int bytesToRead = this.serialPort.BytesToRead;
            byte[] buffer = new byte[bytesToRead];
            this.serialPort.Read(buffer, 0, buffer.Length);
            this.NotifyDataReceived(buffer);
        }

        #endregion
    }
}
