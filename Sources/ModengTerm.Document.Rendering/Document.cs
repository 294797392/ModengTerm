using ModengTerm.Document.Drawing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace ModengTerm.Document.Rendering
{
    /// <summary>
    /// 文档控件
    /// </summary>
    [TemplatePart(Name = "PART_DrawingArea", Type = typeof(DrawingArea))]
    [TemplatePart(Name = "PART_Scrollbar", Type = typeof(ScrollBar))]
    public class Document : Control, IDocument
    {
        #region 实例变量

        /// <summary>
        /// 用来渲染内容的区域
        /// </summary>
        private DrawingArea drawArea;
        private ScrollBar scrollbar;
        private double padding;

        #endregion

        #region 属性

        public DrawingArea DrawArea { get { return this.drawArea; } }

        public VTSize DrawAreaSize
        {
            get
            {
                return new VTSize(this.drawArea.ActualWidth, this.drawArea.ActualHeight);
            }
        }

        public VTScrollbar Scrollbar { get; private set; }

        public bool Visible
        {
            get { return this.Visibility == Visibility.Visible;}
            set
            {
                bool visible = this.Visibility == Visibility.Visible;
                if (visible != value)
                {
                    if (value)
                    {
                        this.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        #endregion

        #region 构造方法

        public Document()
        {
            this.Style = Application.Current.FindResource("StyleWPFDocument") as Style;
        }

        #endregion

        #region IDocument

        public void SetPadding(double padding)
        {
            if (this.padding == padding) 
            {
                return;
            }

            this.padding = padding;
            base.Padding = new Thickness(padding);
        }

        public IDocumentObject CreateDrawingObject()
        {
            DrawingObject drawingObject = new DrawingObject();

            this.drawArea.AddVisual(drawingObject);

            return drawingObject;
        }

        public void DeleteDrawingObject(IDocumentObject drawingObject)
        {
            drawArea.RemoveVisual(drawingObject as DrawingObject);
        }

        public void DeleteDrawingObjects()
        {
            List<IDocumentObject> drawingObjects = drawArea.GetAllVisual().Cast<IDocumentObject>().ToList();

            foreach (IDocumentObject drawingObject in drawingObjects)
            {
                DeleteDrawingObject(drawingObject);
            }
        }

        public VTypeface GetTypeface(double fontSize, string fontFamily)
        {
            Typeface typeface = DrawingUtils.GetTypeface(fontFamily);
            FormattedText formattedText = new FormattedText(" ", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface,
                fontSize, Brushes.Black, null, TextFormattingMode.Display, DrawingUtils.PixelPerDpi);

            return new VTypeface()
            {
                FontSize = fontSize,
                FontFamily = fontFamily,
                Height = formattedText.Height,
                Width = formattedText.WidthIncludingTrailingWhitespace
            };
        }

        #endregion

        #region 重写方法

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.drawArea = base.Template.FindName("PART_DrawingArea", this) as DrawingArea;
            this.scrollbar = base.Template.FindName("PART_Scrollbar", this) as ScrollBar;
            this.Scrollbar = new VTScrollbarImpl(this.scrollbar);
        }

        #endregion
    }
}
