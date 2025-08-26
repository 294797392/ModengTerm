using ModengTerm.Addon.Interactive;
using ModengTerm.Addon.Service;
using System.Windows.Controls;

namespace ModengTerm.Addon.Controls
{
    /// <summary>
    /// 表示扩展的Panel里的内容
    /// </summary>
    public abstract class Panel : UserControl
    {
        protected ClientFactory factory;
        protected IClientEventRegistry eventRegistry;
        protected IClientStorage storage;
        protected IClient client;

        /// <summary>
        /// 插件Panel用来管理客户端Panel状态的接口
        /// </summary>
        public IClientPanel Container { get; set; }

        public Panel()
        {
            this.factory = ClientFactory.GetFactory();
            this.eventRegistry = this.factory.GetEventRegistry();
            this.storage = this.factory.GetStorageService();
            this.client = this.factory.GetClient();
        }

        #region 抽象方法

        /// <summary>
        /// 当创建面板实例的时候触发
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// 当释放面板的时候触发
        /// </summary>
        public abstract void Release();

        /// <summary>
        /// 当面板在界面上显示之后触发
        /// </summary>
        public abstract void Load();

        /// <summary>
        /// 当面板从界面中移除之后触发
        /// </summary>
        public abstract void Unload();

        #endregion

        protected void StartTimer(Action<object> action, object userData) 
        {

        }

        protected void StopTimer(Action<object> action, object userData)
        {

        }
    }
}
