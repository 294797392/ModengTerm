using ModengTerm.Terminal;
using ModengTerm.Terminal.Document;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ModengTerm.Rendering.Wallpaper
{
    /// <summary>
    /// 使用静态图片当做壁纸
    /// </summary>
    public class DrawingWallpaperImage : DrawingObject
    {
        private VTWallpaper wallpaper;
        private ImageSource imageSource;

        protected override void OnInitialize()
        {
            this.wallpaper = this.documentElement as VTWallpaper;

            this.imageSource = VTUtils.GetWallpaperBitmap(this.wallpaper.Uri);
        }

        protected override void OnRelease()
        {
        }

        protected override void OnDraw(DrawingContext dc)
        {
            dc.DrawImage(this.imageSource, this.wallpaper.Rect.GetRect());
        }
    }
}
