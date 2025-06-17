using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon.Panel
{
    /// <summary>
    /// 1. 为插件提供回调功能
    /// 2. 为插件提供数据
    /// </summary>
    public interface IPanelCallback
    {
        /// <summary>
        /// 面板名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Host传递给插件的HostFactory
        /// </summary>
        HostFactory HostFactory { get; set; }

        /// <summary>
        /// 面板初始化的时候触发
        /// </summary>
        void OnInitialize();

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
