using DotNEToolkit;
using ModengTerm.Addon.Interactive;
using ModengTerm.Base.Definitions;
using WPFToolkit.MVVM;
using Panel = ModengTerm.Addon.Controls.Panel;

namespace ModengTerm.ViewModel.Panels
{
    public abstract class ClientPanelVM : ViewModelBase, IClientPanel
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("ClientPanelVM");

        #region 实例变量

        private bool isOpened;
        private string iconURI;
        private Addon.Controls.Panel panel;
        private bool initialized;

        #endregion

        #region 公开属性

        /// <summary>
        /// 面板元数据信息
        /// </summary>
        public PanelMetadata Metadata { get; set; }

        /// <summary>
        /// 插件面板实例
        /// </summary>
        public Panel Panel
        {
            get { return this.panel; }
            set
            {
                if (this.panel != value)
                {
                    this.panel = value;
                    this.NotifyPropertyChanged("Panel");
                }
            }
        }

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

        /// <summary>
        /// 面板对应的图标路径
        /// </summary>
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

        #region 实例方法

        #endregion

        #region 公开接口

        /// <summary>
        /// 在第一次创建面板的时候触发
        /// </summary>
        public void Initialize()
        {
            this.initialized = true;

            // 初始化ClientPanelVM
            this.OnInitialize();
        }

        public void Release()
        {
            if (!this.initialized)
            {
                return;
            }

            this.panel.OnRelease();

            this.OnRelease();

            // release

            this.initialized = false;
        }

        /// <summary>
        /// 在显示之后触发
        /// </summary>
        public void Loaded()
        {
            this.panel.OnLoaded();
        }

        /// <summary>
        /// 在从界面移出之前触发
        /// </summary>
        public void Unloaded()
        {
            this.panel.OnUnload();
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

        #region 抽象方法

        protected abstract void OnInitialize();
        protected abstract void OnRelease();

        /// <summary>
        /// 打开侧边栏
        /// </summary>
        public abstract void Open();

        /// <summary>
        /// 关闭侧边栏
        /// </summary>
        public abstract void Close();

        #endregion
    }
}
