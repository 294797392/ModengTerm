using ModengTerm.Addon.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon
{
    /// <summary>
    /// 定义和Tab页面相关的事件
    /// 所有事件在UI线程触发
    /// </summary>
    public enum TabEvent
    {
        // 0 - 300 ShellTab事件

        /// <summary>
        /// 每次渲染完之后触发
        /// </summary>
        SHELL_RENDERED,

        /// <summary>
        /// 每当发送数据的时候触发
        /// </summary>
        SHELL_SENDDATA
    }

    public abstract class TabEventArgs
    {
        public abstract TabEvent Type { get; }

        /// <summary>
        /// 触发该事件的Tab
        /// </summary>
        public IClientTab ClientTab { get; set; }
    }

    public class TabEventShellRendered : TabEventArgs
    {
        public override TabEvent Type => TabEvent.SHELL_RENDERED;

        /// <summary>
        /// 渲染的数据缓冲区
        /// </summary>
        public byte[] Buffer { get; set; }

        /// <summary>
        /// 缓冲区中的数据长度
        /// </summary>
        public int Length { get; set; }
    }

    public class TabEventShellSendData : TabEventArgs
    {
        public override TabEvent Type => TabEvent.SHELL_SENDDATA;

        /// <summary>
        /// 发送的数据缓冲区
        /// </summary>
        public byte[] Buffer { get; set; }
    }
}
