using ModengTerm.Addon.Client;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addon.Panel;
using ModengTerm.Base.Definitions;
using System.Windows;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel.Panel
{
    /// <summary>
    /// 提供扩展面板的接口
    /// </summary>
    public class HostPanel : ViewModelBase, IHostPanel
    {
        #region 实例变量

        private bool isOpened;
        private string iconURI;

        #endregion

        #region 属性

        public IAddonPanel ClientPanel { get { return this.Content as IAddonPanel; } }

        public FrameworkElement Content { get; set; }

        public SidePanelDocks Dock { get; set; }

        public PanelDefinition Definition { get; set; }

        /// <summary>
        /// 该面板是否打开
        /// </summary>
        public bool IsOpened
        {
            get { return this.isOpened; }
            set
            {
                if (this.isOpened != value)
                {
                    this.isOpened = value;
                    this.NotifyPropertyChanged("IsOpened");
                }
            }
        }

        public string IconURI
        {
            get { return this.iconURI; }
            set
            {
                if (this.iconURI != value)
                {
                    this.iconURI = value;
                    this.NotifyPropertyChanged("IconURI");
                }
            }
        }

        #endregion

        #region IHostPanel

        /// <summary>
        /// 打开侧边栏
        /// </summary>
        public void Open()
        {
            this.IsOpened = true;
        }

        /// <summary>
        /// 关闭侧边栏
        /// </summary>
        public void Close()
        {
            this.IsOpened = false;
        }

        public void Initialize(PanelContext context)
        {
            this.ClientPanel.OnInitialize(context);
        }

        /// <summary>
        /// 在显示之后触发
        /// </summary>
        public void Loaded()
        {
            this.ClientPanel.OnLoaded();
        }

        /// <summary>
        /// 在从界面移出之前触发
        /// </summary>
        public void Unloaded()
        {
            this.ClientPanel.OnUnload();
        }

        public void Release()
        {
            this.ClientPanel.OnRelease();
        }

        public void SwitchStatus()
        {
            if (this.isOpened)
            {
                this.Close();
            }
            else
            {
                this.Open();
            }
        }

        #endregion
    }
}
