using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using XTerminal.Base;
using XTerminal.Document;

namespace XTerminal.Rendering
{
    public class DrawingLine : DrawingObject
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("DrawingLine");

        #endregion

        #region 实例变量

        internal Typeface typeface;
        internal Brush foreground;

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

        #region DrawingObject

        protected override void OnInitialize(VTDocumentElement element)
        {
            VTextLine textLine = element as VTextLine;

            FontFamily fontFamily = new FontFamily(textLine.Style.FontFamily);
            this.typeface = new Typeface(fontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);

            this.foreground = DrawingUtils.ConvertBrush(textLine.Style.Foreground);
        }

        protected override void Draw(DrawingContext dc)
        {
            VTextLine textLine = this.DocumentElement as VTextLine;

            FormattedText formattedText = DrawingUtils.CreateFormattedText(textLine);

            this.Offset = new Vector(0, textLine.OffsetY);

            DrawingUtils.UpdateTextMetrics(textLine, formattedText);

            //// 遍历链表，给每个TextBlock设置样式
            //VTextBlock current = this.TextLine.First;

            //while (current != null)
            //{
            //    // TODO：设置样式

            //    current = current.Next;
            //}

            dc.DrawText(formattedText, new Point());
        }

        #endregion
    }
}
