using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Ftp.Enumerations
{
    public enum ListOptions
    {
        /// <summary>
        /// 只列举文件
        /// </summary>
        File = 2,

        /// <summary>
        /// 只列举目录
        /// </summary>
        Directory = 4,

        /// <summary>
        /// 列举文件和目录
        /// </summary>
        All = 1024,
    }
}
