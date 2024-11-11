using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Triggers
{
    public enum ActionTypeEnum
    {
        /// <summary>
        /// 发送文本
        /// </summary>
        SendText,
    }

    public class TriggerAction
    {
        /// <summary>
        /// 动作类型
        /// </summary>
        public ActionTypeEnum Type { get; set; }

        /// <summary>
        /// 动作参数
        /// </summary>
        public string Parameter { get; set; }
    }
}
