using DotNEToolkit;
using ModengTerm.Addon.Interactive;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.ServiceAgents;
using ModengTerm.Document;
using ModengTerm.ViewModel.Panels;
using ModengTerm.ViewModel.Session;
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
            ServiceAgent serviceAgent = VTApp.Context.ServiceAgent;

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

        public static List<ContextMenuVM> CreateContextMenuVMs(bool toolbarMenu)
        {
            List<AddonMenuDefinition> menuItems = new List<AddonMenuDefinition>();

            if (toolbarMenu)
            {
                // 先加载所有的根节点菜单
                menuItems.AddRange(VTApp.Context.Manifest.ToolbarMenus);
            }

            // 再加载所有的插件菜单
            foreach (AddonMetadata addon in VTApp.Context.Manifest.Addons)
            {
                List<AddonMenuDefinition> menus = toolbarMenu ? addon.ToolbarMenus : addon.ContextMenus;

                foreach (AddonMenuDefinition menuItem in menus)
                {
                    menuItem.AddonId = addon.ID;
                }

                menuItems.AddRange(menus);
            }

            List<ContextMenuVM> result = new List<ContextMenuVM>();

            foreach (AddonMenuDefinition menuItem in menuItems)
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

        private static void LoadChildMenuItems(ContextMenuVM parentVM, List<AddonMenuDefinition> children)
        {
            foreach (AddonMenuDefinition menuItem in children)
            {
                menuItem.AddonId = parentVM.AddonId;

                ContextMenuVM contextMenuVM = new ContextMenuVM(menuItem);
                parentVM.Children.Add(contextMenuVM);
            }
        }
    }
}
