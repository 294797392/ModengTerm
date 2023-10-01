using ModengTerm.Base;
using ModengTerm.Terminal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document
{
    /// <summary>
    /// 定义文本装饰
    /// 按位组合
    /// </summary>
    public enum VTextAttributes
    {
        /// <summary>
        /// 加粗字体
        /// </summary>
        Bold = 0,

        /// <summary>
        /// 下划线
        /// </summary>
        Underline = 1,

        /// <summary>
        /// 斜体
        /// </summary>
        Italics = 2,

        /// <summary>
        /// 双下划线
        /// </summary>
        DoublyUnderlined = 3,

        /// <summary>
        /// 字体背景颜色
        /// </summary>
        Background = 4,

        /// <summary>
        /// 字体颜色
        /// </summary>
        Foreground = 5
    }

    /// <summary>
    /// 0 0 0 0 0 0
    /// </summary>
    public class VTextAttribute
    {
        /// <summary>
        /// 文本属性的字符起始索引
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        /// 文本属性的字符数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 文本属性类型
        /// </summary>
        public VTextAttributes Attribute { get; set; }

        /// <summary>
        /// 属性参数
        /// </summary>
        public object Parameter { get; set; }

        /// <summary>
        /// 是否结束
        /// 渲染的使用用不到，创建VTFormattedText的时候用到
        /// </summary>
        public bool Closed { get; set; }
    }

    public class VTextAttributeState
    {
        /// <summary>
        /// 按位存储的值
        /// </summary>
        public int Value;

        public VTColor Background { get; set; }

        public VTColor Foreground { get; set; }

        public VTextAttributeState()
        {
        }

        public void Clear()
        {
            this.Value = 0;
            this.Background = null;
            this.Foreground = null;
        }
    }
}
