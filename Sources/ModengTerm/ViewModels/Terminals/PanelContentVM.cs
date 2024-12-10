using ModengTerm.Base.DataModels;
using ModengTerm.Base.ServiceAgents;
using ModengTerm.Terminal.Watch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels.Terminals
{
    /// <summary>
    /// 所有PanelContent的基类
    /// </summary>
    public abstract class PanelContentVM : MenuContentVM
    {
        public const string KEY_SERVICE_AGENT = "serviceAgent";
        public const string KEY_XTERM_SESSION = "xtermSession";

        protected ServiceAgent ServiceAgent { get; private set; }
        protected XTermSession Session { get; private set; }

        public override void OnInitialize()
        {
            this.ServiceAgent = this.Parameters[KEY_SERVICE_AGENT] as ServiceAgent;
            this.Session = this.Parameters[KEY_XTERM_SESSION] as XTermSession;
        }

        public override void OnRelease()
        {
        }

        public override void OnLoaded()
        {
        }

        public override void OnUnload()
        {
        }
    }
}
