using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels
{
    public class VTMenuListVM : ViewModelBase
    {
        public BindableCollection<VTMenuGroupVM> MenuGroups { get; set; }

        public VTMenuListVM()
        {
        }
    }
}
