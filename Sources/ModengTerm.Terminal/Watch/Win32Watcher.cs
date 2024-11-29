using ModengTerm.Base.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Watch
{
    public class Win32Watcher : AbstractWatcher
    {
        public Win32Watcher(XTermSession session) :
            base(session)
        {
        }

        public override void GetDisks()
        {
            throw new NotImplementedException();
        }

        public override void GetNetworkInterfaces()
        {
            throw new NotImplementedException();
        }

        public override SystemInfo GetSystemInfo()
        {
            return new SystemInfo();
        }
    }
}
