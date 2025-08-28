using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using System.IO.Ports;

namespace ModengTerm.Terminal.Engines
{
    public class SerialPortChannel : ChannelBase
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("SerialPortSession");

        #endregion

        #region 实例变量

        private SerialPort serialPort;

        #endregion

        #region 构造方法

        public SerialPortChannel()
        {
        }

        #endregion

        #region ChannelBase

        public override int Open()
        {
            SerialPortChannelOptions channelOptions = this.options as SerialPortChannelOptions;

            string portName = channelOptions.PortName;
            int baudRate = channelOptions.BaudRate;
            int dataBits = channelOptions.DataBits;
            StopBits stopBits = channelOptions.StopBits;
            Parity parity = channelOptions.Parity;
            Handshake handshake = channelOptions.Handshake;

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

        public override void Write(byte[] bytes)
        {
            this.serialPort.Write(bytes, 0, bytes.Length);
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
