using DotNEToolkit.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.DataModels
{
    /// <summary>
    /// 定义触发器的触发条件
    /// </summary>
    public enum TriggerConditionEnum
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
    /// 定义触发器触发的时候要执行的动作
    /// </summary>
    public enum ActionTypeEnum
    {
        /// <summary>
        /// 发送文本
        /// </summary>
        SendText,
    }

    /// <summary>
    /// 定义一个触发器
    /// </summary>
    public class Trigger : ModelBase
    {
        /// <summary>
        /// 触发器运行次数
        /// </summary>
        public int Times { get; set; }

        /// <summary>
        /// 触发器的触发条件
        /// </summary>
        public TriggerConditionEnum Condition { get; set; }

        /// <summary>
        /// 触发器触发的所有条件
        /// </summary>
        public Dictionary<string, string> ConditionParameters { get; set; }

        /// <summary>
        /// 触发器触发的时候要执行的动作
        /// </summary>
        public ActionTypeEnum ActionType { get; set; }

        /// <summary>
        /// 执行动作使用的参数
        /// </summary>
        public Dictionary<string, string> ActionParamters { get; set; }
    }
}
