using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document
{
    /// <summary>
    /// 存储格式化后的文本数据
    /// </summary>
    public class VTFormattedText
    {
        /// <summary>
        /// 文本字符串
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 文本属性列表
        /// </summary>
        public List<VTextAttribute> Attributes { get; private set; }

        /// <summary>
        /// X偏移量，可选参数
        /// </summary>
        public double OffsetX { get; set; }

        /// <summary>
        /// Y偏移量，可选参数
        /// </summary>
        public double OffsetY { get; set; }

        /// <summary>
        /// 默认文本样式
        /// </summary>
        public VTypeface Style { get; set; }

        public VTFormattedText()
        {
            Text = string.Empty;
            Attributes = new List<VTextAttribute>();
        }
    }
}
