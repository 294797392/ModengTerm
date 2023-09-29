using ModengTerm.Terminal;
using ModengTerm.Terminal.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ModengTerm.Rendering.Wallpaper
{
    /// <summary>
    /// 实现静态图片作为背景图
    /// </summary>
    public class DrawingImagedWallpaper : DrawingWallpaper
    {
        private BitmapSource bitmapSource;
        private byte[] bitmapPixel;

        protected override WallpaperFormat GetFormat(VTWallpaper wallpaper)
        {
            BitmapSource bitmapSource = this.EnsureBitmapSource();

            return new WallpaperFormat()
            {
                Format = bitmapSource.Format,
                SingleFrame = true,
                Height = bitmapSource.PixelHeight,
                Width = bitmapSource.PixelWidth,
                Palette = bitmapSource.Palette,
            };
        }

        protected override byte[] GetNextFrame()
        {
            return this.bitmapPixel;
        }

        private BitmapSource EnsureBitmapSource()
        {
            if (this.bitmapSource == null)
            {
                this.bitmapSource = VTUtils.GetWallpaperBitmap(this.wallpaper.Uri);
                int bytesPerPixel = this.bitmapSource.Format.BitsPerPixel / 8;
                this.bitmapPixel = new byte[(int)this.bitmapSource.PixelWidth * (int)this.bitmapSource.PixelHeight * bytesPerPixel];
                this.bitmapSource.CopyPixels(this.bitmapPixel, (int)this.bitmapSource.PixelWidth * bytesPerPixel, 0);
            }
            return this.bitmapSource;
        }
    }
}
