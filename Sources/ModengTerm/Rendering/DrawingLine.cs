using ModengTerm.Terminal;
using ModengTerm.Terminal.Document;
using ModengTerm.Terminal.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using XTerminal;
using XTerminal.Base;
using XTerminal.Parser;

namespace ModengTerm.Rendering
{
    public class DrawingLine : DrawingObject, IDrawingObjectText
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("DrawingLine");

        #endregion

        #region 实例变量

        /// <summary>
        /// 默认文本样式
        /// </summary>
        internal Typeface typeface;

        /// <summary>
        /// 默认的文本前景色
        /// </summary>
        internal Brush foreground;

        /// <summary>
        /// 关联的VTextLine
        /// </summary>
        private VTextLine textLine;

        #endregion

        #region 属性

        /// <summary>
        /// 该行在ViewableDocument里的位置
        /// </summary>
        public int Row { get; private set; }

        #endregion

        #region 构造方法

        public DrawingLine()
        {
            this.ID = "Drawable" + string.Format("{0}", -1).PadLeft(2, '0');
        }

        #endregion

        #region IDrawingObjectText

        protected override void OnInitialize()
        {
            this.textLine = this.documentElement as VTextLine;

            this.typeface = DrawingUtils.GetTypeface(this.textLine.Style);
            this.foreground = DrawingUtils.GetBrush(textLine.Style.ForegroundColor);
        }

        protected override void OnRelease()
        {
        }

        protected override void OnDraw(DrawingContext dc)
        {
            FormattedText formattedText = DrawingUtils.CreateFormattedText(this.textLine, dc);
            DrawingUtils.UpdateTextMetrics(this.textLine, formattedText);
            dc.DrawText(formattedText, DrawingUtils.ZeroPoint);
        }

        /// <summary>
        /// 测量某个文本行的大小
        /// 测量后的结果存储在VTextLine.Metrics属性里
        /// </summary>
        /// <param name="textLine">要测量的数据模型</param>
        /// <returns></returns>
        public void MeasureLine()
        {
            FormattedText formattedText = DrawingUtils.CreateFormattedText(this.textLine);
            DrawingUtils.UpdateTextMetrics(textLine, formattedText);
            textLine.SetMeasureDirty(false);
        }

        /// <summary>
        /// 测量指定文本里的子文本的矩形框
        /// </summary>
        /// <param name="textLine">要测量的数据模型</param>
        /// <param name="startIndex">要测量的起始字符索引</param>
        /// <param name="count">要测量的最大字符数，0为全部测量</param>
        /// <returns></returns>
        public VTRect MeasureTextBlock(int startIndex, int count)
        {
            return CommonMeasureLine(textLine, startIndex, count);
        }

        /// <summary>
        /// 测量一行里某个字符的测量信息
        /// 注意该接口只能测量出来X偏移量，Y偏移量需要外部根据高度自己计算
        /// </summary>
        /// <param name="textLine">要测量的文本行</param>
        /// <param name="characterIndex">要测量的字符</param>
        /// <returns>文本坐标，X=文本左边的X偏移量，Y永远是0，因为边界框是相对于该行的</returns>
        public VTRect MeasureCharacter(int characterIndex)
        {
            return CommonMeasureLine(textLine, characterIndex, 1);
        }

        #endregion

        #region 实例方法

        private static VTRect CommonMeasureLine(VTextLine textLine, int startIndex, int count)
        {
            if (startIndex < 0)
            {
                startIndex = 0;
            }

            int totalChars = textLine.Characters.Count;
            if (startIndex + count > totalChars)
            {
                startIndex = 0;
                count = totalChars;
            }

            if (startIndex == 0 && count == 0)
            {
                return new VTRect();
            }

            FormattedText formattedText = DrawingUtils.CreateFormattedText(textLine);
            Geometry geometry = formattedText.BuildHighlightGeometry(DrawingUtils.ZeroPoint, startIndex, count);
            return new VTRect(geometry.Bounds.Left, geometry.Bounds.Top, geometry.Bounds.Width, geometry.Bounds.Height);
        }

        #endregion
    }
}
