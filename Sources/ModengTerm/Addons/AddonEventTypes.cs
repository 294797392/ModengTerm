using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addons
{
    public enum AddonEventTypes
    {
        /// <summary>
        /// 当某个会话状态改变的时候触发
        /// OpenedSessionVM session
        /// SessionStatusEnum oldStatus
        /// SessionStatusEnum newState
        /// </summary>
        SessionStatusChanged,

        /// <summary>
        /// 当选中的会话改变的时候触发
        /// OpenedSessionVM removedSession
        /// OpenedSessionVM addedSession
        /// </summary>
        SelectedSessionChanged
    }
}
