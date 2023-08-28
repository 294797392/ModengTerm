using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base.DataModels;

namespace XTerminal.ViewModels
{
    public class OpenSessionVM : OpenedSessionVM
    {
        public override void Close()
        {
            throw new NotImplementedException();
        }

        public override int Open(XTermSession session)
        {
            throw new NotImplementedException();
        }
    }
}
