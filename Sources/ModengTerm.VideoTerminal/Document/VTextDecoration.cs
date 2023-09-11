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
    public enum VTextDecorationEnum
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
        Foreground,
    }

    /// <summary>
    /// 存储文本的一些属性
    /// VTextDecoration和Row，Column的关系是，一个Cell可以包含一组VTextDecoration
    /// </summary>
    public class VTextDecoration : Reusable<VTextDecoration>
    {
        /// <summary>
        /// 该装饰的起始列，渲染的时候要渲染该列
        /// 从0开始计数
        /// </summary>
        public int StartColumn { get; set; }

        /// <summary>
        /// 该装饰的结束列，渲染的时候要渲染该列
        /// man模式下，有可能一行里面有多个下划线，如果只保存StartColumn，WPF不容易进行渲染，所以保存下EndColumn
        /// </summary>
        public int EndColumn { get; set; }

        /// <summary>
        /// 是否形成闭环（EndColumn不为空）
        /// 如果没形成闭环，那么渲染整行文本
        /// 如果形成闭环，那么只渲染StartColumn到EndColumn之间的文本
        /// </summary>
        public bool Closed { get; set; }

        /// <summary>
        /// 该文本的装饰
        /// </summary>
        public VTextDecorationEnum Decoration { get; set; }

        /// <summary>
        /// 装饰对应的参数
        /// </summary>
        public object Parameter { get; set; }

        /// <summary>
        /// 请使用VTextAttribute.Create创建实例
        /// </summary>
        private VTextDecoration()
        {
        }

        public override void CopyTo(VTextDecoration dest)
        {
            dest.StartColumn = this.StartColumn;
            dest.EndColumn = this.EndColumn;
            dest.Closed = this.Closed;
            dest.Decoration = this.Decoration;
            dest.Parameter = this.Parameter;
        }

        public override void SetDefault()
        {
            this.StartColumn = 0;
            this.EndColumn = 0;
            this.Closed = false;
            this.Decoration = (VTextDecorationEnum)0;
            this.Parameter = null;
        }
    }
}
