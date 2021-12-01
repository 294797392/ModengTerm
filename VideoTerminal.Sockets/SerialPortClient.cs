using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoTerminal.Clients
{
    /// <summary>
    /// 实现与串口的通信
    /// </summary>
    public class SerialPortClient : ClientBase
    {
        public override ClientTypes Protocol
        {
            get
            {
                return ClientTypes.SerialPort;
            }
        }

        public SerialPortClient(ClientAuthorition authorition) : base(authorition)
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
