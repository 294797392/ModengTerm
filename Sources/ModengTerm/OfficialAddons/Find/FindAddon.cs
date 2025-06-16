using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addons;
using ModengTerm.Base;
using ModengTerm.Base.Enumerations;
using ModengTerm.Document;
using System;
using System.Windows;

namespace ModengTerm.OfficialAddons.Find
{
    /// <summary>
    /// 搜索窗口在所有Shell类型的会话中共享。用这个类来管理搜索窗口的生命周期
    /// </summary>
    public static class FindWindowMgr
    {
        private static FindWindow findWindow;

        /// <summary>
        /// 获取查找窗口是否显示
        /// </summary>
        public static bool WindowShown { get { return findWindow != null; } }

        /// <summary>
        /// 显示搜索窗口
        /// 如果已经显示了搜索窗口，那么使用传入的shellSession立即进行一次搜索
        /// </summary>
        /// <param name="shellTab">要搜索的Shell会话</param>
        public static void Show(IShellTab shellTab)
        {
            IVideoTerminal vt = shellTab.VideoTerminal;

            // 高亮背景和前景色
            string highlightBackground = shellTab.GetOption<string>(OptionKeyEnum.THEME_FIND_HIGHLIGHT_BACKCOLOR, OptionDefaultValues.THEME_FIND_HIGHLIGHT_BACKCOLOR);
            string highlightForeground = shellTab.GetOption<string>(OptionKeyEnum.THEME_FIND_HIGHLIGHT_FORECOLOR, OptionDefaultValues.THEME_FIND_HIGHLIGHT_FORECOLOR);

            FindVM findVM = null;

            if (findWindow == null)
            {
                findVM = new FindVM()
                {
                    HighlightBackground = VTColor.CreateFromRgbKey(highlightBackground),
                    HighlightForeground = VTColor.CreateFromRgbKey(highlightForeground)
                };

                findWindow = new FindWindow(findVM);
                findWindow.Owner = Application.Current.MainWindow;
                findWindow.Closed += FindWindow_Closed;
                findWindow.Show();
            }
            else
            {
                // 已经显示了搜索窗口
                findVM = findWindow.DataContext as FindVM;
                if (findWindow.Visibility == System.Windows.Visibility.Collapsed)
                {
                    findWindow.Visibility = System.Windows.Visibility.Visible;
                }
            }

            findVM.SetVideoTerminal(vt);
        }

        /// <summary>
        /// 隐藏窗口（不关闭）
        /// 窗口的所有状态还在
        /// </summary>
        public static void Hide() 
        {
            if (findWindow == null) 
            {
                return;
            }

            findWindow.Visibility = System.Windows.Visibility.Collapsed;
        }

        private static void FindWindow_Closed(object? sender, EventArgs e)
        {
            findWindow.Closed -= FindWindow_Closed;

            FindVM findVM = findWindow.DataContext as FindVM;
            findVM.Release();

            findWindow = null;
        }
    }

    public class FindAddon : AddonModule
    {
        private FindWindow findWindow;

        protected override void OnActive(ActiveContext e)
        {
            this.RegisterEvent(HostEvent.HOST_ACTIVE_TAB_CHANGED, this.OnActiveTabChanged);
            this.RegisterCommand("FindAddon.Find", this.OpenFindWindow);
        }

        protected override void OnDeactive()
        {
        }

        private void OnActiveTabChanged(HostEvent evType, HostEventArgs evArgs)
        {
            //throw new RefactorImplementedException();

            //ActiveTabChangedEventArgs activeTabChanged = evArgs as ActiveTabChangedEventArgs;

            //// 选中的不是终端类型的Tab页面，隐藏搜索窗口
            //if (activeTabChanged.AddedTab == null || !VTBaseUtils.IsTerminal(activeTabChanged.AddedTab.Type))
            //{
            //    return;
            //}

            //// 如果选中的会话是Shell会话并且显示了查找窗口，那么搜索选中的会话
            //if (FindWindowMgr.WindowShown)
            //{
            //    // bug:新打开窗口之后，也会触发该事件，但是此时心打开的窗口里的会话还没初始化完，所以会导致IVideoTerminal为空的bug

            //    IHostInsidePanel


            //    //FindWindowMgr.Show(activeTabChanged.AddedTab as IShellTab);
            //}
            //else
            //{
                
            //}
        }

        private void OpenFindWindow(CommandArgs e)
        {
            IShellTab shellTab = this.hostWindow.GetActiveTab<IShellTab>();
            FindWindowMgr.Show(shellTab);
        }
    }
}
