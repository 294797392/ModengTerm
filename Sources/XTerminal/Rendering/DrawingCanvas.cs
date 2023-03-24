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
    /// 用来显示字符的容器
    /// </summary>
    public class DrawingCanvas : FrameworkElement, IDrawingCanvas
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("DocumentRenderer");

        #endregion

        #region 实例变量

        private VisualCollection visuals;

        private DrawingCanvasOptions options;

        #endregion

        #region 属性

        // Provide a required override for the VisualChildrenCount property.
        protected override int VisualChildrenCount
        {
            get { return this.visuals.Count; }
        }

        #endregion

        #region 构造方法

        public DrawingCanvas()
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

        private DrawingObject CreateDrawable(Drawables type)
        {
            switch (type)
            {
                case Drawables.Cursor: return new DrawingCursor();
                case Drawables.SelectionRange: return new DrawingSelection();
                case Drawables.TextLine: return new DrawingLine();
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region IDocumentCanvas

        public void Initialize(DrawingCanvasOptions options)
        {
            this.options = options;
        }

        public List<IDrawingObject> RequestDrawable(Drawables type, int count)
        {
            List<IDrawingObject> drawables = new List<IDrawingObject>();

            for (int i = 0; i < count; i++)
            {
                DrawingObject drawable = this.CreateDrawable(type);

                drawables.Add(drawable);

                this.visuals.Add(drawable);
            }

            return drawables;
        }

        public VTElementMetrics MeasureLine(ITextLine textLine, int maxCharacters)
        {
            string text = textLine.Text;
            if (maxCharacters > 0 && text.Length >= maxCharacters)
            {
                text = text.Substring(0, maxCharacters);
            }

            Typeface typeface = WPFRenderUtils.GetTypeface(VTextStyle.Default);
            FormattedText formattedText = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, VTextStyle.Default.FontSize, Brushes.Black, null, TextFormattingMode.Display, App.PixelsPerDip);

            VTElementMetrics metrics = new VTElementMetrics()
            {
                Height = formattedText.Height,
                Width = formattedText.WidthIncludingTrailingWhitespace,
            };

            return metrics;
        }

        public VTRect MeasureCharacter(ITextLine textLine, int characterIndex)
        {
            Typeface typeface = WPFRenderUtils.GetTypeface(VTextStyle.Default);

            string text = textLine.Text;
            if (characterIndex == 0)
            {
                // 第一个字符，返回第一个字符的左边
                FormattedText formattedText = new FormattedText(text.Substring(0, 1), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, VTextStyle.Default.FontSize, Brushes.Black, null, TextFormattingMode.Display, App.PixelsPerDip);
                return new VTRect(0, 0, formattedText.WidthIncludingTrailingWhitespace, formattedText.Height);
            }
            else
            {
                // 其他字符，返回前一个字符的右边
                FormattedText formattedText1 = new FormattedText(text.Substring(0, characterIndex), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, VTextStyle.Default.FontSize, Brushes.Black, null, TextFormattingMode.Display, App.PixelsPerDip);
                FormattedText formattedText2 = new FormattedText(text.Substring(characterIndex, 1), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, VTextStyle.Default.FontSize, Brushes.Black, null, TextFormattingMode.Display, App.PixelsPerDip);
                return new VTRect(formattedText1.WidthIncludingTrailingWhitespace, 0, formattedText2.WidthIncludingTrailingWhitespace, formattedText2.Height);
            }
        }

        public void DrawDrawable(IDrawingObject drawable)
        {
            DrawingObject drawingVisual = drawable as DrawingObject;
            drawingVisual.Draw();
        }

        public void UpdatePosition(IDrawingObject drawable, double offsetX, double offsetY)
        {
            DrawingObject drawingVisual = drawable as DrawingObject;
            drawingVisual.Offset = new Vector(offsetX, offsetY);
        }

        public void SetOpacity(IDrawingObject drawable, double opacity)
        {
            DrawingObject drawingVisual = drawable as DrawingObject;
            drawingVisual.Opacity = opacity;
        }

        #endregion
    }
}
