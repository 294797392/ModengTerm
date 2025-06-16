using ModengTerm.Addon.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon.Panel
{
    /// <summary>
    /// 自定义的插件面板需要实现这个接口
    /// </summary>
    public interface IAddonPanel
    {
        /// <summary>
        /// 面板名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 面板初始化的时候触发
        /// </summary>
        /// <param name="context"></param>
        void OnInitialize(PanelContext context);

        /// <summary>
        /// 释放面板资源的时候触发
        /// </summary>
        void OnRelease();

        /// <summary>
        /// 当面板在界面上显示的时候触发
        /// </summary>
        void OnLoaded();

        /// <summary>
        /// 当面板从界面中移除后触发
        /// </summary>
        void OnUnload();
    }
}
