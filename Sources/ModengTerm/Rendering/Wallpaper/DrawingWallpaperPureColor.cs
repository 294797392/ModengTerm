using ModengTerm.Terminal.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using XTerminal.Document;

namespace ModengTerm.Rendering.Background
{
    public class DrawingWallpaperPureColor : DrawingObject
    {
        private VTWallpaper wallpaper;
        private Brush brush;

        /// <summary>
        /// 获取该壁纸大小
        /// </summary>
        protected Rect Size
        {
            get
            {
                // -5,-5,+5,+5是为了消除边距
                return new Rect(-5, -5, this.wallpaper.Rect.Width + 5, this.wallpaper.Rect.Height + 5);
            }
        }

        protected override void OnInitialize()
        {
            this.wallpaper = this.documentElement as VTWallpaper;

            this.brush = DrawingUtils.GetBrush(this.wallpaper.Uri);
        }

        protected override void OnRelease()
        {
            base.OnRelease();
        }

        protected override void OnDraw(DrawingContext dc)
        {
            dc.DrawRectangle(this.brush, null, this.Size);
        }
    }
}
