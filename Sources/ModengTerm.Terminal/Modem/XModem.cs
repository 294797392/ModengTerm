using DotNEToolkit;
using ModengTerm.Base;
using ModengTerm.Terminal.Modules;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Modem
{
    /// <summary>
    /// <SOH><blk #><255-blk #><--128 data bytes--><cksum>
    /// </summary>
    public class XModem : ModemBase
    {
        private static class CRC16
        {
            private static readonly ushort[] crcTable = new ushort[256];

            static CRC16()
            {
                // 初始化 CRC 表，使用 CRC-16/CCITT 多项式 (0x1021)
                ushort polynomial = 0x1021;
                for (int i = 0; i < 256; i++)
                {
                    ushort crc = (ushort)i;
                    for (int j = 0; j < 8; j++)
                    {
                        if ((crc & 0x8000) != 0)
                        {
                            crc = (ushort)((crc << 1) ^ polynomial);
                        }
                        else
                        {
                            crc <<= 1;
                        }
                    }
                    crcTable[i] = crc;
                }
            }

            public static ushort Compute(byte[] data, int offset, int size)
            {
                ushort crc = 0xFFFF; // 初始值

                for (int i = offset; i < offset + size; i++)
                {
                    byte b = data[i];
                    crc = (ushort)((crc << 8) ^ crcTable[((crc >> 8) ^ b) & 0xFF]);
                }
                return crc;
            }
        }

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
        private bool xmodem1k = false;

        /// <summary>
        /// 当数据包传输失败，重传次数
        /// </summary>
        private int retry = 3;

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
                logger.ErrorFormat("unkown start byte, {0}", onebyte[0]);
                return ResponseCode.FAILED;
            }

            int datasize = this.xmodem1k ? 1024 : 128;
            byte[] buffer = new byte[datasize];
            byte packetnum = 1;
            bool eof = false;
            while (true)
            {
                // data
                n = stream.Read(buffer, 0, datasize);
                if (n == 0)
                {
                    eof = true;
                }
                else
                {
                    #region 如果数据不足128或者1024，那么使用0x1A填充

                    if (n < datasize)
                    {
                        // 数据不足，用0x1A填充
                        int npad = datasize - n;
                        byte[] padding = Enumerable.Repeat<byte>(0x1A, npad).ToArray();
                        Buffer.BlockCopy(padding, 0, buffer, n, npad);
                        eof = true;
                    }

                    #endregion

                    #region 传输数据包

                    bool success = false;

                    for (int i = 0; i < this.retry; i++)
                    {
                        int code = this.WritePacket(buffer, packetnum, this.xmodem1k, checksum);
                        if (code != ResponseCode.SUCCESS)
                        {
                            logger.ErrorFormat("XModemWritePacket失败, {0}", code);
                            return ResponseCode.FAILED;
                        }

                        n = this.stream.Read(onebyte, 0, onebyte.Length);
                        if (n <= 0)
                        {
                            logger.ErrorFormat("read packet ack failed, {0}", n);
                            return ResponseCode.FAILED;
                        }

                        if (onebyte[0] == ACK)
                        {
                            logger.InfoFormat("xmodem transfer success, {0}", packetnum);

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
                        else if (onebyte[0] == NAK)
                        {
                            // 重传，包序号不变
                            logger.InfoFormat("xmodem nak signal");
                        }
                        else if (onebyte[0] == CAN)
                        {
                            logger.InfoFormat("xmodem cancel signal");
                        }
                        else
                        {
                            // ...
                            logger.ErrorFormat("xmodem unhandl signal, {0}", onebyte[0]);
                            return ResponseCode.FAILED;
                        }
                    }

                    if (!success)
                    {
                        logger.ErrorFormat("传输XModem数据包失败");
                        return ResponseCode.FAILED;
                    }

                    #endregion
                }

                if (eof)
                {
                    n = this.Write(new byte[1] { EOT });
                    if (n != ResponseCode.SUCCESS)
                    {
                        logger.ErrorFormat("xmodem EOT failed, {0}", n);
                        return ResponseCode.FAILED;
                    }

                    n = this.stream.Read(onebyte, 0, onebyte.Length);
                    if (n == 0) 
                    {
                        logger.ErrorFormat("xmodem read ack failed");
                        return ResponseCode.FAILED;
                    }

                    if (onebyte[0] == ACK)
                    {
                        logger.InfoFormat("xmodem success");
                        return ResponseCode.SUCCESS;
                    }

                    logger.ErrorFormat("xmodem eot response failed, {0}", onebyte[0]);

                    return ResponseCode.FAILED;
                }
            }
        }

        public override int Receive()
        {
            return ResponseCode.SUCCESS;
        }

        #endregion

        #region 实例方法

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">要传输的数据</param>
        /// <param name="packetnum">包序号</param>
        /// <param name="xmodem1k">是否使用XModem1k协议</param>
        /// <param name="checksum">是否使用checksum做完整性校验</param>
        /// <param name="eof">是否到达了文件末尾</param>
        /// <returns></returns>
        private int WritePacket(byte[] data, byte packetnum, bool xmodem1k, bool checksum)
        {
            // 计算一个数据包的总大小
            int datasize = data.Length;
            int pktsize = checksum ? 3 + datasize + 1 : 3 + datasize + 2;

            byte[] packet = new byte[pktsize];

            // header
            packet[0] = xmodem1k ? STX : SOH;
            packet[1] = packetnum; // TODO：包序号255怎么办？从0开始？
            packet[2] = (byte)(255 - packetnum);

            // data
            Buffer.BlockCopy(data, 0, packet, 3, datasize);

            // checksum或者crc16
            if (checksum)
            {
                // 用checksum校验
                byte cksum = 0;
                for (int i = 3; i < packet.Length - 1; i++)
                {
                    cksum += packet[i];
                }
                packet[packet.Length - 1] = cksum;
            }
            else
            {
                // 用crc16校验
                ushort crc = CRC16.Compute(packet, 3, datasize);
                packet[packet.Length - 2] = (byte)(crc & 0xFF);
                packet[packet.Length - 1] = (byte)(crc >> 8);
            }

            return base.Write(packet);
        }

        #endregion
    }
}
