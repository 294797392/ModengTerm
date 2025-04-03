using ModengTerm.Document;
using ModengTerm.Document.Drawing;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace ModengTerm.UserControls.TerminalUserControls.Rendering
{
    /// <summary>
    /// 文档控件
    /// </summary>
    [TemplatePart(Name = "PART_DrawingArea", Type = typeof(DrawingArea))]
    [TemplatePart(Name = "PART_Scrollbar", Type = typeof(ScrollBar))]
    public class DocumentControl : Control, GraphicsInterface
    {
        #region 类变量

        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("DocumentControl");

        #endregion

        #region 公开事件

        public event Action<GraphicsInterface, MouseData> GIMouseDown;
        public event Action<GraphicsInterface, MouseData> GIMouseUp;
        public event Action<GraphicsInterface, MouseData> GIMouseMove;
        public event Action<GraphicsInterface, MouseWheelData> GIMouseWheel;
        public event Action<GraphicsInterface> GILoaded;
        public event Action<GraphicsInterface, ScrollChangedData> GIScrollChanged;

        #endregion

        #region 实例变量

        /// <summary>
        /// 用来渲染内容的区域
        /// </summary>
        private DrawingArea drawArea;
        private ScrollBar scrollbar;
        private double padding;
        private MouseData mouseDownData;
        private MouseData mouseUpData;
        private MouseData mouseMoveData;
        private MouseWheelData mouseWheelData;
        private ScrollChangedData scrollChangedData;

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
            get { return this.Visibility == Visibility.Visible; }
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

        public bool GIMouseCaptured
        {
            get { return this.IsMouseCaptured; }
        }

        public VTModifierKeys PressedModifierKey { get { return DrawingUtils.ConvertToVTModifierKeys(Keyboard.Modifiers); } }

        #endregion

        #region 构造方法

        public DocumentControl()
        {
            this.Style = Application.Current.FindResource("StyleDocumentControl") as Style;
            this.mouseDownData = new MouseData();
            this.mouseUpData = new MouseData();
            this.mouseMoveData = new MouseData();
            this.mouseWheelData = new MouseWheelData();
            this.scrollChangedData = new ScrollChangedData();
            this.Loaded += DocumentControl_Loaded;
        }

        #endregion

        #region 实例方法

        private int GetScrollValue()
        {
            var newvalue = (int)Math.Round(this.scrollbar.Value, 0);
            if (newvalue > this.scrollbar.Maximum)
            {
                newvalue = (int)Math.Round(this.scrollbar.Maximum, 0);
            }
            return newvalue;
        }

        private VTMouseButton MouseButton2VTMouseButton(MouseButton mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButton.Left:
                    {
                        return VTMouseButton.LeftButton;
                    }

                case MouseButton.Right:
                    {
                        return VTMouseButton.RightButton;
                    }

                case MouseButton.Middle:
                    {
                        return VTMouseButton.MiddleButton;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region GraphicsInterface

        public void SetPadding(double padding)
        {
            if (this.padding == padding)
            {
                return;
            }

            this.padding = padding;
            base.SetCurrentValue(DocumentControl.PaddingProperty, new Thickness(padding));
        }

        public GraphicsObject CreateDrawingObject()
        {
            DrawingObject drawingObject = new DrawingObject();

            this.drawArea.AddVisual(drawingObject);

            return drawingObject;
        }

        public void DeleteDrawingObject(GraphicsObject drawingObject)
        {
            drawArea.RemoveVisual(drawingObject as DrawingObject);
        }

        public void DeleteDrawingObjects()
        {
            List<GraphicsObject> drawingObjects = drawArea.GetAllVisual().Cast<GraphicsObject>().ToList();

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

        /// <summary>
        /// 捕获鼠标之后，即使鼠标移出了触发鼠标事件的元素所在区域，也会继续触发该元素的鼠标事件（比如鼠标移动事件）
        /// </summary>
        public bool GICaptureMouse()
        {
            return this.CaptureMouse();
        }

        /// <summary>
        /// 释放捕获鼠标
        /// </summary>
        public void GIRleaseMouseCapture()
        {
            this.ReleaseMouseCapture();
        }

        #endregion

        #region 事件处理器

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.drawArea = base.Template.FindName("PART_DrawingArea", this) as DrawingArea;
            this.drawArea.SizeChanged += DrawArea_SizeChanged;
            this.scrollbar = base.Template.FindName("PART_Scrollbar", this) as ScrollBar;
            this.scrollbar.PreviewMouseLeftButtonDown += Scrollbar_PreviewMouseLeftButtonDown;
            this.scrollbar.PreviewMouseLeftButtonUp += Scrollbar_PreviewMouseLeftButtonUp;
            this.scrollbar.PreviewMouseMove += Scrollbar_PreviewMouseMove;
            this.Scrollbar = new VTScrollbarImpl(this.scrollbar);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            Point position = e.GetPosition(this.drawArea);
            this.mouseDownData.X = position.X;
            this.mouseDownData.Y = position.Y;
            this.mouseDownData.ClickCount = e.ClickCount;
            this.mouseDownData.Button = MouseButton2VTMouseButton(e.ChangedButton);

            this.GIMouseDown?.Invoke(this, this.mouseDownData);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            Point position = e.GetPosition(this.drawArea);
            this.mouseUpData.X = position.X;
            this.mouseUpData.Y = position.Y;
            this.mouseUpData.ClickCount = e.ClickCount;
            this.mouseDownData.Button = MouseButton2VTMouseButton(e.ChangedButton);

            this.GIMouseUp?.Invoke(this, this.mouseUpData);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            Point position = e.GetPosition(this.drawArea);
            this.mouseMoveData.X = position.X;
            this.mouseMoveData.Y = position.Y;

            this.GIMouseMove?.Invoke(this, this.mouseMoveData);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            this.mouseWheelData.Delta = e.Delta;

            this.GIMouseWheel?.Invoke(this, this.mouseWheelData);
        }

        #region 滚动条事件

        private void Scrollbar_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.MouseDevice.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            this.scrollChangedData.NewScroll = this.GetScrollValue();
            this.GIScrollChanged?.Invoke(this, this.scrollChangedData);
        }

        private void Scrollbar_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        }

        private void Scrollbar_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.scrollChangedData.OldScroll = this.GetScrollValue();
        }

        #endregion

        private void DocumentControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.GILoaded?.Invoke(this);
        }

        private void DrawArea_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            logger.InfoFormat("drawAreaWidth = {0}, drawAreaHeight = {1}", this.drawArea.ActualWidth, this.drawArea.ActualHeight);
        }

        #endregion
    }
}
