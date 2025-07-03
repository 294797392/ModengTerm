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

        /// <summary>
        /// 会话被打开之后触发的事件
        /// </summary>
        TAB_OPENED,

        /// <summary>
        /// 会话被关闭之后触发的事件
        /// </summary>
        TAB_CLOSED,

        /// <summary>
        /// 选项卡选中项改变之后触发
        /// </summary>
        TAB_CHANGED,

        // 301 - 500 ShellTab事件

        /// <summary>
        /// 每次渲染完之后触发
        /// </summary>
        SHELL_RENDERED = 301,

        /// <summary>
        /// 每当发送用户输入的数据的时候触发
        /// </summary>
        SHELL_SEND_USER_INPUT,
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
    public class TabEventTabOpened : TabEventArgs
    {
        public override string Name => "onTabOpened";
        public override string FullName => string.Format("onTabOpened:{0}", this.OpenedTab.Type);

        public override TabEvent Type => TabEvent.TAB_OPENED;

        /// <summary>
        /// 被打开的Tab页面
        /// </summary>
        public IClientTab OpenedTab { get; set; }
    }
    public class TabEventTabClosed : TabEventArgs
    {
        public override string Name => "onTabClosed";
        public override string FullName => string.Format("onTabClosed:{0}", this.ClosedTab.Type);

        public override TabEvent Type => TabEvent.TAB_CLOSED;

        public IClientTab ClosedTab { get; set; }
    }
    public class TabEventTabChanged : TabEventArgs
    {
        public override string Name => "onTabChanged";
        public override string FullName => string.Format("onTabChanged:{0}", this.NewTab.Type);

        public override TabEvent Type => TabEvent.TAB_CHANGED;

        /// <summary>
        /// 选中之前的Tab
        /// </summary>
        public IClientTab OldTab { get; set; }

        /// <summary>
        /// 选中之后的Tab
        /// </summary>
        public IClientTab NewTab { get; set; }
    }


    public class TabEventShellRendered : TabEventArgs
    {
        public override string Name => "onTabShellRendered";

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

    public class TabEventShellSendUserInput : TabEventArgs
    {
        public override string Name => "OnTabShellSendUserInput";

        public override TabEvent Type => TabEvent.SHELL_SEND_USER_INPUT;

        /// <summary>
        /// 发送的数据缓冲区
        /// </summary>
        public byte[] Buffer { get; set; }
    }
}
