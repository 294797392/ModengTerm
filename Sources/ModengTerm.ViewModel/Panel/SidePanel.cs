using ModengTerm.Addon.Client;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addon.Panel;
using ModengTerm.Base.Definitions;
using System.Windows;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel.Panel
{
    /// <summary>
    /// 提供扩展侧边栏面板的接口
    /// </summary>
    public class SidePanel : HostPanel, ISidePanel
    {
        #region 实例变量

        #endregion

        #region 属性

        public SidePanelDocks Dock { get; set; }

        #endregion

        #region HostPanel

        public override void Open()
        {
            this.IsOpened = true;
        }

        public override void Close()
        {
            this.IsOpened = false;
        }

        #endregion
    }
}
