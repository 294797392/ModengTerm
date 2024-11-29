using ModengTerm.Base.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Watch
{
    public abstract class UnixWatcher : AbstractWatcher
    {
        protected XTermSession session;

        public UnixWatcher(XTermSession session) :
            base(session)
        {
            this.session = session;
        }

        public override SystemInfo GetSystemInfo()
        {
            string meminfo = this.proc_meminfo();

            return new SystemInfo();
        }

        public override void GetDisks()
        {
            throw new NotImplementedException();
        }

        public override void GetNetworkInterfaces()
        {
            throw new NotImplementedException();
        }

        public abstract string proc_meminfo();
        public abstract string proc_stat();
        public abstract string df_h();
    }
}
