using ModengTerm.Addon.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.OfficialAddons.BroadcastInput
{
    public class BroadcastSessionVM : ViewModelBase
    {
        public IClientShellTab BroadcasePanel { get; set; }
    }
}
