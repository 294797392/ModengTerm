using ModengTerm.Addon.Interactive;
using ModengTerm.Base;
using ModengTerm.Base.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon
{
    /// <summary>
    /// 定义客户端通用事件
    /// 所有事件在UI线程触发
    /// </summary>
    public enum ClientEvent
    {
        // 所有CLIENT开头的事件类型，是通用事件类型。0 - 100 通用

        /// <summary>
        /// 当应用程序启动之后触发
        /// </summary>
        CLIENT_INITIALIZED = 0,

        /// <summary>
        /// 当前显示的Tab变化之后触发
        /// onClientTabChanged:ssh|local|serial|tcp
        /// </summary>
        CLIENT_TAB_CHANGED,

        /// <summary>
        /// onClientTabOpened:ssh|local|serial|tcp
        /// </summary>
        CLIENT_TAB_OPENED,

        /// <summary>
        /// onClientTabClosed:ssh|local|serial|tcp
        /// </summary>
        CLIENT_TAB_CLOSED
    }

    public abstract class EventArgsBase
    {
        public virtual string Name { get { return string.Empty; } }

        public virtual bool MatchCondition(string cond)
        {
            return true;
        }
    }

    public abstract class ClientEventArgs : EventArgsBase
    {
        public abstract ClientEvent Type { get; }
    }

    public class ClientEventClientInitialized : ClientEventArgs
    {
        public override string Name => "onClientInitialized";

        public override ClientEvent Type => ClientEvent.CLIENT_INITIALIZED;
    }

    /// <summary>
    /// onClientTabChanged:ssh|local|serial|tcp
    /// </summary>
    public class ClientEventTabChanged : ClientEventArgs
    {
        public override string Name => "onClientTabChanged";

        public override ClientEvent Type => ClientEvent.CLIENT_TAB_CHANGED;

        /// <summary>
        /// 选中之前的Tab页面
        /// </summary>
        public IClientTab OldTab { get; set; }

        /// <summary>
        /// 选中之后的Tab页面
        /// </summary>
        public IClientTab NewTab { get; set; }

        public override bool MatchCondition(string cond)
        {
            return cond.Contains(VTBaseUtils.GetSessionTypeName(this.NewTab.Type));
        }
    }

    /// <summary>
    /// onTabOpened:ssh|local|serial|tcp
    /// </summary>
    public class ClientEventTabOpened : ClientEventArgs
    {
        public override string Name => "onClientTabOpened";

        public override ClientEvent Type => ClientEvent.CLIENT_TAB_OPENED;

        /// <summary>
        /// 被打开的Tab页面
        /// </summary>
        public IClientTab OpenedTab { get; set; }

        public override bool MatchCondition(string cond)
        {
            return cond.Contains(VTBaseUtils.GetSessionTypeName(this.OpenedTab.Type));
        }
    }

    public class ClientEventTabClosed : ClientEventArgs
    {
        public override string Name => "onClientTabClosed";

        public override ClientEvent Type => ClientEvent.CLIENT_TAB_CLOSED;

        public IClientTab ClosedTab { get; set; }

        public override bool MatchCondition(string cond)
        {
            return cond.Contains(VTBaseUtils.GetSessionTypeName(this.ClosedTab.Type));
        }
    }
}






