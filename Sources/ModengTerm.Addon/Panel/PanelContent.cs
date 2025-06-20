using ModengTerm.Addon.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ModengTerm.Addon.Panel
{
    /// <summary>
    /// 表示扩展的Panel里的内容
    /// </summary>
    public abstract class PanelContent : UserControl
    {
        /// <summary>
        /// 所属的Panel
        /// 在创建PanelContent实例的时候赋值
        /// </summary>
        public IClientPanel OwnerPanel { get; set; }

        /// <summary>
        /// 面板初始化的时候触发
        /// </summary>
        public abstract void OnInitialize();

        /// <summary>
        /// 释放面板资源的时候触发
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
