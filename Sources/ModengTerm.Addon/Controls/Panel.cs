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
        protected IClient client;

        public Panel() 
        {
            this.factory = ClientFactory.GetFactory();
            this.eventRegistry = this.factory.GetEventRegistry();
            this.client = this.factory.GetClient();
        }

        /// <summary>
        /// 当创建面板实例的时候触发
        /// </summary>
        public abstract void OnInitialize();

        /// <summary>
        /// 当释放面板的时候触发
        /// </summary>
        public abstract void OnRelease();

        /// <summary>
        /// 当面板在界面上显示之后触发
        /// </summary>
        public abstract void OnLoaded();

        /// <summary>
        /// 当面板从界面中移除之后触发
        /// </summary>
        public abstract void OnUnload();
    }
}
