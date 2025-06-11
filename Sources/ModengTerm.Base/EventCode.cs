using ModengTerm.Base.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Base
{
    /// <summary>
    /// OpenedSessionVM触发的事件类型
    /// </summary>
    public enum EventCode
    {
        // 0 - 100 定义通用事件
        COMMON_STATUS_CHANGED = 0,

        // 101 - 200定义Shell会话事件

        // 201 - 300定义Sftp会话事件
    }

    public class StatusChangedEventArgs : EventArgs
    {
        public SessionStatusEnum OldStatus { get; set; }

        public SessionStatusEnum NewStatus { get; set; }
    }
}
