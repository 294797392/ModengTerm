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
        
    }

    /// <summary>
    /// 存储文本的一些属性
    /// </summary>
    public class VTextAttribute
    {
        /// <summary>
        /// 字符在某一行的起始位置
        /// </summary>
        public int OffsetX { get; set; }

        /// <summary>
        /// 字符数量，一个多字节字符算一个字符
        /// </summary>
        public int Characters { get; set; }

        /// <summary>
        /// 该文本特性所属的文本行
        /// </summary>
        public VTextLine OwnerLine { get; set; }

        /// <summary>
        /// 该文本所需要的装饰
        /// </summary>
        public List<VTextDecorations> Decorations { get; private set; }

        public VTextAttribute() 
        {
            this.Decorations = new List<VTextDecorations>();
        }
    }
}
