using ModengTerm.Document;
using ModengTerm.Document.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace ModengTerm.UserControls.TerminalUserControls
{
    /// <summary>
    /// 终端显示控件
    /// </summary>
    [TemplatePart(Name = "PART_DrawingArea", Type = typeof(DrawingArea))]
    [TemplatePart(Name = "PART_Scrollbar", Type = typeof(ScrollBar))]
    public class TerminalControl : Control, GraphicsFactory
    {
        #region 类变量

        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("TerminalControl");

        #endregion

        #region 公开事件

        public event Action<GraphicsFactory> GILoaded;

        #endregion

        #region 实例变量

        /// <summary>
        /// 用来渲染内容的区域
        /// </summary>
        private DrawingArea drawArea;
        private ScrollBar scrollbar;
        private DrawingObjectScrollbar scrollbarDrawingObject;

        #endregion

        #region 属性

        public DrawingArea DrawArea { get { return this.drawArea; } }

        public ScrollBar Scrollbar { get { return this.scrollbar; } }

        public VTSize TerminalSize
        {
            get
            {
                return new VTSize(this.drawArea.ActualWidth, this.drawArea.ActualHeight);
            }
        }

        #endregion

        #region 构造方法

        public TerminalControl()
        {
            this.Style = Application.Current.FindResource("StyleTerminalControl") as Style;
        }

        #endregion

        #region 实例方法

        #endregion

        #region GraphicsFactory

        public GraphicsScrollbar GetScrollbarDrawingObject() 
        {
            return this.scrollbarDrawingObject;
        }

        public GraphicsObject CreateDrawingObject()
        {
            DrawingObject drawingObject = new DrawingObject();

            this.drawArea.Visuals.Add(drawingObject);

            return drawingObject;
        }

        public void DeleteDrawingObject(GraphicsObject drawingObject)
        {
            drawArea.Visuals.Remove(drawingObject as DrawingObject);
        }

        public void DeleteDrawingObjects()
        {
            List<GraphicsObject> drawingObjects = drawArea.Visuals.Cast<GraphicsObject>().ToList();

            foreach (GraphicsObject drawingObject in drawingObjects)
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

        #region 事件处理器

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.drawArea = base.Template.FindName("PART_DrawingArea", this) as DrawingArea;
            this.scrollbar = base.Template.FindName("PART_Scrollbar", this) as ScrollBar;
            this.scrollbarDrawingObject = new DrawingObjectScrollbar(this.scrollbar);
        }

        #endregion
    }
}
