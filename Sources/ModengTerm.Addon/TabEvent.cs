using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon
{
    /// <summary>
    /// 定义和Tab页面相关的事件
    /// </summary>
    public enum TabEvent
    {
        // 0 - 300 ShellTab事件

        /// <summary>
        /// 每次渲染完之后触发
        /// </summary>
        SHELL_RENDERED
    }

    public abstract class TabEventArgs
    {
        public abstract TabEvent Type { get; }
    }

    public class ShellRenderedEventArgs : TabEventArgs
    {
        public override TabEvent Type => TabEvent.SHELL_RENDERED;
    }
}
