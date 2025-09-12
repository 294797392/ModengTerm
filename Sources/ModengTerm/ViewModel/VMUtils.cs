using ModengTerm.Addon;
using ModengTerm.Addon.Service;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.Metadatas;
using ModengTerm.Base.ServiceAgents;
using ModengTerm.ViewModel.Panels;
using ModengTerm.ViewModel.Session;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel
{
    public static class VMUtils
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("VMUtils");

        /// <summary>
        /// 创建一个会话树形列表
        /// </summary>
        /// <param name="onlyGroup">树形列表里是否只有分组</param>
        /// <param name="includeRootNode">是否包含根节点分组（Root）</param>
        public static SessionTreeVM CreateSessionTreeVM(bool onlyGroup = false, bool includeRootNode = false)
        {
            ServiceAgent serviceAgent = ServiceAgentFactory.Get();

            SessionTreeVM sessionTreeVM = new SessionTreeVM();
            TreeViewModelContext context = sessionTreeVM.Context;

            SessionGroupVM rootNode = null;

            if (includeRootNode)
            {
                rootNode = new SessionGroupVM(context, 0, VTBaseConsts.RootGroup);
                sessionTreeVM.AddRootNode(rootNode);
            }

            List<SessionGroup> groups = serviceAgent.GetSessionGroups();
            IEnumerable<SessionGroup> rootGroups = groups.Where(v => v.ParentId == string.Empty);
            foreach (SessionGroup group in rootGroups)
            {
                SessionGroupVM groupVM = new SessionGroupVM(context, 0, group);
                if (rootNode != null)
                {
                    groupVM.Level = 1;
                    rootNode.Add(groupVM);
                }
                else
                {
                    sessionTreeVM.AddRootNode(groupVM);
                }

                LoadSessionGroupNode(groupVM, groups);
            }

            if (!onlyGroup)
            {
                List<XTermSession> sessions = serviceAgent.GetSessions();
                foreach (XTermSession session in sessions)
                {
                    XTermSessionVM sessionVM = new XTermSessionVM(context, 0, session);

                    if (string.IsNullOrEmpty(session.GroupId))
                    {
                        // 如果Session不属于任何分组，那么直接加到根节点
                        if (rootNode != null)
                        {
                            sessionVM.Level = rootNode.Level + 1;
                            rootNode.Add(sessionVM);
                        }
                        else
                        {
                            sessionTreeVM.AddRootNode(sessionVM);
                        }
                    }
                    else
                    {
                        TreeNodeViewModel parentVM;
                        if (!context.TryGetNode(session.GroupId, out parentVM))
                        {
                            logger.FatalFormat("没有找到Session对应的Gorup, {0},{1}", session.ID, session.GroupId);
                            continue;
                        }

                        sessionVM.Level = parentVM.Level + 1;
                        parentVM.Add(sessionVM);
                    }
                }
            }

            return sessionTreeVM;
        }

        /// <summary>
        /// 创建客户端默认的菜单
        /// </summary>
        /// <param name="metadatas">要创建的菜单列表</param>
        /// <returns></returns>
        public static List<MenuItemVM> CreateDefaultMenuItems(List<MenuMetadata> metadatas)
        {
            List<MenuItemVM> result = new List<MenuItemVM>();

            foreach (MenuMetadata metadata in metadatas)
            {
                MenuItemVM mivm = new MenuItemVM(metadata);
                mivm.CommandKey = metadata.Command;
                result.Add(mivm);
                // 加载这个菜单的子节点
                LoadChildMenuItems(mivm, metadata.Children);
            }

            return result;
        }

        /// <summary>
        /// 创建顶部菜单或者右键菜单
        /// </summary>
        /// <param name="toolbarMenu"></param>
        /// <returns></returns>
        public static List<MenuItemVM> CreateAddonMenuItems(bool toolbarMenu)
        {
            List<MenuItemVM> result = new List<MenuItemVM>();

            // 加载所有的插件菜单
            foreach (AddonMetadata addon in ClientContext.Context.Manifest.Addons)
            {
                List<MenuMetadata> menus = toolbarMenu ? addon.ToolbarMenus : addon.ContextMenus;

                foreach (MenuMetadata menuItem in menus)
                {
                    MenuItemVM mivm = new MenuItemVM(menuItem);

                    if (!string.IsNullOrEmpty(menuItem.Command))
                    {
                        mivm.CommandKey = AddonUtils.GetCommandKey(addon.ID, menuItem.Command);
                    }

                    result.Add(mivm);

                    // 加载这个菜单的子节点
                    LoadChildMenuItems(mivm, menuItem.Children, addon);
                }
            }

            return result;
        }

        public static SidePanelVM CreateSidePanelVM(SidePanelMetadata metadata)
        {
            SidePanelVM sidePanel = new SidePanelVM();
            sidePanel.Metadata = metadata;
            sidePanel.ID = metadata.ID;
            sidePanel.Name = metadata.Name;
            sidePanel.IconURI = metadata.Icon;
            sidePanel.Dock = metadata.Dock;
            return sidePanel;
        }

        public static OverlayPanelVM CreateOverlayPanelVM(OverlayPanelMetadata metadata)
        {
            OverlayPanelVM overlayPanel = new OverlayPanelVM();
            overlayPanel.Metadata = metadata;
            overlayPanel.ID = metadata.ID;
            overlayPanel.Name = metadata.Name;
            overlayPanel.IconURI = metadata.Icon;
            overlayPanel.Dock = metadata.Dock;
            return overlayPanel;
        }



        private static void LoadSessionGroupNode(SessionGroupVM parentGroup, List<SessionGroup> groups)
        {
            // 先找到该分组的所有子节点
            IEnumerable<SessionGroup> children = groups.Where(v => v.ParentId == parentGroup.ID.ToString());

            foreach (SessionGroup child in children)
            {
                SessionGroupVM groupVM = new SessionGroupVM(parentGroup.Context, parentGroup.Level + 1, child);
                parentGroup.Add(groupVM);
                LoadSessionGroupNode(groupVM, groups);
            }
        }

        private static void LoadChildMenuItems(MenuItemVM parentVM, List<MenuMetadata> children, AddonMetadata addon)
        {
            foreach (MenuMetadata menuItem in children)
            {
                MenuItemVM mivm = new MenuItemVM(menuItem);

                if (!string.IsNullOrEmpty(menuItem.Command))
                {
                    mivm.CommandKey = AddonUtils.GetCommandKey(addon.ID, menuItem.Command);
                }

                parentVM.Children.Add(mivm);

                LoadChildMenuItems(mivm, menuItem.Children, addon);
            }
        }

        private static void LoadChildMenuItems(MenuItemVM parentVM, List<MenuMetadata> children)
        {
            foreach (MenuMetadata metadata in children)
            {
                MenuItemVM mivm = new MenuItemVM(metadata);
                mivm.CommandKey = metadata.Command;
                parentVM.Children.Add(mivm);
                LoadChildMenuItems(mivm, metadata.Children);
            }
        }
    }
}
