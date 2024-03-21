using ModengTerm.Document.Drawing;
using ModengTerm.Terminal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace ModengTerm.Document.Rendering
{
    public class DrawingLine : DrawingObject, IDrawingTextLine
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("DrawingLine");

        #endregion

        #region 实例变量

        #endregion

        #region 属性

        public VTFormattedText FormattedText { get; set; }

        /// <summary>
        /// 该行在Document里的位置
        /// </summary>
        public int Row { get; private set; }

        #endregion

        #region 构造方法

        public DrawingLine()
        {
            this.ID = "Drawable" + string.Format("{0}", -1).PadLeft(2, '0');
        }

        #endregion

        #region IDrawingTextLine

        protected override void OnInitialize()
        {
        }

        protected override void OnRelease()
        {
        }

        protected override void OnDraw(DrawingContext dc)
        {
            FormattedText formattedText = DrawingUtils.CreateFormattedText(this.FormattedText, dc);
            dc.DrawText(formattedText, DrawingUtils.ZeroPoint);
        }

        /// <summary>
        /// 测量某个文本行的大小
        /// 测量后的结果存储在VTextLine.Metrics属性里
        /// </summary>
        /// <param name="textLine">要测量的数据模型</param>
        /// <returns></returns>
        public VTextMetrics Measure()
        {
            FormattedText formattedText = DrawingUtils.CreateFormattedText(this.FormattedText);
            return new VTextMetrics()
            {
                Width = formattedText.WidthIncludingTrailingWhitespace,
                Height = formattedText.Height
            };
        }

        /// <summary>
        /// 测量指定文本里的子文本的矩形框
        /// </summary>
        /// <param name="textLine">要测量的数据模型</param>
        /// <param name="startIndex">要测量的起始字符索引</param>
        /// <param name="count">要测量的最大字符数，0为全部测量</param>
        /// <returns></returns>
        public VTextRange MeasureTextRange(int startIndex, int count)
        {
            return CommonMeasureLine(this.FormattedText, this.Offset.Y, startIndex, count);
        }

        #endregion

        #region 实例方法

        private static VTextRange CommonMeasureLine(VTFormattedText textData, double offsetY, int startIndex, int count)
        {
            if (textData == null)
            {
                return new VTextRange();
            }

            if (startIndex < 0)
            {
                startIndex = 0;
            }

            int totalChars = textData.Text.Length;
            if (startIndex + count > totalChars)
            {
                startIndex = 0;
                count = totalChars;
            }

            if (startIndex == 0 && count == 0)
            {
                return new VTextRange();
            }

            FormattedText formattedText = DrawingUtils.CreateFormattedText(textData);
            System.Windows.Media.Geometry geometry = formattedText.BuildHighlightGeometry(DrawingUtils.ZeroPoint, startIndex, count);
            return new VTextRange(geometry.Bounds.Left, offsetY, geometry.Bounds.Width, geometry.Bounds.Height);
        }

        #endregion
    }
}
