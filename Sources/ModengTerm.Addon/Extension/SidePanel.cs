using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon.Extensions
{
    /// <summary>
    /// 向插件公开扩展侧边栏的功能
    /// </summary>
    public abstract class SidePanel
    {
        public string Name { get; set; }

        public abstract void OnInitialize();
        public abstract void OnRelease();
        public abstract void OnLoaded();
        public abstract void OnUnload();
    }
}
