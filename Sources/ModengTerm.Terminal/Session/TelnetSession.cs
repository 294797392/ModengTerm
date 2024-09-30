using ModengTerm.Base.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Session
{
    public class TelnetSession : SessionDriver
    {
        public TelnetSession(XTermSession session) : 
            base(session)
        {
        }

        public override int Open()
        {
            throw new NotImplementedException();
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }

        public override void Resize(int row, int col)
        {
            throw new NotImplementedException();
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
