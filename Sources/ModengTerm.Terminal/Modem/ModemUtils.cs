using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Modem
{
    public static class ModemUtils
    {
        public class XModemCRC
        {
            // CRC-16 (ITU-T V.41) polynomial: 0x1021
            private const ushort Polynomial = 0x1021;

            /// <summary>
            /// 计算 XModem CRC-16 校验值。
            /// </summary>
            /// <param name="data">要计算 CRC 的字节数组。</param>
            /// <returns>16 位 CRC 校验值。</returns>
            public static ushort CalculateCRC(byte[] data)
            {
                ushort crc = 0x0000;  // 初始值为 0x0000

                foreach (byte b in data)
                {
                    crc ^= (ushort)(b << 8);  // 将字节左移 8 位并与当前 CRC 异或

                    for (int i = 0; i < 8; i++)
                    {
                        if ((crc & 0x8000) != 0)
                        {
                            crc = (ushort)((crc << 1) ^ Polynomial);
                        }
                        else
                        {
                            crc <<= 1;
                        }
                    }
                }

                return crc;  // 返回最终的 CRC 值
            }
        }

        private const byte SOH = 0x01;  // Start of Header
        private const byte EOT = 0x04;  // End of Transmission
        private const byte ACK = 0x06;  // Acknowledge
        private const byte NAK = 0x15;  // Negative Acknowledge
        private const byte CAN = 0x18;  // Cancel
        private const byte STX = 0x02;

        /// <summary>
        /// 创建一个数据包
        /// </summary>
        /// <param name="buffer">存储数据的缓冲区，要么是1024，要么是128</param>
        /// <param name="size">要发送的实际的数据长度</param>
        /// <param name="padchar">如果要发送的实际数据长度比缓冲区长度小，那么使用padchar填充</param>
        /// <param name="packetnum">数据包序号</param>
        /// <param name="checksum">是否使用checksum校验和</param>
        /// <returns></returns>
        public static byte[] CreatePacket(byte[] buffer, int size, byte padchar, byte packetnum, bool checksum)
        {
            // 计算一个数据包的总大小
            int datasize = buffer.Length;
            int pktsize = checksum ? 3 + datasize + 1 : 3 + datasize + 2;
            bool xmodel1k = datasize == 1024;

            if (buffer.Length < datasize)
            {
                // 数据不足，填充
                int npad = datasize - buffer.Length;
                byte[] padding = Enumerable.Repeat<byte>(padchar, npad).ToArray();
                Buffer.BlockCopy(padding, 0, buffer, size, npad);
            }

            byte[] packet = new byte[pktsize];

            // header
            packet[0] = xmodel1k ? STX : SOH;
            packet[1] = packetnum; // TODO：包序号255怎么办？从0开始？
            packet[2] = (byte)(255 - packetnum);

            // data
            Buffer.BlockCopy(buffer, 0, packet, 3, datasize);

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
                ushort crc = XModemCRC.CalculateCRC(buffer);
                byte[] crcbytes = BitConverter.GetBytes(crc);
                packet[packet.Length - 1] = crcbytes[0];
                packet[packet.Length - 2] = crcbytes[1];
            }

            return packet;
        }
    }
}
