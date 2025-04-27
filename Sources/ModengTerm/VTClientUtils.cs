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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="parameters">要传递到PanelContentVM里的参数</param>
        /// <param name="matchType"></param>
        /// <returns></returns>
        public static PanelVM PanelDefinition2PanelVM(PanelDefinition panel)
        {
            PanelVM panelVM = new PanelVM();
            panelVM.ID = panel.ID;
            panelVM.Name = panel.Name;

            foreach (PanelItemDefinition panelItem in panel.Items)
            {
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

        public static List<ContextMenuVM> CreateContextMenuVMs(bool toolbarMenu)
        {
            List<MenuItemDefinition> menuItems = new List<MenuItemDefinition>();

            if (toolbarMenu)
            {
                // 先加载所有的根节点菜单
                menuItems.AddRange(MTermApp.Context.Manifest.ToolbarMenus);
            }

            // 再加载所有的插件菜单
            foreach (AddonDefinition addon in MTermApp.Context.Manifest.Addons)
            {
                List<MenuItemDefinition> menus = toolbarMenu ? addon.ToolbarMenus : addon.ContextMenus;

                foreach (MenuItemDefinition menuItem in menus)
                {
                    menuItem.AddonId = addon.ID;
                }

                menuItems.AddRange(menus);
            }

            List<ContextMenuVM> result = new List<ContextMenuVM>();

            foreach (MenuItemDefinition menuItem in menuItems)
            {
                ContextMenuVM cmvm = new ContextMenuVM(menuItem);

                if (string.IsNullOrEmpty(menuItem.ParentId))
                {
                    // 此时说明是根菜单
                    result.Add(cmvm);
                }
                else
                {
                    // 此时说明不是根菜单
                    ContextMenuVM parentVM = result.FirstOrDefault(v => v.ID.ToString() == menuItem.ParentId);
                    parentVM.Children.Add(cmvm);
                }

                // 加载这个菜单的子节点
                LoadChildMenuItems(cmvm, menuItem.Children);
            }

            return result;
        }

        private static void LoadChildMenuItems(ContextMenuVM parentVM, List<MenuItemDefinition> children)
        {
            foreach (MenuItemDefinition menuItem in children)
            {
                menuItem.AddonId = parentVM.AddonId;

                ContextMenuVM contextMenuVM = new ContextMenuVM(menuItem);
                parentVM.Children.Add(contextMenuVM);
            }
        }
    }
}
