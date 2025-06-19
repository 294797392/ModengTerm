using DotNEToolkit;
using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addon.Panel;
using System.Windows;
using System.Windows.Threading;

namespace ModengTerm.ViewModel.Panel
{
    /// <summary>
    /// 提供扩展悬浮面板的接口
    /// </summary>
    public class OverlayPanel : HostPanel, IOverlayPanel
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("OverlayPanel");

        #endregion

        #region 实例变量

        private OverlayPanelDocks dock;

        #endregion

        #region 公开属性

        /// <summary>
        /// 悬浮面板所属的Tab页面
        /// </summary>
        public IClientShellTab OwnerTab { get; set; }

        public ClientFactory HostFactory { get; set; }

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

        #endregion

        #region HostPanel

        public override void Open()
        {
            if (this.Content == null) 
            {
                FrameworkElement frameworkElement = null;

                try
                {
                    frameworkElement = ConfigFactory<FrameworkElement>.CreateInstance(this.Definition.ClassName);
                }
                catch (Exception ex)
                {
                    logger.Error("加载OverlayPanel异常", ex);
                    return;
                }

                this.Content = frameworkElement;

                IAddonOverlayPanel callback = this.Callback as IAddonOverlayPanel;
                callback.OwnerTab = this.OwnerTab;
                base.Initialize();
            }

            this.IsOpened = true;

            // https://gitee.com/zyfalreadyexsit/terminal/issues/ICG96L
            // 确保控件Loaded完毕，此时设置才可以设置焦点
            this.Content.Dispatcher.BeginInvoke(base.Loaded, DispatcherPriority.Loaded);
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
