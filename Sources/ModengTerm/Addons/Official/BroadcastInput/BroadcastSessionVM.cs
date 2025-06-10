using ModengTerm.Addons.Shell;
using ModengTerm.Base.Addon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.Addons.BroadcastInput
{
    public class BroadcastSessionVM : ViewModelBase
    {
        public ITerminalShell Session { get; set; }
    }
}
