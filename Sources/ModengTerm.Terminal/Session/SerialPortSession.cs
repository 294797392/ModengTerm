using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base;
using XTerminal.Base.DataModels;
using XTerminal.Base.Enumerations;

namespace ModengTerm.Terminal.Session
{
    public class SerialPortSession : SessionDriver
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

        #region SessionDriver

        public override int Open()
        {
            string portName = this.session.GetOption<string>(OptionKeyEnum.SERIAL_PORT_NAME);
            int baudRate = this.session.GetOption<int>(OptionKeyEnum.SERIAL_PORT_BAUD_RATE);
            int dataBits = this.session.GetOption<int>(OptionKeyEnum.SERIAL_PORT_DATA_BITS);
            StopBits stopBits = this.session.GetOption<StopBits>(OptionKeyEnum.SERIAL_PORT_STOP_BITS);
            Parity parity = this.session.GetOption<Parity>(OptionKeyEnum.SERIAL_PORT_PARITY);
            Handshake handshake = this.session.GetOption<Handshake>(OptionKeyEnum.SERIAL_PORT_HANDSHAKE);

            try
            {
                this.serialPort = new SerialPort(portName);
                this.serialPort.BaudRate = baudRate;
                this.serialPort.DataBits = dataBits;
                this.serialPort.StopBits = stopBits;
                this.serialPort.Parity = parity;
                this.serialPort.Handshake = handshake;
                this.serialPort.Open();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("串口打开失败, {0}, {1}", portName, ex);
                return ResponseCode.FAILED;
            }

            return ResponseCode.SUCCESS;
        }

        public override void Close()
        {
            this.serialPort.Close();
            this.serialPort.Dispose();
        }

        public override int Write(byte[] bytes)
        {
            this.serialPort.Write(bytes, 0, bytes.Length);
            return ResponseCode.SUCCESS;
        }

        internal override int Read(byte[] buffer)
        {
            return this.serialPort.Read(buffer, 0, buffer.Length);
        }

        public override void Resize(int row, int col)
        {
        }

        #endregion

        #region 事件处理器

        #endregion
    }
}
