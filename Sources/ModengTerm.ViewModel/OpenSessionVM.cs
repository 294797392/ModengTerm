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
        protected override int OnOpen(XTermSession sessionInfo)
        {
            throw new NotImplementedException();
        }

        protected override void OnClose()
        {
            throw new NotImplementedException();
        }
    }
}
