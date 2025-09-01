using ModengTerm.Addon.Interactive;
using ModengTerm.Addon.Service;
using ModengTerm.Base;
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

        /// <summary>
        /// 获取该插件所有配置项
        /// </summary>
        public Dictionary<string, object> Options { get; set; }

        public Panel()
        {
            this.factory = ClientFactory.GetFactory();
            this.eventRegistry = this.factory.GetEventRegistry();
            this.storage = this.factory.GetStorageService();
            this.client = this.factory.GetClient();
            this.Options = new Dictionary<string, object>();
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

        /// <summary>
        /// 获取该插件指定的配置项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetOptions<T>(string key)
        {
            return this.Options.GetOptions<T>(key);
        }

        #endregion
    }
}
