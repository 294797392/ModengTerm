using ModengTerm.Document.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document
{
    public class VTParagraphOptions
    {
        /// <summary>
        /// 第一行的物理行号
        /// </summary>
        public int FirstPhysicsRow { get; set; }

        /// <summary>
        /// 最后一行的物理行号
        /// </summary>
        public int LastPhysicsRow { get; set; }

        /// <summary>
        /// 第一行的起始列数
        /// </summary>
        public int StartColumn { get; set; }

        /// <summary>
        /// 最后一行的结束列
        /// </summary>
        public int EndColumn { get; set; }

        /// <summary>
        /// 段落格式
        /// </summary>
        public ParagraphFormatEnum FormatType { get; set; }
    }

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
        /// 该段落的纯文件数据，包含换行符
        /// </summary>
        public string Content { get; set; }

        public VTParagraph()
        {
        }
    }
}







