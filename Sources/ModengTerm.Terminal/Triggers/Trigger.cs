using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Triggers
{
    public abstract class Trigger
    {
        /// <summary>
        /// 触发器运行次数
        /// </summary>
        public int Times { get; set; }
    }

    /// <summary>
    /// 事件触发器
    /// </summary>
    public class EventTrigger : Trigger
    {
        public enum EventTypeEnum
        {
            /// <summary>
            /// 连接上主机之后触发
            /// </summary>
            HostConnected,

            /// <summary>
            /// 与主机断开连接之后触发
            /// </summary>
            HostDisconnected,

            /// <summary>
            /// 接收到数据之后触发
            /// </summary>
            DataReceived,

            /// <summary>
            /// 发送完数据之后触发
            /// </summary>
            DataSent
        }

        /// <summary>
        /// 触发器的事件类型
        /// </summary>
        public EventTypeEnum Type { get; set; }
    }
}
