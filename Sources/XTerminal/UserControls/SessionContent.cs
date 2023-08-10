using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using XTerminal.Base.DataModels;

namespace XTerminal.UserControls
{
    public abstract class SessionContent : UserControl
    {
        public abstract int Open(XTermSession session);

        public abstract void Close();
    }
}
