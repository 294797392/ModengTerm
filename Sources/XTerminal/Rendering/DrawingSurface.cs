using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using VideoTerminal.Rendering;
using XTerminal.Document;
using XTerminal.Document.Rendering;

namespace XTerminal.Rendering
{
    /// <summary>
    /// 用来渲染终端输出的表面
    /// </summary>
    public class DrawingSurface : FrameworkElement, ITerminalSurface
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("DocumentRenderer");

        #endregion

        #region 实例变量

        private VisualCollection visuals;

        private TerminalSurfaceOptions options;

        #endregion

        #region 属性

        // Provide a required override for the VisualChildrenCount property.
        protected override int VisualChildrenCount
        {
            get { return this.visuals.Count; }
        }

        #endregion

        #region 构造方法

        public DrawingSurface()
        {
            this.visuals = new VisualCollection(this);
        }

        #endregion

        #region 重写方法

        // Provide a required override for the GetVisualChild method.
        protected override Visual GetVisualChild(int index)
        {
            return this.visuals[index];
        }

        #endregion

        #region 实例方法

        private DrawingObject CreateDrawingObject(VTDocumentElements type)
        {
            switch (type)
            {
                case VTDocumentElements.Cursor: return new DrawingCursor();
                case VTDocumentElements.SelectionRange: return new DrawingSelection();
                case VTDocumentElements.TextLine: return new DrawingLine();
                default:
                    throw new NotImplementedException();
            }
        }

        private DrawingObject EnsureDrawingObject(VTDocumentElement drawable)
        {
            DrawingObject drawingObject = drawable.DrawingContext as DrawingObject;
            if (drawingObject == null)
            {
                drawingObject = this.CreateDrawingObject(drawable.Type);
                drawingObject.Drawable = drawable;
                drawable.DrawingContext = drawingObject;
                this.visuals.Add(drawingObject);
            }
            return drawingObject;
        }

        private FormattedText CreateFormattedText(string text, IEnumerable<VTCharacter> characters)
        {
            Typeface typeface = WPFRenderUtils.GetTypeface(VTextStyle.Default);
            FormattedText formattedText = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, VTextStyle.Default.FontSize, Brushes.Black, null, TextFormattingMode.Display, App.PixelsPerDip);
            return formattedText;
        }

        #endregion

        #region ITerminalSurface

        public void Initialize(TerminalSurfaceOptions options)
        {
            this.options = options;
        }

        public void MeasureLine(VTextLine textLine)
        {
            string text = textLine.Text;

            FormattedText formattedText = this.CreateFormattedText(text, textLine.Characters);

            textLine.Metrics.Height = formattedText.Height;
            textLine.Metrics.Width = formattedText.WidthIncludingTrailingWhitespace;

            textLine.SetMeasureDirty(false);
        }

        /// <summary>
        /// 测量某个渲染模型的大小
        /// TODO：如果测量的是字体，要考虑到对字体应用样式后的测量信息
        /// </summary>
        /// <param name="textLine">要测量的数据模型</param>
        /// <param name="maxCharacters">要测量的最大字符数，0为全部测量</param>
        /// <returns></returns>
        public VTSize MeasureBlock(VTextLine textLine, int maxCharacters)
        {
            string text = textLine.Text;
            if (maxCharacters > 0 && text.Length >= maxCharacters)
            {
                text = text.Substring(0, maxCharacters);
            }

            FormattedText formattedText = this.CreateFormattedText(text, textLine.Characters);

            return new VTSize(formattedText.WidthIncludingTrailingWhitespace, formattedText.Height);
        }

        public VTRect MeasureCharacter(VTHistoryLine textLine, int characterIndex)
        {
            string text = textLine.Text;

            if (characterIndex == 0)
            {
                // 第一个字符，返回第一个字符的左边
                string textToMeasure = text.Substring(0, 1);
                FormattedText formattedText = this.CreateFormattedText(textToMeasure, textLine.Characters);
                return new VTRect(0, 0, formattedText.WidthIncludingTrailingWhitespace, formattedText.Height);
            }
            else
            {
                // 其他字符，返回前一个字符的右边
                string textToMeasure1 = text.Substring(0, characterIndex);
                string textToMeasure2 = text.Substring(characterIndex, 1);
                FormattedText formattedText1 = this.CreateFormattedText(textToMeasure1, textLine.Characters);
                FormattedText formattedText2 = this.CreateFormattedText(textToMeasure2, textLine.Characters);
                return new VTRect(formattedText1.WidthIncludingTrailingWhitespace, 0, formattedText2.WidthIncludingTrailingWhitespace, formattedText2.Height);
            }
        }

        public void Draw(VTDocumentElement drawable)
        {
            DrawingObject drawingObject = this.EnsureDrawingObject(drawable);
            drawingObject.Draw();
            drawable.SetRenderDirty(false);
        }

        public void Arrange(VTDocumentElement drawable)
        {
            DrawingObject drawingObject = this.EnsureDrawingObject(drawable);
            drawingObject.Offset = new Vector(drawable.OffsetX, drawable.OffsetY);
        }

        public void SetOpacity(VTDocumentElement drawable, double opacity)
        {
            DrawingObject drawingObject = this.EnsureDrawingObject(drawable);
            drawingObject.Opacity = opacity;
        }

        public VTRect GetRectRelativeToDesktop()
        {
            Point leftTop = this.PointToScreen(new Point(0, 0));
            return new VTRect(leftTop.X, leftTop.Y, this.ActualWidth, this.ActualHeight);
        }

        #endregion
    }
}
