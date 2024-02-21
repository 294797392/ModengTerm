using ModengTerm;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using XTerminal.Base.DataModels;

namespace ModengTerm.Controls
{
    public abstract class SessionContent : UserControl
    {
        public OpenedSessionVM ViewModel { get; set; }

        public XTermSession Session { get; set; }

        /// <summary>
        /// 控件被完全Loaded的时候调用
        /// </summary>
        /// <returns></returns>
        public int Open()
        {
            return this.OnOpen(this.ViewModel);
        }

        public void Close()
        {
            this.Session = null;

            this.OnClose();
        }

        protected abstract int OnOpen(OpenedSessionVM sessionVM);
        protected abstract void OnClose();
    }
}
