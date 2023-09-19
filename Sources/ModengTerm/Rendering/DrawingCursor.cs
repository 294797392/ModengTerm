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

        private VTCursor cursor;
        private Brush brush;
        private Rect rect;

        protected override void OnInitialize(VTDocumentElement element)
        {
            this.cursor = element as VTCursor;
            this.brush = DrawingUtils.GetBrush(this.cursor.Color);

            switch (this.cursor.Style)
            {
                case VTCursorStyles.Block:
                    {
                        this.rect = new Rect(0, 0, BlockWidth, this.cursor.Size.Height);
                        break;
                    }

                case VTCursorStyles.Line:
                    {
                        this.rect = new Rect(0, 0, LineWidth, this.cursor.Size.Height);
                        break;
                    }

                case VTCursorStyles.Underscore:
                    {
                        this.rect = new Rect(0, 0, UnderscoreWidth, 5);
                        break;
                    }

                default:
                    {
                        this.rect = new Rect();
                        break;
                    }
            }

            // 先画出来，不然永远不会显示鼠标元素
            // 因为光标闪烁只会移动光标的位置和显示/隐藏光标，并不会重新Draw光标
            this.Draw();
        }

        protected override void Draw(DrawingContext dc)
        {
            dc.DrawRectangle(this.brush, TransparentPen, this.rect);
        }
    }
}
