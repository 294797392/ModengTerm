using DotNEToolkit;
using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addon.Panel;
using ModengTerm.Base.Definitions;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel.Panel
{
    public abstract class ClientPanel : ViewModelBase, IClientPanel
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("ClientPanel");

        #region 实例变量

        private bool isOpened;
        private string iconURI;
        private PanelContent content;
        private bool initialized;

        #endregion

        #region 公开属性

        public PanelDefinition Definition { get; set; }

        public PanelContent Content
        {
            get { return this.content; }
            private set
            {
                if (this.content != value)
                {
                    this.content = value;
                    this.NotifyPropertyChanged("Content");
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
        
        /// <summary>
        /// Panel所属的插件
        /// </summary>
        public AddonModule OwnerAddon { get; set; }

        #endregion

        #region 实例方法

        private void InitializeContent()
        {
            PanelContent content = null;

            try
            {
                content = ConfigFactory<PanelContent>.CreateInstance(this.Definition.ClassName);
                content.OwnerPanel = this;
                content.OwnerAddon = this.OwnerAddon;
            }
            catch (Exception ex)
            {
                logger.Error("加载PanelContent异常", ex);
            }

            this.Content = content;
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 在第一次Active之后，OnLoad之前触发
        /// </summary>
        public void Initialize()
        {
            this.initialized = true;

            this.InitializeContent();

            // init

            this.OnInitialize();

            // 最后调用content.OnInitialize
            // OverlayPanel OnInitialize的时候需要OverlayPanelContent的OwnerTab赋值
            // OverlayPanelContent.OnInitialize里可能会用到OwnerTab
            this.content.OnInitialize();
        }

        public void Release() 
        {
            if (!this.initialized)
            {
                return;
            }

            this.content.OnRelease();

            this.OnRelease();

            // release

            this.initialized = false;
        }

        /// <summary>
        /// 在显示之后触发
        /// </summary>
        public void Loaded()
        {
            this.content.OnLoaded();
        }

        /// <summary>
        /// 在从界面移出之前触发
        /// </summary>
        public void Unloaded()
        {
            this.content.OnUnload();
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
