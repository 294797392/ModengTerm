using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace XTerminal.ViewModels
{
    public class GroupVM : ViewModelBase
    {
        public BindableCollection<SessionVM> SessionList { get; private set; }

        public GroupVM()
        {
            this.SessionList = new BindableCollection<SessionVM>();
        }
    }
}
