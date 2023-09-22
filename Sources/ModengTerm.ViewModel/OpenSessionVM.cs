using ModengTerm.Base.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base.DataModels;

namespace ModengTerm.ViewModels
{
    public class OpenSessionVM : OpenedSessionVM
    {
        public OpenSessionVM(XTermSession session) :
            base(session)
        { }

        protected override int OnOpen()
        {
            throw new NotImplementedException();
        }

        protected override void OnClose()
        {
            throw new NotImplementedException();
        }
    }
}
