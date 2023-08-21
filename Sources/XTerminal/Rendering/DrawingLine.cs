using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using XTerminal.Base;
using XTerminal.Document;
using XTerminal.Document.Rendering;
using XTerminal.Parser;

namespace XTerminal.Rendering
{
    public class DrawingLine : DrawingObject, IDrawingObjectText
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("DrawingLine");

        private static readonly Point ZeroPoint = new Point();

        #endregion

        #region 实例变量

        internal Typeface typeface;
        internal Brush foreground;

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
            Geometry geometry = formattedText.BuildHighlightGeometry(ZeroPoint, startIndex, count);
            return new VTRect(geometry.Bounds.Left, geometry.Bounds.Top, geometry.Bounds.Width, geometry.Bounds.Height);
        }

        #region IDrawingObjectText

        protected override void OnInitialize(VTDocumentElement documentElement)
        {
            this.textLine = documentElement as VTextLine;

            FontFamily fontFamily = new FontFamily(textLine.Style.FontFamily);
            this.typeface = new Typeface(fontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);

            this.foreground = DrawingUtils.ConvertBrush(textLine.Style.Foreground);
        }

        protected override void Draw(DrawingContext dc)
        {
            FormattedText formattedText = DrawingUtils.CreateFormattedText(this.textLine);

            this.Offset = new Vector(0, this.textLine.OffsetY);

            DrawingUtils.UpdateTextMetrics(this.textLine, formattedText);

            foreach (VTextAttribute textAttribute in this.textLine.Attributes)
            {
                switch (textAttribute.Decoration)
                {
                    case VTextDecorations.Foreground:
                        {
                            if (textAttribute.Parameter == null)
                            {
                                continue;
                            }

                            int startIndex = this.textLine.FindCharacterIndex(textAttribute.StartColumn);
                            int endIndex = this.textLine.FindCharacterIndex(textAttribute.EndColumn - 1);
                            if (startIndex == -1 || endIndex == -1)
                            {
                                continue;
                            }

                            logger.ErrorFormat("startIndex = {0}, endIndex = {1}", startIndex, endIndex);

                            VTColors color = (VTColors)textAttribute.Parameter;
                            Brush brush = DrawingUtils.VTColor2Brush(color);
                            formattedText.SetForegroundBrush(brush, startIndex, endIndex - startIndex + 1);
                            break;
                        }

                    default:
                        break;
                }
            }

            //// 遍历链表，给每个TextBlock设置样式
            //VTextBlock current = this.TextLine.First;

            //while (current != null)
            //{
            //    // TODO：设置样式

            //    current = current.Next;
            //}

            dc.DrawText(formattedText, new Point());
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
        public VTRect MeasureLine(int startIndex, int count)
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
    }
}
