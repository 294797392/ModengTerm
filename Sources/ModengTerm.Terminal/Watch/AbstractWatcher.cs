using ModengTerm.Base.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Watch
{
    public abstract class AbstractWatcher
    {
        public AbstractWatcher(XTermSession session) { }

        public abstract SystemInfo GetSystemInfo();

        public abstract void GetDisks();

        public abstract void GetNetworkInterfaces();

        //public abstract void GetProcesses();
    }
}
