using ModengTerm.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document.Drawing
{
    /// <summary>
    /// 在DrawingObejct的基础上新增了测量文本的接口
    /// </summary>
    public interface IDrawingTextLine : IDrawingObject
    {
        /// <summary>
        /// 要渲染的文本
        /// </summary>
        VTFormattedText FormattedText { get; set; }

        /// <summary>
        /// 测量某个文本行的大小
        /// 测量后的结果存储在VTextLine.Metrics属性里
        /// </summary>
        /// <returns></returns>
        VTextMetrics Measure();

        /// <summary>
        /// 测量指定文本里的子文本的矩形框
        /// </summary>
        /// <param name="startIndex">要测量的起始字符索引</param>
        /// <param name="count">要测量的最大字符数，0为全部测量</param>
        /// <returns></returns>
        VTextRange MeasureTextRange(int startIndex, int count);
    }
}