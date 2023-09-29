using ModengTerm.Rendering.Wallpaper;
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
using System.Windows.Media.Imaging;
using XTerminal.Document;

namespace ModengTerm.Rendering.Background
{
    /// <summary>
    /// 使用纯色或者静态图绘制背景
    /// </summary>
    public class DrawingColoredWallpaper : DrawingWallpaper
    {
        /// <summary>
        /// 纯色frame
        /// </summary>
        private byte[] frame;

        protected override WallpaperFormat GetFormat(VTWallpaper wallpaper)
        {
        }

        protected override byte[] GetNextFrame()
        {
            if (this.frame == null)
            {
                this.frame = Enumerable.Repeat((byte)0, (int)this.wallpaper.Rect.Width * (int)this.wallpaper.Rect.Height).ToArray();
            }

            return this.frame;
        }

        //protected override void OnInitialize()
        //{
        //    this.wallpaper = this.documentElement as VTWallpaper;

        //    if (this.wallpaper.PaperType == WallpaperTypeEnum.Color)
        //    {
        //        this.brush = DrawingUtils.GetBrush(this.wallpaper.Uri);
        //    }
        //    else if (this.wallpaper.PaperType == WallpaperTypeEnum.Image)
        //    {
        //        ImageBrush imageBrush = new ImageBrush();
        //        imageBrush.ImageSource = VTUtils.GetWallpaperBitmap(this.wallpaper.Uri);
        //        imageBrush.TileMode = TileMode.None;
        //        this.brush = imageBrush;
        //    }
        //    else
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //protected override void OnRelease()
        //{
        //}

        //public override void Draw()
        //{
        //    if (this.oldWidth != this.wallpaper.Rect.Width ||
        //        this.oldHeight != this.wallpaper.Rect.Height)
        //    {
        //        DrawingContext dc = this.RenderOpen();
        //        dc.DrawRectangle(this.brush, null, this.wallpaper.Rect.GetRect());
        //        dc.Close();
        //        this.oldWidth = this.wallpaper.Rect.Width;
        //        this.oldHeight = this.wallpaper.Rect.Height;
        //    }
        //}

        //protected override void OnDraw(DrawingContext dc)
        //{
        //}
    }
}
