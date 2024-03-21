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
    [TemplatePart(Name = "PART_DrawingSurface", Type = typeof(DrawingSurface))]
    [TemplatePart(Name = "PART_Scrollbar", Type = typeof(ScrollBar))]
    public class DrawingDocument : Control, IDrawingDocument
    {
        #region 实例变量

        private DrawingSurface surface;
        private ScrollBar scrollbar;

        #endregion

        #region 属性

        /// <summary>
        /// 用来渲染内容的区域
        /// </summary>
        public DrawingSurface ContentArea { get { return this.surface; } }

        public VTScrollbar Scrollbar { get; private set; }

        public VTSize Size
        {
            get
            {
                return new VTSize(this.ActualWidth, this.ActualHeight);
            }
        }

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

        public DrawingDocument()
        {
            this.Style = Application.Current.FindResource("StyleDrawingDocument") as Style;
        }

        #endregion

        #region IDrawingDocument

        public IDrawingObject CreateDrawingObject(DrawingObjectTypes types)
        {
            IDrawingObject drawingObject = DrawingObjectFactory.CreateDrawingObject(types);

            this.surface.AddVisual(drawingObject as DrawingObject);

            return drawingObject;
        }

        public void DeleteDrawingObject(IDrawingObject drawingObject)
        {
            drawingObject.Release();
            surface.RemoveVisual(drawingObject as DrawingObject);
        }

        public void DeleteDrawingObjects()
        {
            List<IDrawingObject> drawingObjects = surface.GetAllVisual().Cast<IDrawingObject>().ToList();

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
                SpaceWidth = formattedText.WidthIncludingTrailingWhitespace
            };
        }

        #endregion

        #region 重写方法

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.surface = base.Template.FindName("PART_DrawingSurface", this) as DrawingSurface;
            this.scrollbar = base.Template.FindName("PART_Scrollbar", this) as ScrollBar;
            this.Scrollbar = new VTScrollbarImpl(this.scrollbar);
        }

        #endregion
    }
}
