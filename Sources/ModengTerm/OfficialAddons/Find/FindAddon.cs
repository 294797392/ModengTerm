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
    ///// <summary>
    ///// 搜索窗口在所有Shell类型的会话中共享。用这个类来管理搜索窗口的生命周期
    ///// </summary>
    //public static class FindWindowMgr
    //{
    //    private static FindWindow findWindow;

    //    /// <summary>
    //    /// 获取查找窗口是否显示
    //    /// </summary>
    //    public static bool WindowShown { get { return findWindow != null; } }

    //    /// <summary>
    //    /// 显示搜索窗口
    //    /// 如果已经显示了搜索窗口，那么使用传入的shellSession立即进行一次搜索
    //    /// </summary>
    //    /// <param name="shellTab">要搜索的Shell会话</param>
    //    public static void Show(IShellTab shellTab)
    //    {
    //        IVideoTerminal vt = shellTab.VideoTerminal;

    //        // 高亮背景和前景色
    //        string highlightBackground = shellTab.GetOption<string>(OptionKeyEnum.THEME_FIND_HIGHLIGHT_BACKCOLOR, OptionDefaultValues.THEME_FIND_HIGHLIGHT_BACKCOLOR);
    //        string highlightForeground = shellTab.GetOption<string>(OptionKeyEnum.THEME_FIND_HIGHLIGHT_FORECOLOR, OptionDefaultValues.THEME_FIND_HIGHLIGHT_FORECOLOR);

    //        FindVM findVM = null;

    //        if (findWindow == null)
    //        {
    //            findVM = new FindVM()
    //            {
    //                HighlightBackground = VTColor.CreateFromRgbKey(highlightBackground),
    //                HighlightForeground = VTColor.CreateFromRgbKey(highlightForeground)
    //            };

    //            findWindow = new FindWindow(findVM);
    //            findWindow.Owner = Application.Current.MainWindow;
    //            findWindow.Closed += FindWindow_Closed;
    //            findWindow.Show();
    //        }
    //        else
    //        {
    //            // 已经显示了搜索窗口
    //            findVM = findWindow.DataContext as FindVM;
    //            if (findWindow.Visibility == System.Windows.Visibility.Collapsed)
    //            {
    //                findWindow.Visibility = System.Windows.Visibility.Visible;
    //            }
    //        }

    //        findVM.SetVideoTerminal(vt);
    //    }

    //    /// <summary>
    //    /// 隐藏窗口（不关闭）
    //    /// 窗口的所有状态还在
    //    /// </summary>
    //    public static void Hide() 
    //    {
    //        if (findWindow == null) 
    //        {
    //            return;
    //        }

    //        findWindow.Visibility = System.Windows.Visibility.Collapsed;
    //    }

    //    private static void FindWindow_Closed(object? sender, EventArgs e)
    //    {
    //        findWindow.Closed -= FindWindow_Closed;

    //        FindVM findVM = findWindow.DataContext as FindVM;
    //        findVM.Release();

    //        findWindow = null;
    //    }
    //}

    public class FindAddon : AddonModule
    {
        protected override void OnActive(ActiveContext e)
        {
            this.eventRegistory.RegisterHotkey(this, "Esc", HotkeyScopes.ClientShellTab, this.OnEscKeyDown);
            this.eventRegistory.RegisterHotkey(this, "Alt+F", HotkeyScopes.ClientShellTab, this.OnAltFKeyDown);
            this.RegisterCommand("FindAddon.Find", this.FindCommandExecuted);
        }

        protected override void OnDeactive()
        {
        }


        private void FindCommandExecuted(CommandArgs e)
        {
            IClientShellTab shellTab = e.ActiveTab as IClientShellTab;
            if (shellTab == null)
            {
                return;
            }

            IOverlayPanel overlayPanel = this.EnsureOverlayPanel("FindOverlayPanel", shellTab);
            overlayPanel.Dock = OverlayPanelDocks.RightTop;
            overlayPanel.SwitchStatus();
        }

        private void OnEscKeyDown()
        {
            IClientShellTab shellTab = this.client.GetActiveTab<IClientShellTab>();
            IOverlayPanel overlayPanel = shellTab.GetOverlayPanel("FindOverlayPanel");
            if (overlayPanel.IsOpened)
            {
                overlayPanel.Close();
            }
        }

        private void OnAltFKeyDown() 
        {
            IClientShellTab shellTab = this.client.GetActiveTab<IClientShellTab>();
            IOverlayPanel overlayPanel = this.EnsureOverlayPanel("FindOverlayPanel", shellTab);
            overlayPanel.Dock = OverlayPanelDocks.RightTop;
            overlayPanel.SwitchStatus();
        }
    }
}
