using ModengTerm.Terminal.Watch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.ViewModels.Terminals.PanelContent
{
    public class WatchSystemInfo : WatchVM
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("WatchSystemInfo");

        public override void Watch(AbstractWatcher watcher)
        {
            SystemInfo systemInfo = watcher.GetSystemInfo();

            //logger.ErrorFormat(Guid.NewGuid().ToString());
        }
    }
}
