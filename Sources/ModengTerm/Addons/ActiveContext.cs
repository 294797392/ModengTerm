using ModengTerm.Base.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addons
{
    /// <summary>
    /// 保存插件激活时候的上下文信息
    /// </summary>
    public class ActiveContext
    {
        /// <summary>
        /// 激活该插件的事件
        /// </summary>
        public ActiveEvent Event { get; set; }

        /// <summary>
        /// 事件所带的参数
        /// 不同的ActiveEvent所携带的参数也不同
        /// </summary>
        public ActiveArgument Argument { get; set; }
    }

    public abstract class ActiveArgument
    {

    }
}
