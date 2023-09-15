using ModengTerm.ViewModels;
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
        protected XTermSession session;

        public XTermSession Session
        {
            get { return this.session; }
        }

        public int Open(XTermSession session)
        {
            this.session = session;

            return this.OnOpen(session);
        }

        public void Close()
        {
            this.session = null;

            this.OnClose();
        }

        protected abstract int OnOpen(XTermSession session);
        protected abstract void OnClose();
    }
}
