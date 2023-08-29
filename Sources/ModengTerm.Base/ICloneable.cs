using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Base
{
    public interface ICloneable<T>
    {
        /// <summary>
        /// 把该对象里的所有的属性内容复制到目标对象
        /// </summary>
        /// <param name="dest">要复制到的目标对象</param>
        void CopyTo(T dest);

        /// <summary>
        /// 把该对象的值设置为初始值
        /// </summary>
        void SetDefault();
    }
}
