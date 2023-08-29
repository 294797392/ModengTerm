using DotNEToolkit.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Base.DataModels
{
    /// <summary>
    /// 表示一个收藏夹
    /// </summary>
    public class Favorites : ModelBase
    {
        /// <summary>
        /// 该收藏属于哪个会话
        /// </summary>
        public string SessionID { get; set; }

        /// <summary>
        /// 起始行
        /// </summary>
        public int FirstRow { get; set; }

        /// <summary>
        /// 起始行的起始字符索引
        /// </summary>
        public int FirstCharacterIndex { get; set; }

        /// <summary>
        /// 结束行
        /// </summary>
        public int LastRow { get; set; }

        /// <summary>
        /// 结束行的结束字符索引
        /// </summary>
        public int LastCharacterIndex { get; set; }
    }
}
