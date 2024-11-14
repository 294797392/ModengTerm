using ModengTerm.Document;
using ModengTerm.Document.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ModengTerm.UserControls.TerminalUserControls.Rendering
{
    /// <summary>
    /// 表示文档上的一个可视化对象（光标，文本块，文本行...）
    /// </summary>
    public class DrawingObject : DrawingVisual, IDocumentObject
    {
        #region 实例变量

        private Visibility visible = Visibility.Visible;

        private StreamGeometry streamGeometry;

        #endregion

        #region 属性

        public string ID { get; protected set; }

        #endregion

        #region 构造方法

        public DrawingObject()
        {
        }

        #endregion

        #region 公开接口

        public void SetOpacity(double opacity)
        {
            if (this.Opacity == opacity)
            {
                return;
            }

            this.Opacity = opacity;
        }

        public void Arrange(double x, double y)
        {
            this.Offset = new Vector(x, y);
        }

        public void DrawRectangle(VTRect vtRec, VTPen vtPen, VTColor vtColor)
        {
            Brush brush = DrawingUtils.GetBrush(vtColor);

            Pen pen = null;
            if (vtPen != null)
            {
                pen = DrawingUtils.GetPen(vtPen);
            }

            Rect rectangle = vtRec.GetRect();

            DrawingContext dc = this.RenderOpen();

            dc.DrawRectangle(brush, pen, rectangle);

            dc.Close();
        }

        public void DrawRectangles(List<VTRect> vtRects, VTPen vtPen, VTColor vtColor)
        {
            if (this.streamGeometry == null)
            {
                this.streamGeometry = new StreamGeometry();
            }

            StreamGeometryContext sgc = this.streamGeometry.Open();

            foreach (VTRect bounds in vtRects)
            {
                sgc.BeginFigure(new Point(bounds.LeftTop.X, bounds.LeftTop.Y), true, true);
                sgc.LineTo(new Point(bounds.RightTop.X, bounds.RightTop.Y), true, true);
                sgc.LineTo(new Point(bounds.RightBottom.X, bounds.RightBottom.Y), true, true);
                sgc.LineTo(new Point(bounds.LeftBottom.X, bounds.LeftBottom.Y), true, true);
            }

            sgc.Close();

            Brush brush = DrawingUtils.GetBrush(vtColor);

            Pen pen = null;
            if (vtPen != null)
            {
                pen = DrawingUtils.GetPen(vtPen);
            }

            DrawingContext dc = this.RenderOpen();

            dc.DrawGeometry(brush, pen, this.streamGeometry);

            dc.Close();
        }

        public VTextMetrics DrawText(VTFormattedText vtFormattedText)
        {
            DrawingContext dc = this.RenderOpen();

            FormattedText formattedText = DrawingUtils.CreateFormattedText(vtFormattedText, dc);
            dc.DrawText(formattedText, DrawingUtils.ZeroPoint);

            dc.Close();

            return new VTextMetrics()
            {
                Width = formattedText.WidthIncludingTrailingWhitespace,
                Height = formattedText.Height
            };
        }

        public VTextRange MeasureText(VTFormattedText vtFormattedText, int startIndex, int count)
        {
            if (vtFormattedText == null)
            {
                return new VTextRange();
            }

            if (startIndex < 0)
            {
                startIndex = 0;
            }

            int totalChars = vtFormattedText.Text.Length;
            if (startIndex + count > totalChars)
            {
                startIndex = 0;
                count = totalChars;
            }

            if (startIndex == 0 && count == 0)
            {
                return new VTextRange();
            }

            FormattedText formattedText = DrawingUtils.CreateFormattedText(vtFormattedText);
            System.Windows.Media.Geometry geometry = formattedText.BuildHighlightGeometry(DrawingUtils.ZeroPoint, startIndex, count);
            return new VTextRange(geometry.Bounds.Left + this.Offset.X, this.Offset.Y, geometry.Bounds.Width, geometry.Bounds.Height);
        }

        public void Clear()
        {
            DrawingContext dc = this.RenderOpen();

            dc.Close();
        }

        #endregion
    }
}
