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
    public class DrawingWallpaperPureColor : DrawingWallpaper
    {
        private Brush brush;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            this.brush = DrawingUtils.GetBrush(this.wallpaper.Wallpaper.Uri);
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
