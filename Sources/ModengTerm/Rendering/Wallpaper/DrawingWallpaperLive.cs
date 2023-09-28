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
        private static log4net.ILog logger = log4net.LogManager.GetLogger("logger");

        #region 实例变量

        protected VTWallpaper wallpaper;
        private WriteableBitmap writeableBitmap;
        private double oldWidth;
        private double oldHeight;

        #endregion

        #region 属性

        /// <summary>
        /// 获取该壁纸大小
        /// </summary>
        private Rect Size
        {
            get
            {
                // -5,-5,+5,+5是为了消除边距
                return new Rect(-5, -5, this.wallpaper.Rect.Width + 5, this.wallpaper.Rect.Height + 5);
            }
        }

        #endregion

        #region 实例方法

        #endregion

        #region IDrawingObject

        public void Initialize(VTDocumentElement documentElement)
        {
            this.wallpaper = documentElement as VTWallpaper;

            // 初始化WriteableBitmap
            GifMetadata gifMetadata = this.wallpaper.GifMetadata;
            DpiScale dpiScale = VisualTreeHelper.GetDpi(this);
            this.writeableBitmap = new WriteableBitmap(gifMetadata.Width, gifMetadata.Height, dpiScale.PixelsPerInchX, dpiScale.PixelsPerInchY, gifMetadata.PixelFormat, gifMetadata.BitmapPalette);

            // 画
            DrawingContext dc = this.RenderOpen();
            dc.DrawImage(this.writeableBitmap, this.Size);
            dc.Close();

            this.oldWidth = this.wallpaper.Rect.Width;
            this.oldHeight = this.wallpaper.Rect.Height;
        }

        public void Release()
        {
        }

        public void Draw()
        {
            if (this.oldWidth != this.wallpaper.Rect.Width ||
                this.oldHeight != this.wallpaper.Rect.Height)
            {
                DrawingContext dc = this.RenderOpen();
                dc.DrawImage(this.writeableBitmap, this.Size);
                dc.Close();

                this.oldWidth = this.wallpaper.Rect.Width;
                this.oldHeight = this.wallpaper.Rect.Height;
            }

            // 更新
            GifFrame gifFrame = this.wallpaper.Frame;

            Int32Rect dirtyRect = new Int32Rect(gifFrame.Left, gifFrame.Top, gifFrame.PixelWidth, gifFrame.PixelHeight);
            //this.writeableBitmap.WritePixels(dirtyRect, gifFrame.Data, gifFrame.PixelWidth, 0);

            this.writeableBitmap.Lock();
            this.writeableBitmap.AddDirtyRect(dirtyRect);
            Marshal.Copy(gifFrame.Data, 0, this.writeableBitmap.BackBuffer, gifFrame.Data.Length);
            this.writeableBitmap.Unlock();
        }

        #endregion
    }
}
