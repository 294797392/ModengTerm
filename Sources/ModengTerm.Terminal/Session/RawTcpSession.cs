using ModengTerm.Base.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Session
{
    public class RawTcpSession : SessionDriver
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("RawTcpSession");

        private TcpClient client;

        public RawTcpSession(XTermSession session) :
            base(session)
        {
        }

        public override int Open()
        {
            this.client = new TcpClient();
            throw new NotImplementedException();
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }

        public override void Resize(int row, int col)
        {
        }

        public override void Write(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        internal override int Read(byte[] buffer)
        {
            throw new NotImplementedException();
        }
    }
}
