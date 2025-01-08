using ModengTerm.Base;
using ModengTerm.Base.Definitions;
using ModengTerm.DataModels;
using ModengTerm.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using WPFToolkit.MVVM;

namespace ModengTerm
{
    public static class VTClientUtils
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTClientUtils");

        public static List<ContextMenuVM> CreateContextMenuVM(List<MenuItemRelation> menuItems)
        {
            List<ContextMenuVM> results = new List<ContextMenuVM>();

            // MenuId -> ContetMenuVM
            Dictionary<string, ContextMenuVM> menuCaches = new Dictionary<string, ContextMenuVM>();

            foreach (ContextMenuDefinition menuDefinition in menuItems.Select(v => v.MenuDefinition))
            {
                ContextMenuVM contextMenuVM = new ContextMenuVM(menuDefinition);
                menuCaches.Add(menuDefinition.ID, contextMenuVM);
            }

            foreach (MenuItemRelation menuItem in menuItems)
            {
                ContextMenuDefinition definition = menuItem.MenuDefinition;
                string menuId = definition.ID;
                string parentID = menuItem.ParentID;

                if (parentID == "-1")
                {
                    continue;
                }

                ContextMenuVM contextMenuVM = menuCaches[menuId];

                if (string.IsNullOrWhiteSpace(parentID))
                {
                    // 根节点
                    results.Add(contextMenuVM);
                }
                else
                {
                    // 子节点
                    ContextMenuVM parentVM;
                    if (menuCaches.TryGetValue(parentID, out parentVM))
                    {
                        parentVM.Children.Add(contextMenuVM);
                    }
                }
            }

            return results;
        }

        public static Dictionary<PanelAlignEnum, PanelVM> CreatePanels(List<ContextMenuDefinition> menuDefinitions, Dictionary<string, object> parameters = null)
        {
            Dictionary<PanelAlignEnum, PanelVM> panels = new Dictionary<PanelAlignEnum, PanelVM>();

            List<PanelAlignEnum> panelAligns = VTBaseUtils.GetEnumValues<PanelAlignEnum>();
            foreach (PanelAlignEnum panelAlign in panelAligns)
            {
                PanelVM panelVM = new PanelVM();
                panelVM.ID = Guid.NewGuid().ToString();
                panels[panelAlign] = panelVM;
            }

            foreach (ContextMenuDefinition menuDefinition in menuDefinitions)
            {
                PanelVM panelVM = panels[(PanelAlignEnum)menuDefinition.PanelAlign];

                PanelItemVM panelItemVM = new PanelItemVM()
                {
                    ID = menuDefinition.ID,
                    Name = menuDefinition.Name,
                    VMClassName = menuDefinition.PanelVMEntry,
                    ClassName = menuDefinition.PanelEntry
                };

                if (parameters != null)
                {
                    foreach (KeyValuePair<string, object> kv in parameters)
                    {
                        panelItemVM.Parameters[kv.Key] = kv.Value;
                    }
                }

                panelVM.AddMenuItem(panelItemVM);
            }

            return panels;
        }
    }
}
