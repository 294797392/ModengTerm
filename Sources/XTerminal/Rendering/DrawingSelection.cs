using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using XTerminal.Document;
using XTerminal.Rendering;

namespace VideoTerminal.Rendering
{
    /// <summary>
    /// 用来画光标选中的文本的Drawable
    /// </summary>
    public class DrawingSelection : DrawingObject
    {
        private static readonly Brush DefaultSelectionBrush = Application.Current.FindResource("BrushSelectionBackground") as Brush;

        private StreamGeometry selectionGeometry;
        private Pen pen;
        private Brush brush;
        private VTextSelection textSelection;

        public DrawingSelection()
        {
        }

        protected override void OnInitialize(VTDocumentElement documentElement)
        {
            this.textSelection = documentElement as VTextSelection;
            this.selectionGeometry = new StreamGeometry();
            this.pen = new Pen(Brushes.Transparent, 1);
            this.brush = DefaultSelectionBrush;
        }

        protected override void Draw(DrawingContext dc)
        {
            StreamGeometryContext sgc = this.selectionGeometry.Open();

            foreach (VTRect bounds in this.textSelection.Geometry)
            {
                sgc.BeginFigure(new Point(bounds.LeftTop.X, bounds.LeftTop.Y), true, true);
                sgc.LineTo(new Point(bounds.RightTop.X, bounds.RightTop.Y), true, true);
                sgc.LineTo(new Point(bounds.RightBottom.X, bounds.RightBottom.Y), true, true);
                sgc.LineTo(new Point(bounds.LeftBottom.X, bounds.LeftBottom.Y), true, true);
            }

            sgc.Close();

            dc.DrawGeometry(this.brush, this.pen, this.selectionGeometry);
        }
    }
}