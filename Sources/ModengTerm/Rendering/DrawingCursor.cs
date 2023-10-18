using ModengTerm.Terminal.Document;
using ModengTerm.Terminal.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using XTerminal.Base;
using XTerminal.Base.Enumerations;

namespace ModengTerm.Rendering
{
    /// <summary>
    /// 光标的渲染模型
    /// </summary>
    public class DrawingCursor : DrawingObject, IDrawingCursor
    {
        private static readonly Pen TransparentPen = new Pen(Brushes.Transparent, 0);

        private Brush brush;
        private Rect rect;

        public VTCursorStyles Style { get; set; }

        public VTSize Size { get; set; }

        public string Color { get; set; }

        protected override void OnInitialize()
        {
            this.brush = DrawingUtils.GetBrush(this.Color);

            switch (this.Style)
            {
                case VTCursorStyles.Block:
                    {
                        this.rect = new Rect(0, 0, this.Size.Width, this.Size.Height);
                        break;
                    }

                case VTCursorStyles.Line:
                    {
                        this.rect = new Rect(0, 0, 2, this.Size.Height);
                        break;
                    }

                case VTCursorStyles.Underscore:
                    {
                        this.rect = new Rect(0, this.Size.Height - 2, this.Size.Width, 2);
                        break;
                    }

                case VTCursorStyles.None:
                    {
                        this.rect = new Rect();
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            // 先画出来，不然永远不会显示鼠标元素
            // 因为光标闪烁只会移动光标的位置和显示/隐藏光标，并不会重新Draw光标
            this.Draw();
        }

        protected override void OnRelease()
        {
        }

        protected override void OnDraw(DrawingContext dc)
        {
            dc.DrawRectangle(this.brush, TransparentPen, this.rect);
        }
    }
}
