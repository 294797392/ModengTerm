using ModengTerm.Terminal.ViewModels;
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
        public IShellSession Session { get; set; }
    }
}
