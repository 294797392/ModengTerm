using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document.Enumerations
{
    /// <summary>
    /// EraseLine的类型
    /// </summary>
    public enum EraseType
    {
        /// <summary>
        /// 删除从当前光标处到该行结尾的所有字符
        /// </summary>
        ToEnd = 0,

        /// <summary>
        /// 删除从行首到当前光标处的字符
        /// </summary>
        FromBeginning = 1,

        /// <summary>
        /// 删除光标所在整行字符
        /// </summary>
        All = 2
    }
}
