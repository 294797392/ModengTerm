using ModengTerm.Addon.Interactive;
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
        /// 
        /// </summary>
        CLIENT_ACTIVE_TAB_CHANGED
    }

    public abstract class ClientEventArgs
    {
        public abstract ClientEvent Type { get; }
    }

    public class ClientEventClientInitialized : ClientEventArgs
    {
        public override ClientEvent Type => ClientEvent.CLIENT_INITIALIZED;
    }

    public class ClientEventActiveTabChanged : ClientEventArgs
    {
        public override ClientEvent Type => ClientEvent.CLIENT_ACTIVE_TAB_CHANGED;

        /// <summary>
        /// 选中之前的Tab页面
        /// </summary>
        public IClientTab RemovedTab { get; set; }

        /// <summary>
        /// 选中之后的Tab页面
        /// </summary>
        public IClientTab AddedTab { get; set; }
    }
}
