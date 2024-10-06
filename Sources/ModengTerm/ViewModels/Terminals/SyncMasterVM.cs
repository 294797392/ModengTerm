using ModengTerm.Terminal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels.Terminals
{
    public class SyncMasterVM : ViewModelBase
    {
        public BindableCollection<SyncSlaveVM> Slaves { get; private set; }

        public SyncMasterVM() 
        {
            this.Slaves = new BindableCollection<SyncSlaveVM>();
        }
    }
}
