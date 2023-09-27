using ModengTerm.Base;
using ModengTerm.Terminal.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using XTerminal.Document;
using XTerminal.Document.Rendering;

namespace ModengTerm.Rendering.Background
{
    /// <summary>
    /// 动态壁纸
    /// </summary>
    public class DrawingWallpaperLive : FrameworkVisual, IDrawingObject
    {
        #region 实例变量

        protected VTWallpaper wallpaper;
        private WriteableBitmap writeableBitmap;
        private Int32Rect dirtyRect;

        #endregion

        #region 属性

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

        #endregion

        #region IDrawingObject

        public void Initialize(VTDocumentElement documentElement)
        {
            this.wallpaper = documentElement as VTWallpaper;

            // 初始化WriteableBitmap
            GifMetadata gifMetadata = this.wallpaper.GifMetadata;
            DpiScale dpiScale = VisualTreeHelper.GetDpi(this);
            this.writeableBitmap = new WriteableBitmap((int)gifMetadata.Width, (int)gifMetadata.Height, dpiScale.PixelsPerInchX, dpiScale.PixelsPerInchY, gifMetadata.PixelFormat, gifMetadata.BitmapPalette);

            // 画
            DrawingContext dc = this.RenderOpen();
            dc.DrawImage(this.writeableBitmap, this.Size);
            dc.Close();

            this.dirtyRect = new Int32Rect(0, 0, (int)gifMetadata.Width, (int)gifMetadata.Height);
        }

        public void Release()
        {
        }

        public void Draw()
        {
            // 更新
            GifFrame gifFrame = this.wallpaper.Frame;

            this.writeableBitmap.Lock();
            this.writeableBitmap.AddDirtyRect(this.dirtyRect);
            Marshal.Copy(gifFrame.Data, 0, this.writeableBitmap.BackBuffer, gifFrame.Data.Length);
            this.writeableBitmap.Unlock();
        }

        #endregion
    }
}
