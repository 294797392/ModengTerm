using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels
{
    public class ToolPanelItemVM : MenuItemVM
    {
        public ToolPanelItemVM(MenuDefinition menu) :
            base(menu)
        {
        }
    }

    public class ToolPanelVM : MenuVM
    {
    }
}
