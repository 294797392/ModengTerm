using DotNEToolkit;
using ModengTerm.Addon.Client;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addon.Panel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFToolkit.MVVM;

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
        private PanelContext context;

        #endregion

        #region 公开属性

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

        public OverlayPanel(PanelContext context)
        {
            this.context = context;
        }
        
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
                this.ClientPanel.OnInitialize(this.context);
            }

            this.IsOpened = true;

            this.ClientPanel.OnLoaded();
        }

        public override void Close()
        {
            this.IsOpened = false;

            this.ClientPanel.OnUnload();
        }

        #endregion
    }
}
