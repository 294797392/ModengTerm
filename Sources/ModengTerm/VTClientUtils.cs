using ModengTerm.Base;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using ModengTerm.DataModels;
using ModengTerm.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

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

            foreach (MenuItemDefinition menuDefinition in menuItems.Select(v => v.MenuDefinition))
            {
                ContextMenuVM contextMenuVM = new ContextMenuVM(menuDefinition);
                menuCaches.Add(menuDefinition.ID, contextMenuVM);
            }

            foreach (MenuItemRelation menuItem in menuItems)
            {
                MenuItemDefinition definition = menuItem.MenuDefinition;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="parameters">要传递到PanelContentVM里的参数</param>
        /// <param name="matchType"></param>
        /// <returns></returns>
        public static PanelVM PanelDefinition2PanelVM(PanelDefinition panel, int? matchType = null)
        {
            PanelVM panelVM = new PanelVM();
            panelVM.ID = panel.ID;
            panelVM.Name = panel.Name;
            panelVM.Dock = (SideWindowDock)panel.Dock;

            foreach (PanelItemDefinition panelItem in panel.Items)
            {
                // 过滤会话类型
                if (panelItem.SessionTypes.Count > 0 && matchType != null)
                {
                    if (!panelItem.SessionTypes.Contains(matchType.Value))
                    {
                        continue;
                    }
                }

                PanelItemVM panelItemVM = new PanelItemVM(panelItem);
                panelItemVM.ID = panelItem.ID;
                panelItemVM.Name = panelItem.Name;
                panelItemVM.IconURI = panelItem.Icon;
                panelItemVM.ClassName = panelItem.ClassName;
                panelItemVM.VMClassName = panelItem.VMClassName;
                panelVM.AddMenuItem(panelItemVM);
            }

            return panelVM;
        }
    }
}
