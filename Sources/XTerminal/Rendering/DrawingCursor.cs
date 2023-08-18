using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using XTerminal.Base;
using XTerminal.Base.Enumerations;
using XTerminal.Document;

namespace XTerminal.Rendering
{
    /// <summary>
    /// 光标的渲染模型
    /// </summary>
    public class DrawingCursor : DrawingObject
    {
        private static readonly Pen TransparentPen = new Pen(Brushes.Transparent, 0);
        private static readonly double BlockWidth = 5;
        private static readonly double LineWidth = 2;
        private static readonly double UnderscoreWidth = 3;

        private VTCursor cursor;

        /// <summary>
        /// TODO:先把光标高度写死，后面再优化..
        /// </summary>
        private static readonly double CursorHeight = 15;

        protected override void OnInitialize(VTDocumentElement element)
        {
            this.cursor = element as VTCursor;
        }

        protected override void Draw(DrawingContext dc)
        {
            this.Offset = new Vector(this.cursor.OffsetX, this.cursor.OffsetY);

            Brush brush = DrawingUtils.VTForeground2Brush(this.cursor.Color);

            switch (this.cursor.Style)
            {
                case VTCursorStyles.Block:
                    {
                        dc.DrawRectangle(brush, TransparentPen, new Rect(0, 0, BlockWidth, CursorHeight));
                        break;
                    }

                case VTCursorStyles.Line:
                    {
                        dc.DrawRectangle(brush, TransparentPen, new Rect(0, 0, LineWidth, CursorHeight));
                        break;
                    }

                case VTCursorStyles.Underscore:
                    {
                        dc.DrawRectangle(brush, TransparentPen, new Rect(0, 0, UnderscoreWidth, 5));
                        break;
                    }

                case VTCursorStyles.None:
                    {
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
