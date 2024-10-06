using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.Terminal.DataModels
{
    public class SyncSlave : ViewModelBase
    {
        public string MasterSessionId { get; set; }
    }
}
