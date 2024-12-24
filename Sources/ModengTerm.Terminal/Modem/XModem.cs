using ModengTerm.Base;
using ModengTerm.Terminal.Modules;
using System.IO;

namespace ModengTerm.Terminal.Modem
{
    /// <summary>
    /// https://wiki.synchro.net/ref:ymodem
    /// <SOH><blk #><255-blk #><--128 data bytes--><cksum>
    /// </summary>
    public class XModem : ModemBase
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("XModem");

        private const byte SOH = 0x01;  // Start of Header
        private const byte EOT = 0x04;  // End of Transmission
        private const byte ACK = 0x06;  // Acknowledge
        private const byte NAK = 0x15;  // Negative Acknowledge
        private const byte CAN = 0x18;  // Cancel
        private const byte STX = 0x02;
        private const int PacketSize = 128; // Standard XModem packet size

        #endregion

        #region 实例变量

        private SynchronizedStream stream;

        /// <summary>
        /// 是否使用xmodel1k协议
        /// xmodem1k协议数据包大小是1024
        /// </summary>
        private bool xmodem1k = false;

        /// <summary>
        /// 当数据包传输失败，重传次数
        /// </summary>
        private int retry = 10;

        /// <summary>
        /// 如果数据包不足1024或者128字节，指定填充字符
        /// </summary>
        private byte padchar = (byte)'\0';

        #endregion

        #region 属性

        public override VTModuleTypes Type => VTModuleTypes.XModem;

        #endregion

        #region VTModuleBase

        protected override void OnStart()
        {
            this.stream = new SynchronizedStream()
            {
                ReadTimeout = 50000
            };

            this.Status = VTModuleStatus.Started;
        }

        protected override void OnStop()
        {
            this.stream.Close();
        }

        public override void OnDataReceived(byte[] bytes, int size)
        {
            SynchronizedStream _stream = this.stream;

            if (_stream == null)
            {
                return;
            }

            //logger.InfoFormat("DataReceived, {0}, {1}", bytes[0], size);

            _stream.Write(bytes, size);
        }

        #endregion

        #region ModemBase

        /// <summary>
        /// 1. 用户执行rx
        /// 2. 点击发送文件按钮
        /// 3. 运行Send
        /// </summary>
        /// <returns></returns>
        public override int Send(Stream stream)
        {
            byte[] onebyte = new byte[1];

            int n = this.stream.Read(onebyte, 0, onebyte.Length);

            if (n <= 0)
            {
                logger.ErrorFormat("onebyte read failed, {0}", n);
                return ResponseCode.FAILED;
            }

            #region 使用第一个字节判断数据校验方式        

            bool checksum = false;

            if (onebyte[0] == 'C')
            {
                // 使用CRC16方式传输
                checksum = false;
            }
            else if (onebyte[0] == NAK)
            {
                // 使用校验和方式传输
                checksum = true;
            }
            else
            {
                logger.ErrorFormat("xmodem unhandled start byte, {0}", onebyte[0]);
                return ResponseCode.FAILED;
            }

            logger.InfoFormat("xmodem start byte:{0}", onebyte[0]);

            #endregion

            int datasize = this.xmodem1k ? 1024 : 128;
            byte[] buffer = new byte[datasize];
            byte packetnum = 1;
            bool end = false;

            while (true)
            {
                // data
                n = stream.Read(buffer, 0, datasize);
                if (n == 0)
                {
                    end = true;
                }
                else
                {
                    end = n < datasize;

                    #region 传输数据包

                    bool success = false;
                    byte[] packet = ModemUtils.CreatePacket(buffer, n, this.padchar, packetnum, checksum);

                    for (int i = 0; i < this.retry; i++)
                    {
                        if ((n = this.SendToHost(packet)) != ResponseCode.SUCCESS)
                        {
                            logger.ErrorFormat("XModemWritePacket失败, {0}", n);
                            return n;
                        }

                        if ((n = this.stream.Read(onebyte, 0, onebyte.Length)) <= 0)
                        {
                            logger.ErrorFormat("read packet ack failed, {0}", n);
                            return ResponseCode.FAILED;
                        }

                        byte b = onebyte[0];

                        if (b == ACK)
                        {
                            logger.InfoFormat("xmodem ACK signal, {0}", packetnum);

                            // ACK表示传输成功，递增包序号
                            if (packetnum == 255)
                            {
                                packetnum = 1;
                            }
                            else
                            {
                                packetnum++;
                            }

                            success = true;
                            break;
                        }
                        else if (b == NAK)
                        {
                            // 重传，包序号不变
                            logger.InfoFormat("xmodem NAK signal");
                        }
                        else if (b == 'C')
                        {
                            // 重传
                            logger.InfoFormat("xmodem C siganl");
                        }
                        else if (b == CAN)
                        {
                            logger.InfoFormat("xmodem CAN signal");
                            return ResponseCode.OPERATION_CANCEL;
                        }
                        else
                        {
                            // ...
                            logger.ErrorFormat("xmodem unhandle signal, {0}", onebyte[0]);
                            return ResponseCode.FAILED;
                        }
                    }

                    if (!success)
                    {
                        logger.ErrorFormat("send xmodem packet failed");
                        return ResponseCode.FAILED;
                    }

                    #endregion
                }

                if (end)
                {
                    if ((n = this.SendToHost(new byte[1] { EOT })) != ResponseCode.SUCCESS)
                    {
                        logger.ErrorFormat("xmodem EOT failed, {0}", n);
                        return ResponseCode.FAILED;
                    }

                    if ((n = this.stream.Read(onebyte, 0, onebyte.Length)) == 0)
                    {
                        logger.ErrorFormat("xmodem read ack failed");
                        return ResponseCode.FAILED;
                    }

                    if (onebyte[0] != ACK)
                    {
                        logger.ErrorFormat("xmodem eot response failed, {0}", onebyte[0]);
                        return ResponseCode.FAILED;
                    }

                    logger.InfoFormat("xmodem success");

                    return ResponseCode.SUCCESS;
                }
            }
        }

        public override int Receive()
        {
            return ResponseCode.SUCCESS;
        }

        #endregion

        #region 实例方法

        #endregion
    }
}
