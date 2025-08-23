using ModengTerm.Addon.Interactive;
using ModengTerm.Addon.Controls;
using ModengTerm.Base.Addon;
using ModengTerm.Base.Definitions;
using System.Windows;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel.Panels
{
    /// <summary>
    /// 提供扩展侧边栏面板的接口
    /// </summary>
    public class SidePanelVM : ClientPanelVM, ISidePanel
    {
        #region 实例变量

        #endregion

        #region 属性

        public SidePanelDocks Dock { get; set; }

        /// <summary>
        /// 如果侧边栏面板的Scope属于会话，那么该属性存储属于哪个会话
        /// </summary>
        public OpenedSessionVM Session { get; set; }

        #endregion

        #region ClientPanel

        protected override void OnInitialize()
        {
        }

        protected override void OnRelease()
        {
        }

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
