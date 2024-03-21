using DotNEToolkit.DataModels;
using ModengTerm.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.DataModels
{
    /// <summary>
    /// 表示一个收藏夹
    /// 相当于一个段落的快照
    /// </summary>
    public class Favorites : ModelBase
    {
        /// <summary>
        /// 该收藏属于哪个会话
        /// </summary>
        public string SessionID { get; set; }

        /// <summary>
        /// 该段落的字符列表
        /// </summary>
        public List<VTHistoryLine> CharacterList { get; set; }

        /// <summary>
        /// 第一行第一个的字符索引
        /// </summary>
        public int StartCharacterIndex { get; set; }

        /// <summary>
        /// 最后一行最后一个字符的索引
        /// </summary>
        public int EndCharacterIndex { get; set; }

        /// <summary>
        /// 字体样式
        /// </summary>
        public VTypeface Typeface { get; set; }
    }
}
