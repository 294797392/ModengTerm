using ModengTerm.Addons.Shell;
using ModengTerm.Base.Addon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.Addon.Official.BroadcastInput
{
    public class BroadcastSessionVM : ViewModelBase
    {
        public IShellPanel Session { get; set; }
    }
}
