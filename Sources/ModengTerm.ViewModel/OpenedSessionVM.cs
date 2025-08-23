using DotNEToolkit;
using log4net.Repository.Hierarchy;
using ModengTerm.Addon;
using ModengTerm.Addon.Controls;
using ModengTerm.Addon.Interactive;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.ServiceAgents;
using ModengTerm.ViewModel.Panels;
using System.Windows;
using WPFToolkit.MVVM;
using Panel = ModengTerm.Addon.Controls.Panel;

namespace ModengTerm.ViewModel
{
    /// <summary>
    /// 表示一个被打开的会话
    /// </summary>
    public abstract class OpenedSessionVM : SessionItemVM, IClientTab
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("OpenedSessionVM");

        #region 公开事件

        /// <summary>
        /// 会话触发的TabEvent
        /// </summary>
        public event Action<OpenedSessionVM, TabEventArgs> TabEvent;

        #endregion

        #region 实例变量

        private SessionStatusEnum status;
        private DependencyObject content;

        private Dictionary<AddonModule, Dictionary<string, object>> dataMap;

        #endregion

        #region 属性

        /// <summary>
        /// 访问服务的代理
        /// </summary>
        public ServiceAgent ServiceAgent { get; set; }

        /// <summary>
        /// 界面上的控件
        /// </summary>
        public DependencyObject Content
        {
            get { return content; }
            set
            {
                if (content != value)
                {
                    content = value;
                    this.NotifyPropertyChanged("Content");
                }
            }
        }

        /// <summary>
        /// 对应的会话信息
        /// </summary>
        public XTermSession Session { get; private set; }

        /// <summary>
        /// 与会话的连接状态
        /// </summary>
        public SessionStatusEnum Status
        {
            get { return status; }
            protected set
            {
                if (status != value)
                {
                    status = value;
                    this.NotifyPropertyChanged("Status");
                }
            }
        }

        public SessionTypeEnum Type { get { return (SessionTypeEnum)this.Session.Type; } }

        /// <summary>
        /// 该会话所拥有的侧边栏
        /// </summary>
        public List<SidePanelVM> SidePanels { get; private set; }

        #endregion

        #region 构造方法

        public OpenedSessionVM(XTermSession session)
        {
            this.Session = session;
            this.dataMap = new Dictionary<AddonModule, Dictionary<string, object>>();
            this.SidePanels = new List<SidePanelVM>();
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 在所有属性都赋值之后，Open之前调用
        /// 和构造函数相比有个不同的地方，Initialize调用的时候所有属性都赋值了，而构造函数调用的时候则没有
        /// </summary>
        public void Initialize()
        {
            // init

            this.OnInitialize();
        }

        public void Release()
        {
            this.OnRelease();

            // release
            this.dataMap.Clear();
        }

        public int Open()
        {
            // 创建面板容器
            return OnOpen();
        }

        public void Close()
        {
            OnClose();
        }

        public void CreateSidePanels(List<SidePanelMetadata> metadatas)
        {
            foreach (SidePanelMetadata metadata in metadatas)
            {
                SidePanelVM spvm = VMUtils.CreateSidePanelVM(metadata);
                spvm.Session = this;

                #region 创建Panel实例

                SidePanel sidePanel = null;

                try
                {
                    sidePanel = ConfigFactory<SidePanel>.CreateInstance(spvm.Metadata.ClassName);
                    sidePanel.OnInitialize();
                }
                catch (Exception ex)
                {
                    logger.Error("加载Panel异常", ex);
                    return;
                }

                spvm.Panel = sidePanel;

                #endregion

                TabedSidePanel tabSidePanel = sidePanel as TabedSidePanel;
                if (tabSidePanel != null)
                {
                    tabSidePanel.Tab = this;
                }

                spvm.Initialize();

                this.SidePanels.Add(spvm);
            }
        }

        #endregion

        #region IClientTab

        public T GetOption<T>(OptionKeyEnum key, T defaultValue)
        {
            return this.Session.GetOption<T>(key, defaultValue);
        }

        public void SetData(AddonModule addon, string key, object value)
        {
            Dictionary<string, object> map;

            if (!this.dataMap.TryGetValue(addon, out map))
            {
                map = new Dictionary<string, object>();
                this.dataMap[addon] = map;
            }

            map[key] = value;
        }

        public TValue GetData<TValue>(AddonModule addon, string key)
        {
            Dictionary<string, object> map;

            if (!this.dataMap.TryGetValue(addon, out map))
            {
                return default(TValue);
            }

            object value;
            if (!map.TryGetValue(key, out value))
            {
                return default(TValue);
            }

            if (value == null)
            {
                return default(TValue);
            }

            return (TValue)value;
        }

        #endregion

        #region 抽象方法

        protected abstract void OnInitialize();
        protected abstract void OnRelease();
        protected abstract int OnOpen();
        protected abstract void OnClose();

        #endregion

        #region 受保护方法

        protected void RaiseTabEvent(TabEventArgs e)
        {
            this.TabEvent?.Invoke(this, e);
        }

        #endregion

        #region 实例方法

        #endregion

        #region 事件处理器

        #endregion
    }
}
