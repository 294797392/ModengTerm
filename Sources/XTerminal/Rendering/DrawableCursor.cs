using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using XTerminal.Document;

namespace XTerminal.Rendering
{
    /// <summary>
    /// 光标的渲染模型
    /// </summary>
    public class DrawableCursor : XDocumentDrawable
    {
        private static readonly Pen TransparentPen = new Pen(Brushes.Transparent, 0);
        private static readonly double BlockWidth = 5;
        private static readonly double LineWidth = 2;
        private static readonly double UnderscoreWidth = 3;

        private VTCursorStyles cursorType = VTCursorStyles.None;

        protected override void Draw(DrawingContext dc)
        {
            VTCursor cursor = this.OwnerElement as VTCursor;

            this.Offset = new Vector(cursor.OffsetX, cursor.OffsetY);

            Brush brush = WPFRenderUtils.VTForeground2Brush(cursor.Color);

            switch (cursor.Style)
            {
                case VTCursorStyles.Block:
                    {
                        dc.DrawRectangle(brush, TransparentPen, new Rect(0, 0, BlockWidth, cursor.LineHeight));
                        break;
                    }

                case VTCursorStyles.Line:
                    {
                        dc.DrawRectangle(brush, TransparentPen, new Rect(0, 0, LineWidth, cursor.LineHeight));
                        break;
                    }

                case VTCursorStyles.Underscore:
                    {
                        dc.DrawRectangle(brush, TransparentPen, new Rect(0, 0, UnderscoreWidth, cursor.LineHeight));
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
