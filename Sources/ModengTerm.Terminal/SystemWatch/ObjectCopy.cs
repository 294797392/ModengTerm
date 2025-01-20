using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        /// 从上次Copy到这次Copy的时间
        /// </summary>
        public TimeSpan Elapsed { get { return this.Stopwatch.Elapsed; } }

        public Stopwatch Stopwatch { get; private set; }

        public ObjectCopy() 
        {
            this.Stopwatch = new Stopwatch();
        }

        /// <summary>
        /// 把Source的数据更新到target
        /// </summary>
        /// <param name="target">要拷贝到的对象</param>
        /// <param name="source">从哪个对象拷贝</param>
        /// <param name="elapsed">从上次Copy到这次拷贝的时间</param>
        public abstract void CopyTo(Target target, Source source);

        /// <summary>
        /// target和source做比对看是否一致
        /// </summary>
        /// <returns></returns>
        public abstract bool Compare(Target target, Source source);
    }
}
