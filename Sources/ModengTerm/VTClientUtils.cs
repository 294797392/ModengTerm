using ModengTerm.Base;
using ModengTerm.Base.Definitions;
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
    }
}
