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
    /// WPF实现的文档渲染器
    /// </summary>
    [TemplatePart(Name = "PART_DocumentCanvas", Type = typeof(WPFDocumentCanvas))]
    [TemplatePart(Name = "PART_Scrollbar", Type = typeof(ScrollBar))]
    public class WPFDocument : Control, IDocumentRenderer
    {
        #region 实例变量

        /// <summary>
        /// 用来渲染内容的区域
        /// </summary>
        private WPFDocumentCanvas content;
        private ScrollBar scrollbar;
        private double contentMargin;

        #endregion

        #region 属性

        public WPFDocumentCanvas Content { get { return this.content; } }

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

        public VTSize ContentSize
        {
            get
            {
                return new VTSize(this.content.ActualWidth, this.content.ActualHeight);
            }
        }

        public double ContentMargin
        {
            get { return this.contentMargin; }
            set
            {
                if (this.contentMargin != value)
                {
                    this.contentMargin = value;
                    this.content.Margin = new Thickness(value);
                }
            }
        }

        #endregion

        #region 构造方法

        public WPFDocument()
        {
            this.Style = Application.Current.FindResource("StyleWPFDocument") as Style;
        }

        #endregion

        #region IDocumentRenderer

        public IDrawingObject CreateDrawingObject()
        {
            DrawingObject drawingObject = new DrawingObject();

            this.content.AddVisual(drawingObject);

            return drawingObject;
        }

        public void DeleteDrawingObject(IDrawingObject drawingObject)
        {
            content.RemoveVisual(drawingObject as DrawingObject);
        }

        public void DeleteDrawingObjects()
        {
            List<IDrawingObject> drawingObjects = content.GetAllVisual().Cast<IDrawingObject>().ToList();

            foreach (IDrawingObject drawingObject in drawingObjects)
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

            this.content = base.Template.FindName("PART_DocumentCanvas", this) as WPFDocumentCanvas;
            this.scrollbar = base.Template.FindName("PART_Scrollbar", this) as ScrollBar;
            this.Scrollbar = new VTScrollbarImpl(this.scrollbar);
        }

        #endregion
    }
}
