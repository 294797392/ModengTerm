using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Watch
{
    /// <summary>
    /// 数据同步器
    /// 用于Source拷贝到Target里
    /// </summary>
    public abstract class ObjectCopy<Target, Source>
    {
        /// <summary>
        /// 把Source的数据更新到target
        /// </summary>
        public abstract void CopyTo(Target target, Source source);

        /// <summary>
        /// target和source做比对看是否一致
        /// </summary>
        /// <returns></returns>
        public abstract bool Compare(Target target, Source source);
    }
}
