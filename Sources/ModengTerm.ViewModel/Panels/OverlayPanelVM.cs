using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addon.Controls;
using ModengTerm.Base.Addon;
using System.Windows.Threading;
using Panel = ModengTerm.Addon.Controls.Panel;

namespace ModengTerm.ViewModel.Panels
{
    /// <summary>
    /// 提供扩展悬浮面板的接口
    /// </summary>
    public class OverlayPanelVM : ClientPanelVM, IOverlayPanel
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("OverlayPanel");

        #endregion

        #region 实例变量

        private OverlayPanelDocks dock;
        private ClientFactory factory;

        #endregion

        #region 公开属性

        /// <summary>
        /// 悬浮面板所属的Tab页面
        /// </summary>
        public IClientShellTab OwnerTab { get; set; }

        public OverlayPanelDocks Dock
        {
            get { return this.dock; }
            set
            {
                if (this.dock != value)
                {
                    this.dock = value;
                    this.NotifyPropertyChanged("Dock");
                }
            }
        }

        #endregion

        #region 构造方法

        public OverlayPanelVM() 
        {
            this.factory = ClientFactory.GetFactory();
        }

        #endregion

        #region ClientPanel

        protected override void OnInitialize()
        {
            OverlayPanel overlayPanelContent = base.Panel as OverlayPanel;
            overlayPanelContent.OwnerTab = this.OwnerTab;
        }

        protected override void OnRelease()
        {
        }

        public override void Open()
        {
            Panel content = this.Panel;

            this.IsOpened = true;

            // https://gitee.com/zyfalreadyexsit/terminal/issues/ICG96L
            // 确保控件Loaded完毕，此时设置才可以设置焦点
            this.Panel.Dispatcher.BeginInvoke(base.Loaded, DispatcherPriority.Loaded);
        }

        public override void Close()
        {
            this.IsOpened = false;

            base.Unloaded();
        }

        #endregion

        #region IOverlayPanel

        //public void SetOptions(OverlayPanelOptions options)
        //{
        //    this.Dock = options.Dock;
        //}

        #endregion
    }
}
