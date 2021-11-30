using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoTerminal.Sockets
{
    /// <summary>
    /// 实现与串口的通信
    /// </summary>
    public class SerialPortSocket : SocketBase
    {
        public override SocketTypes Protocol
        {
            get
            {
                return SocketTypes.SerialPort;
            }
        }

        public SerialPortSocket(SocketAuthorition authorition) : base(authorition)
        {

        }

        public override bool Connect()
        {
            throw new NotImplementedException();
        }

        public override bool Disconnect()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public override byte Read()
        {
            throw new NotImplementedException();
        }

        public override bool Write(byte data)
        {
            throw new NotImplementedException();
        }

        public override bool Write(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
