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
        private static readonly Queue<VTextAttribute> CharacterQueue = new Queue<VTextAttribute>();

        /// <summary>
        /// 该装饰的起始列
        /// 从0开始计数
        /// </summary>
        public int StartColumn { get; set; }

        /// <summary>
        /// 该装饰的结束列
        /// 从0开始计数
        /// </summary>
        public int EndColumn { get; set; }

        /// <summary>
        /// 该文本的装饰
        /// </summary>
        public VTextDecorations Decoration { get; set; }

        /// <summary>
        /// 装饰对应的参数
        /// </summary>
        public object Parameter { get; set; }

        public bool Unset { get; set; }

        public VTextAttribute() 
        {
        }
    }
}
