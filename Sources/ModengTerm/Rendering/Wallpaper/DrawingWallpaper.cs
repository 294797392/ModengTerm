using ModengTerm.Terminal;
using ModengTerm.Terminal.Document;
using ModengTerm.Terminal.Enumerations;
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
    /// <summary>
    /// 使用纯色或者静态图绘制背景
    /// </summary>
    public class DrawingWallpaper : DrawingObject
    {
        private VTWallpaper wallpaper;
        private Brush brush;

        protected override void OnInitialize()
        {
            this.wallpaper = this.documentElement as VTWallpaper;

            if (this.wallpaper.PaperType == WallpaperTypeEnum.Color)
            {
                this.brush = DrawingUtils.GetBrush(this.wallpaper.Uri);
            }
            else if (this.wallpaper.PaperType == WallpaperTypeEnum.Image)
            {
                ImageBrush imageBrush = new ImageBrush();
                imageBrush.ImageSource = VTUtils.GetWallpaperBitmap(this.wallpaper.Uri);
                imageBrush.TileMode = TileMode.None;
                this.brush = imageBrush;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        protected override void OnRelease()
        {
        }

        protected override void OnDraw(DrawingContext dc)
        {
            dc.DrawRectangle(this.brush, null, this.wallpaper.Rect.GetRect());
        }
    }
}
