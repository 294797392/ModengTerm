using DotNEToolkit.Crypto;
using ModengTerm.Base;
using ModengTerm.Base.Enumerations;
using System;
using System.IO;
using System.Xml.Serialization;

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

        #endregion

        #region 实例变量

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

        #endregion

        #region ModemBase

        /// <summary>
        /// 1. 用户执行sx
        /// 2. 点击XModel接收文件按钮
        /// 3. 运行Receive
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public override int Receive(List<string> filePaths)
        {
            FileStream stream = ModemUtils.OpenFile(filePaths[0], SendReceive.Receive);
            if (stream == null) 
            {
                return ResponseCode.FAILED;
            }

            bool xmodem1k = this.Session.GetOption<bool>(OptionKeyEnum.XMODEM_XMODEM1K, MTermConsts.XMODEM_XMODEM1K);
            int retry = this.Session.GetOption<int>(OptionKeyEnum.MODEM_RETRY_TIMES, MTermConsts.MODEM_RETRY_TIMES);
            bool crc = this.Session.GetOption<bool>(OptionKeyEnum.XMODEM_RECV_CRC, MTermConsts.XMODEM_RECV_CRC);
            byte padchar = this.Session.GetOption<byte>(OptionKeyEnum.XMODEM_RECV_PADCHAR, MTermConsts.XMODEM_RECV_PADCHAR);
            bool ignorePadChar = this.Session.GetOption<bool>(OptionKeyEnum.XMODEM_RECV_IGNORE_PADCHAR, MTermConsts.XMODEM_RECV_IGNORE_PADCHAR);
            byte[] onebyte = new byte[1];
            int rc = 0;

            byte firstbyte = 0;
            if ((rc = this.Handshake(out firstbyte)) != ResponseCode.SUCCESS)
            {
                logger.ErrorFormat("xmodem接收握手失败, {0}", rc);
                return rc;
            }

            while (true)
            {
                #region 判断接下来的处理方式

                int packetsize = 0;

                if (firstbyte == SOH)
                {
                    // 128字节数据
                    packetsize = 128;
                }
                else if (firstbyte == STX)
                {
                    // 1024字节数据
                    packetsize = 1024;
                }
                else if (firstbyte == EOT)
                {
                    // 文件发送结束
                    logger.InfoFormat("xmodel发送成功");

                    // 在发送ACK之前触发完成事件，这样不会导致ShellSessionVM.ProcessData丢失数据
                    this.NotifyProgressChanged(100, ResponseCode.SUCCESS);
                    return this.HostStream.Send(new byte[] { ACK });
                }
                else
                {
                    // 未知数据
                    logger.ErrorFormat("xmodem接收到未知数据, {0}", firstbyte);
                    return ResponseCode.FAILED;
                }

                #endregion

                #region 接收数据包

                // 该帧一共多少字节
                int blocksize = 3 + packetsize + (crc ? 2 : 1);

                // 一旦开始接收一个数据块，接收方会对每个字符和校验和设置 1 秒的超时。这意味着如果在 1 秒内没有接收到下一个字符或校验和，接收方将认为发生了超时。
                byte[] datablock = new byte[blocksize];
                datablock[0] = firstbyte;

                for (int i = 1; i < datablock.Length; i++)
                {
                    rc = this.HostStream.Receive(datablock, i, 1, 1000);
                    if (rc == 0)
                    {
                        // 接收超时，直接取消
                        logger.ErrorFormat("xmodem接收数据包超时，发送CAN信号");
                        this.HostStream.Send(new byte[] { CAN });
                        return ResponseCode.FAILED;
                    }
                    else if (rc < 0)
                    {
                        // 接收失败
                        logger.ErrorFormat("xmodem接收数据包失败");
                        return ResponseCode.FAILED;
                    }
                    else
                    {
                        // 接收成功
                    }
                }

                #endregion

                #region 处理数据包

                bool error = false;

                // 1. 判断检验和
                if (crc)
                {
                    byte[] crcdata = ModemUtils.CalculateCRC(datablock);
                    if (datablock[datablock.Length - 1] != crcdata[0] || datablock[datablock.Length - 2] != crcdata[1])
                    {
                        error = true;
                    }
                }
                else
                {
                    byte chksum = ModemUtils.Checksum(datablock);
                    if (chksum != datablock[datablock.Length - 1])
                    {
                        error = true;
                    }
                }

                if (error)
                {
                    // 发送重传信号
                    logger.InfoFormat("xmodem校验和错误, 重传");
                    this.HostStream.Send(new byte[] { NAK });
                }
                else
                {
                    // 2. 判断填充字符
                    byte packetnum = datablock[1];
                    int dataoffset = 3;
                    int datasize = packetsize;

                    if (ignorePadChar)
                    {
                        // 忽略填充字符
                    }
                    else
                    {
                        // 查找填充字符
                        // TODO：优化不要每次都查找padchar，在收到EOT的时候判断上一次的padchar位置
                        int len = crc ? 2 : 1;
                        for (int i = 3; i < datablock.Length - len; i++)
                        {
                            if (datablock[i] == padchar)
                            {
                                datasize = i - 2;
                                dataoffset = i;
                                break;
                            }
                        }
                    }

                    // 3. 写入文件
                    stream.Write(datablock, dataoffset, datasize);
                    logger.InfoFormat("xmodem receive, packetnum = {0}", packetnum);

                    // 4. 发送ACK
                    this.HostStream.Send(new byte[] { ACK });
                }

                #endregion

                #region 接收下一个数据包的头部

                rc = this.HostStream.Receive(onebyte, 1000);
                if (rc == 0)
                {
                    logger.ErrorFormat("xmodem接收数据包超时，发送CAN信号");
                    this.HostStream.Send(new byte[] { CAN });
                    return ResponseCode.FAILED;
                }
                else if (rc < 0)
                {
                    // 接收失败
                    logger.ErrorFormat("xmodem接收数据包失败");
                    return ResponseCode.FAILED;
                }
                else
                {
                    // 接收成功
                    firstbyte = onebyte[0];
                }

                #endregion
            }
        }

        /// <summary>
        /// 1. 用户执行rx
        /// 2. 点击XModel发送文件按钮
        /// 3. 运行Send
        /// </summary>
        /// <returns></returns>
        public override int Send(List<string> filePaths)
        {
            FileStream stream = ModemUtils.OpenFile(filePaths[0], SendReceive.Send);
            if (stream == null) 
            {
                return ResponseCode.FAILED;
            }

            this.xmodem1k = this.Session.GetOption<bool>(OptionKeyEnum.XMODEM_XMODEM1K, MTermConsts.XMODEM_XMODEM1K);
            this.retry = this.Session.GetOption<int>(OptionKeyEnum.MODEM_RETRY_TIMES, MTermConsts.MODEM_RETRY_TIMES);

            byte[] onebyte = new byte[1];

            int n = this.HostStream.Receive(onebyte);

            if (n <= 0)
            {
                logger.ErrorFormat("startbyte read failed, {0}", n);
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
                        if ((n = this.HostStream.Send(packet)) != ResponseCode.SUCCESS)
                        {
                            logger.ErrorFormat("XModemWritePacket失败, {0}", n);
                            return n;
                        }

                        if ((n = this.HostStream.Receive(onebyte)) <= 0)
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
                    break;
                }
            }

            if ((n = this.HostStream.Send(new byte[1] { EOT })) != ResponseCode.SUCCESS)
            {
                logger.ErrorFormat("xmodem EOT failed, {0}", n);
                return ResponseCode.FAILED;
            }

            if ((n = this.HostStream.Receive(onebyte)) == 0)
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

            this.NotifyProgressChanged(100, ResponseCode.SUCCESS);

            return ResponseCode.SUCCESS;
        }

        #endregion

        #region 实例方法

        /// <summary>
        /// 接收方有一个 10 秒的超时机制。每次超时后，接收方会发送一个 <NAK>（否定确认）。
        /// 接收方的第一次超时并发送 <NAK> 的行为会向发送方发出开始传输的信号。
        /// 可选地，如果发送方已经准备好，接收方可以立即发送一个 <NAK>，这将节省最初的 10 秒超时时间。
        /// 然而，接收方必须继续每 10 秒超时一次，以确保发送方确实没有准备好。
        /// </summary>
        private int Handshake(out byte b)
        {
            b = 0;
            byte[] onebyte = new byte[1];

            int rc = 0;
            bool crc = this.Session.GetOption<bool>(OptionKeyEnum.XMODEM_RECV_CRC, MTermConsts.XMODEM_RECV_CRC);
            byte[] startbyte = new byte[1];
            startbyte[0] = crc ? (byte)'C' : NAK;

            for (int i = 0; i < retry; i++)
            {
                if ((rc = this.HostStream.Send(startbyte)) != ResponseCode.SUCCESS)
                {
                    return ResponseCode.FAILED;
                }

                rc = this.HostStream.Receive(onebyte, 10000);
                if (rc == 0)
                {
                    // 超时
                    continue;
                }
                else if (rc < 0)
                {
                    // 被关闭了
                    return ResponseCode.FAILED;
                }
                else
                {
                    b = onebyte[0];

                    if (b != STX && b != SOH)
                    {
                        logger.ErrorFormat("xmodem接收到未知握手信号, {0}", b);
                        continue;
                    }

                    return ResponseCode.SUCCESS;
                }
            }

            return ResponseCode.FAILED;
        }

        #endregion
    }
}
