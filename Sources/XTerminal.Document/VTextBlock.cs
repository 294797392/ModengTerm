using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document
{
    public enum DeleteCharacterFrom
    {
        /// <summary>
        /// 从前往后删除字符
        /// </summary>
        FrontToBack,

        /// <summary>
        /// 从后往前删除字符
        /// </summary>
        BackToFront
    }

    public class VTextBlock : VTDocumentElement
    {
        /// <summary>
        /// TextBlock的索引号
        /// </summary>
        public string ID { get; set; }

        ///// <summary>
        ///// 要显示的文本
        ///// </summary>
        //public string Text { get; set; }

        /// <summary>
        /// 字体格式
        /// </summary>
        public VTextStyle Style { get; set; }

        /// <summary>
        /// 该文本块第一个字符所在列数，从0开始
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// 获取该文本块所占据的列数
        /// </summary>
        public int Columns { get; internal set; }

        /// <summary>
        /// 所属行
        /// </summary>
        public VTextLine OwnerLine { get; internal set; }

        /// <summary>
        /// 与WPF关联的画图对象
        /// </summary>
        public object DrawingObject { get; set; }

        public VTextBlock()
        {
        }
    }
}
