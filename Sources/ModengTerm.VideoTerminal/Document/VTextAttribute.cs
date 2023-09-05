using ModengTerm.Base;
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
        BoldUnset,

        /// <summary>
        /// 下划线
        /// </summary>
        Underline,
        UnderlineUnset,

        /// <summary>
        /// 斜体
        /// </summary>
        Italics,
        ItalicsUnset,

        /// <summary>
        /// 双下划线
        /// </summary>
        DoublyUnderlined,
        DoublyUnderlinedUnset,

        /// <summary>
        /// 字体背景颜色
        /// </summary>
        Background,
        BackgroundUnset,

        /// <summary>
        /// 字体颜色
        /// </summary>
        Foreground,
        ForegroundUnset,

        /// <summary>
        /// 用RGB设置前景色
        /// </summary>
        ForegroundRGB,

        /// <summary>
        /// 用RGB设置背景色
        /// </summary>
        BackgroundRGB,
    }

    /// <summary>
    /// 存储文本的一些属性
    /// </summary>
    public class VTextAttribute : Reusable<VTextAttribute>
    {
        /// <summary>
        /// 该装饰的起始列，包含该列
        /// 从0开始计数
        /// </summary>
        public int StartColumn { get; set; }

        /// <summary>
        /// 该文本的装饰
        /// </summary>
        public VTextDecorations Decoration { get; set; }

        /// <summary>
        /// 装饰对应的参数
        /// </summary>
        public object Parameter { get; set; }

        /// <summary>
        /// 请使用VTextAttribute.Create创建实例
        /// </summary>
        private VTextAttribute()
        {
        }

        public override void CopyTo(VTextAttribute dest)
        {
            dest.StartColumn = this.StartColumn;
            dest.Decoration = this.Decoration;
            dest.Parameter = this.Parameter;
        }

        public override void SetDefault()
        {
            this.StartColumn = 0;
            this.Decoration = (VTextDecorations)0;
            this.Parameter = null;
        }
    }
}
