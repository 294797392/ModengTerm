using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document
{
    /// <summary>
    /// 存储文档里的一个段落信息
    /// </summary>
    public class VTParagraph
    {
        /// <summary>
        /// 表示一个空的段落
        /// </summary>
        public static readonly VTParagraph Empty = new VTParagraph();

        /// <summary>
        /// 判断该段落是否为空
        /// </summary>
        public bool IsEmpty { get { return string.IsNullOrEmpty(Content); } }

        /// <summary>
        /// 该段落的创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 该段落的纯文件数据，包含换行符
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 该段落的字符列表
        /// </summary>
        public List<VTHistoryLine> CharacterList { get; set; }

        /// <summary>
        /// 段落的起始行的第一个字符索引
        /// </summary>
        public int StartCharacterIndex { get; set; }

        /// <summary>
        /// 段落的结束行的最后一个字符索引
        /// </summary>
        public int EndCharacterIndex { get; set; }

        /// <summary>
        /// 是否是备用缓冲区里的内容
        /// </summary>
        public bool IsAlternate { get; set; }

        public VTParagraph()
        {
        }
    }
}







