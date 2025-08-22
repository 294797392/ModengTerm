using ModengTerm.Addon.Interactive;
using ModengTerm.Addon.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        /// <summary>
        /// 所属的Panel
        /// 在创建PanelContent实例的时候赋值
        /// </summary>
        public IClientPanel OwnerPanel { get; set; }

        public Panel() 
        {
            this.factory = ClientFactory.GetFactory();
            this.eventRegistry = this.factory.GetEventRegistry();
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
