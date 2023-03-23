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
    public class XDocumentCanvas : FrameworkElement, IDocumentCanvas
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("DocumentRenderer");

        #endregion

        #region 实例变量

        private VisualCollection visuals;

        private DocumentCanvasOptions options;

        #endregion

        #region 属性

        // Provide a required override for the VisualChildrenCount property.
        protected override int VisualChildrenCount
        {
            get { return this.visuals.Count; }
        }

        #endregion

        #region 构造方法

        public XDocumentCanvas()
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

        private XDocumentDrawable CreateDrawable(Drawables type)
        {
            switch (type)
            {
                case Drawables.Cursor: return new DrawableCursor();
                case Drawables.SelectionRange: return new DrawableSelection();
                case Drawables.TextLine: return new DrawableLine();
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region IDocumentCanvas

        public void Initialize(DocumentCanvasOptions options)
        {
            this.options = options;
        }

        public List<IDocumentDrawable> RequestDrawable(Drawables type, int count)
        {
            List<IDocumentDrawable> drawables = new List<IDocumentDrawable>();

            for (int i = 0; i < count; i++)
            {
                XDocumentDrawable drawable = this.CreateDrawable(type);

                drawables.Add(drawable);

                this.visuals.Add(drawable);
            }

            return drawables;
        }

        public VTElementMetrics MeasureLine(VTextLineBase textLine, int maxCharacters)
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
                Width = formattedText.Width,
                WidthIncludingWhitespace = formattedText.WidthIncludingTrailingWhitespace
            };

            return metrics;
        }

        public VTRect MeasureCharacter(VTextLineBase textLine, int characterIndex)
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

        public void DrawDrawable(IDocumentDrawable drawable)
        {
            XDocumentDrawable drawingVisual = drawable as XDocumentDrawable;
            drawingVisual.Draw();
        }

        public void UpdatePosition(IDocumentDrawable drawable, double offsetX, double offsetY)
        {
            XDocumentDrawable drawingVisual = drawable as XDocumentDrawable;
            drawingVisual.Offset = new Vector(offsetX, offsetY);
        }

        public void SetOpacity(IDocumentDrawable drawable, double opacity)
        {
            XDocumentDrawable drawingVisual = drawable as XDocumentDrawable;
            drawingVisual.Opacity = opacity;
        }

        #endregion
    }
}
