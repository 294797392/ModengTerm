using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Base
{
    /// <summary>
    /// 表示一个可以被复用的对象
    /// </summary>
    public abstract class Reusable<T> : ICloneable<T> where T : ICloneable<T>
    {
        /// <summary>
        /// 把该对象里的所有的属性内容复制到目标对象
        /// </summary>
        /// <param name="dest">要复制到的目标对象</param>
        public abstract void CopyTo(T dest);

        /// <summary>
        /// 把该对象的值设置为初始值
        /// </summary>
        public abstract void SetDefault();

        public static T Create() 
        {
            return ReusableQueue<T>.Create();
        }

        public static void Recycle(T instance)
        {
            ReusableQueue<T>.Recycle(instance);
        }

        public static void Recycle(IEnumerable<T> list)
        {
            foreach (T item in list)
            {
                Recycle(item);
            }
        }

        public static void CopyTo(List<T> copyTo, List<T> copyFrom)
        {
            ReusableQueue<T>.CopyTo(copyTo, copyFrom);
        }
    }
}
