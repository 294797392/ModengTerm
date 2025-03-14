using ModengTerm.Base.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.ViewModels
{
    public static class DefaultMenuItems
    {
        public static MenuItemDefinition ResourceManagerMenuItem = new MenuItemDefinition()
        {
            ID = Guid.NewGuid().ToString(),
            Name = "资源管理器",
            Ordinal = -1,
            PanelAlign = (int)PanelAlignEnum.Left,
            PanelEntry = "ModengTerm.UserControls.ResourceManagerUserControl, ModengTerm",
            PanelVMEntry = "ModengTerm.ViewModels.ResourceManagerVM, ModengTerm"
        };
    }
}