using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document
{
    /// <summary>
    /// 定义文本装饰
    /// </summary>
    public enum VTextDecorations
    {
        /// <summary>
        /// 加粗字体
        /// </summary>
        Bold,

        /// <summary>
        /// 下划线
        /// </summary>
        Underline,

        /// <summary>
        /// 斜体
        /// </summary>
        Italics,

        /// <summary>
        /// 双下划线
        /// </summary>
        DoublyUnderlined,

        /// <summary>
        /// 字体背景颜色
        /// </summary>
        Background,

        /// <summary>
        /// 字体颜色
        /// </summary>
        Foreground
    }

    /// <summary>
    /// 存储文本的一些属性
    /// </summary>
    public class VTextAttribute
    {
        /// <summary>
        /// 该装饰的起始字符索引
        /// </summary>
        public int StartCharacter { get; set; }

        /// <summary>
        /// 该装饰的结束字符索引
        /// </summary>
        public int EndCharacter { get; set; }

        /// <summary>
        /// 字符数量，一个多字节字符算一个字符
        /// </summary>
        public int Characters { get { return this.EndCharacter - this.StartCharacter; } }

        /// <summary>
        /// 该文本的装饰
        /// </summary>
        public VTextDecorations Decoration { get; set; }

        /// <summary>
        /// 该属性是否已设置完成
        /// </summary>
        public bool Completed { get; set; }

        public VTextAttribute() 
        {
        }
    }
}
