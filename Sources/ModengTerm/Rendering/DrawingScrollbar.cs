using ModengTerm.Terminal.Document;
using ModengTerm.Terminal.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ModengTerm.Rendering
{
    public class DrawingScrollbar : DrawingFrameworkObject, IDrawingScrollbar
    {
        #region 实例变量

        private Brush trackBrush;
        private Brush thumbBrush;
        private Brush buttonBrush;

        #endregion

        #region IDrawingScrollbar

        public VTRect IncreaseRect { get; set; }
        public VTRect DecreaseRect { get; set; }
        public VTRect ThumbRect { get; set; }
        public bool HasScroll { get; set; }
        public ScrollbarStyle Style { get; set; }

        #endregion

        protected override void OnInitialize()
        {
            this.trackBrush = DrawingUtils.GetBrush(this.Style.TrackColor);
            this.thumbBrush = DrawingUtils.GetBrush(this.Style.ThumbColor);
            this.buttonBrush = DrawingUtils.GetBrush(this.Style.ButtonColor);
        }

        protected override void OnRelease()
        {
        }

        protected override void OnDraw(DrawingContext dc)
        {
            if (this.Width <= 0 || this.Height <= 0)
            {
                return;
            }

            // 背景
            dc.DrawRectangle(this.trackBrush, null, new Rect(0, 0, this.Width, this.Height));

            // 上面的拖动标
            dc.DrawRectangle(this.buttonBrush, null, this.DecreaseRect.GetRect());

            // 下面的拖动标
            dc.DrawRectangle(this.buttonBrush, null, this.IncreaseRect.GetRect());

            // 中间的拖动标
            dc.DrawRectangle(this.thumbBrush, null, this.ThumbRect.GetRect());
        }
    }
}
