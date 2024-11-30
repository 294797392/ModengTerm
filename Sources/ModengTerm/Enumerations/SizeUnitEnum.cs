using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Enumerations
{
    /// <summary>
    /// 指定内存单位的显示方式
    /// </summary>
    public enum SizeUnitsEnum
    {
        /// <summary>
        /// 字节
        /// </summary>
        Byte,

        /// <summary>
        /// 以KB显示
        /// </summary>
        KB,

        /// <summary>
        /// 以MB方式显示
        /// </summary>
        MB,

        /// <summary>
        /// 以GB方式显示
        /// </summary>
        GB,

        TB
    }
}
