using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels
{
    public class VTMenuGroupVM : ViewModelBase
    {
        public BindableCollection<VTMenuItemVM> MenuItems { get; set; }

        public VTMenuGroupVM()
        {
        }
    }
}
