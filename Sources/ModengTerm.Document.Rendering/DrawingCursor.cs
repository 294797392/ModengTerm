using ModengTerm.Document.Drawing;
using ModengTerm.Document.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ModengTerm.Document.Rendering
{
    /// <summary>
    /// 光标的渲染模型
    /// </summary>
    public class DrawingCursor : DrawingObject, IDrawingCursor
    {
        private Brush brush;

        public VTCursorStyles Style { get; set; }

        public VTRect Size { get; set; }

        public string Color { get; set; }

        protected override void OnInitialize()
        {
            this.brush = DrawingUtils.GetBrush(this.Color);

            // 先画出来，不然永远不会显示鼠标元素
            // 因为光标闪烁只会移动光标的位置和显示/隐藏光标，并不会重新Draw光标
            this.Draw();
        }

        protected override void OnRelease()
        {
        }

        protected override void OnDraw(DrawingContext dc)
        {
            dc.DrawRectangle(this.brush, null, this.Size.GetRect());
        }
    }
}
