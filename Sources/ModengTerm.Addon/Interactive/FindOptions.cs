using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon.Interactive
{
    public struct FindOptions
    {
        /// <summary>
        /// 查找关键字
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// 是否使用正则表达式搜索
        /// </summary>
        public bool Regexp { get; set; }

        /// <summary>
        /// 是否忽略大小写
        /// true不忽略大小写
        /// false忽略大小写
        /// </summary>
        public bool CaseSensitive { get; set; }
    }
}
