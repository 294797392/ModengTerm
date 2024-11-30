using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Watch
{
    /// <summary>
    /// 数据同步器
    /// 用于把不同平台的数据转换成统一类型的数据
    /// </summary>
    public abstract class Sync<Target, Source>
    {
        /// <summary>
        /// 把Source的数据填充到target
        /// </summary>
        public abstract void Update(Target target, Source source);

        /// <summary>
        /// target和source做比对看是否一致
        /// </summary>
        /// <returns></returns>
        public abstract bool Compare(Target target, Source source);
    }
}
