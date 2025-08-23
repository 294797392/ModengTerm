using ModengTerm.Addon.Interactive;
using ModengTerm.Base.Enumerations;
using ModengTerm.Document;
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
        // 0 - 300 通用Tab事件

        /// <summary>
        /// 会话状态改变事件
        /// </summary>
        TAB_STATUS_CHANGED,

        // 301 - 500 ShellTab事件

        /// <summary>
        /// onTabShellRendered
        /// 每次渲染完之后触发
        /// </summary>
        SHELL_RENDERED = 301,

        /// <summary>
        /// onTabShellUserInput
        /// 每当发送用户输入的数据的时候触发
        /// </summary>
        SHELL_USER_INPUT,
    }

    public abstract class TabEventArgs : EventArgsBase
    {
        public abstract TabEvent Type { get; }

        /// <summary>
        /// 触发该事件的Tab
        /// </summary>
        public IClientTab Sender { get; set; }
    }

    public class TabEventStatusChanged : TabEventArgs
    {
        public override TabEvent Type => TabEvent.TAB_STATUS_CHANGED;

        public SessionStatusEnum OldStatus { get; set; }

        public SessionStatusEnum NewStatus { get; set; }

        public IClientTab Tab { get; set; }
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

        /// <summary>
        /// 记录主缓冲区新打印的行数
        /// 不包含光标所在行，因为光标所在行有可能还没打印结束
        /// </summary>
        public List<VTHistoryLine> NewLines { get; private set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public long Timestamp { get; set; }

        public TabEventShellRendered()
        {
            this.NewLines = new List<VTHistoryLine>();
        }
    }

    public class TabEventShellUserInput : TabEventArgs
    {
        public override TabEvent Type => TabEvent.SHELL_USER_INPUT;

        /// <summary>
        /// 发送的数据缓冲区
        /// </summary>
        public byte[] Buffer { get; set; }
    }
}
