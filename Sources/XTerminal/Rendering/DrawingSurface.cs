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

        private static readonly Point ZeroPoint = new Point();

        #endregion

        #region 实例变量

        private VisualCollection visuals;

        private RenderingOptions options;

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

        private DrawingObject EnsureDrawingObject(VTDocumentElement documentElement)
        {
            DrawingObject drawingObject = documentElement.DrawingContext as DrawingObject;
            if (drawingObject == null)
            {
                drawingObject = this.CreateDrawingObject(documentElement.Type);
                drawingObject.Initialize(documentElement);
                documentElement.DrawingContext = drawingObject;
                this.visuals.Add(drawingObject);
            }
            return drawingObject;
        }

        private VTRect CommonMeasureLine(VTextLine textLine, int startIndex, int count)
        {
            FormattedText formattedText = DrawingUtils.CreateFormattedText(textLine);
            Geometry geometry = formattedText.BuildHighlightGeometry(ZeroPoint, startIndex, count);
            return new VTRect(geometry.Bounds.Left, geometry.Bounds.Top, geometry.Bounds.Width, geometry.Bounds.Height);
        }

        #endregion

        #region ITerminalSurface

        public void MeasureLine(VTextLine textLine)
        {
            FormattedText formattedText = DrawingUtils.CreateFormattedText(textLine);
            DrawingUtils.UpdateTextMetrics(textLine, formattedText);
            textLine.SetMeasureDirty(false);
        }

        /// <summary>
        /// 测量某个渲染模型的大小
        /// TODO：如果测量的是字体，要考虑到对字体应用样式后的测量信息
        /// </summary>
        /// <param name="textLine">要测量的数据模型</param>
        /// <param name="maxCharacters">要测量的最大字符数</param>
        /// <returns></returns>
        public VTRect MeasureLine(VTextLine textLine, int startIndex, int count)
        {
            return this.CommonMeasureLine(textLine, startIndex, count);
        }

        public VTRect MeasureCharacter(VTextLine textLine, int characterIndex)
        {
            return this.CommonMeasureLine(textLine, characterIndex, 1);
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

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            // 调试用，看这个Surface有多大
            //drawingContext.DrawRectangle(Brushes.Red, new Pen(Brushes.Black, 1), new Rect(0, 0, this.ActualWidth, this.ActualHeight));
        }

        #endregion
    }
}
