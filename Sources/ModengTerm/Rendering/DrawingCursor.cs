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

namespace ModengTerm.Rendering
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

        /// <summary>
        /// TODO:先把光标高度写死，后面再优化..
        /// </summary>
        private static readonly double CursorHeight = 15;

        private VTCursor cursor;
        private Brush brush;

        protected override void OnInitialize(VTDocumentElement element)
        {
            this.cursor = element as VTCursor;
            this.brush = MTermUtils.GetBrush(this.cursor.Color);

            // 先画出来，不然永远不会显示鼠标元素
            // 因为光标闪烁只会移动光标的位置和显示/隐藏光标，并不会重新Draw光标
            this.Draw();
        }

        protected override void Draw(DrawingContext dc)
        {
            switch (this.cursor.Style)
            {
                case VTCursorStyles.Block:
                    {
                        dc.DrawRectangle(this.brush, TransparentPen, new Rect(0, 0, BlockWidth, CursorHeight));
                        break;
                    }

                case VTCursorStyles.Line:
                    {
                        dc.DrawRectangle(this.brush, TransparentPen, new Rect(0, 0, LineWidth, CursorHeight));
                        break;
                    }

                case VTCursorStyles.Underscore:
                    {
                        dc.DrawRectangle(this.brush, TransparentPen, new Rect(0, 0, UnderscoreWidth, 5));
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
