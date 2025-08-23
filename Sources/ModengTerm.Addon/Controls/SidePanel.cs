using ModengTerm.Addon.Interactive;
using ModengTerm.Base.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon.Controls
{
    /// <summary>
    /// 插件要实现的SidePanel接口
    /// </summary>
    public abstract class SidePanel : Panel
    {
    }

    public abstract class TabedSidePanel : SidePanel
    {
        /// <summary>
        /// 获取侧边栏所属的Tab
        /// </summary>
        public IClientTab Tab { get; set; }
    }
}
