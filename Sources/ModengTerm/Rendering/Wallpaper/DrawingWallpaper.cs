using ModengTerm.Base;
using ModengTerm.Terminal;
using ModengTerm.Terminal.Document;
using ModengTerm.Terminal.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ModengTerm.Rendering.Wallpaper
{
    public class WallpaperFormat
    {
        /// <summary>
        /// 帧的宽度
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 帧的高度
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// 帧格式
        /// </summary>
        public PixelFormat Format { get; set; }

        /// <summary>
        /// 如果FrameFormat是索引格式，那么就需要设置调色板
        /// </summary>
        public BitmapPalette Palette { get; set; }

        /// <summary>
        /// 获取该壁纸是否只有一帧（静态壁纸）
        /// </summary>
        public bool SingleFrame { get; set; }
    }

    public class DrawingWallpaper : DrawingObject
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("DrawingWallpaperBase");

        #region 实例变量

        protected VTWallpaper wallpaper;
        protected WallpaperFormat format;

        private WriteableBitmap writeableBitmap;
        private double oldWidth;
        private double oldHeight;

        private EffectRenderer effectRenderer;
        private bool drawOnce;

        private byte[] frameData;

        #endregion

        #region 实例方法

        /// <summary>
        /// 获取当前要绘制的壁纸的格式信息
        /// </summary>
        /// <returns></returns>
        private WallpaperFormat GetFormat()
        {
            switch (this.wallpaper.PaperType)
            {
                case WallpaperTypeEnum.Color:
                    {
                        VTColor vtc = VTColor.CreateFromRgbKey(wallpaper.BackgroundColor);
                        Color color = Color.FromRgb(vtc.R, vtc.G, vtc.B);

                        this.frameData = Enumerable.Repeat((byte)0, (int)this.wallpaper.Rect.Width * (int)this.wallpaper.Rect.Height).ToArray();

                        return new WallpaperFormat()
                        {
                            // 这里的宽和高设置多少无所谓，会自动拉伸
                            Width = (int)wallpaper.Rect.Width,
                            Height = (int)wallpaper.Rect.Height,
                            Format = PixelFormats.Indexed8,
                            Palette = new BitmapPalette(Enumerable.Repeat(color, 256).ToList()),
                            SingleFrame = true
                        };
                    }

                case WallpaperTypeEnum.Image:
                    {
                        BitmapSource bitmapSource = VTUtils.GetWallpaperBitmap(this.wallpaper.Uri);
                        int bytesPerPixel = bitmapSource.Format.BitsPerPixel / 8;
                        this.frameData = new byte[(int)bitmapSource.PixelWidth * (int)bitmapSource.PixelHeight * bytesPerPixel];
                        bitmapSource.CopyPixels(this.frameData, (int)bitmapSource.PixelWidth * bytesPerPixel, 0);

                        return new WallpaperFormat()
                        {
                            Format = bitmapSource.Format,
                            SingleFrame = true,
                            Height = bitmapSource.PixelHeight,
                            Width = bitmapSource.PixelWidth,
                            Palette = bitmapSource.Palette,
                        };
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        private void PrepareNextFrame()
        {
        }

        #endregion

        #region IDrawingObject

        protected override void OnInitialize()
        {
            this.wallpaper = this.documentElement as VTWallpaper;
            this.format = this.GetFormat();

            // 初始化WriteableBitmap
            DpiScale dpiScale = VisualTreeHelper.GetDpi(this);
            this.writeableBitmap = new WriteableBitmap(this.format.Width, this.format.Height, dpiScale.PixelsPerInchX, dpiScale.PixelsPerInchY, this.format.Format, this.format.Palette);

            // 画
            DrawingContext dc = this.RenderOpen();
            dc.DrawImage(this.writeableBitmap, this.wallpaper.Rect.GetRect());
            dc.Close();

            this.oldWidth = this.wallpaper.Rect.Width;
            this.oldHeight = this.wallpaper.Rect.Height;
        }

        protected override void OnRelease()
        {
            this.effectRenderer.Release();
        }

        protected override void OnDraw(DrawingContext dc)
        {
            throw new NotImplementedException();
        }

        public override void Draw()
        {
            if (this.oldWidth != this.wallpaper.Rect.Width ||
                this.oldHeight != this.wallpaper.Rect.Height)
            {
                // 这段代码可以重绘并让writeableBitmap填充元素
                DrawingContext dc = this.RenderOpen();
                dc.DrawImage(this.writeableBitmap, this.wallpaper.Rect.GetRect());
                dc.Close();
                this.oldWidth = this.wallpaper.Rect.Width;
                this.oldHeight = this.wallpaper.Rect.Height;
            }

            // 重绘当前帧
            // 分两种类型的格式：
            // 1. 只有一帧，这种格式只会画一次
            // 2. 有多帧，获取到这种格式的帧数据之后每次都会画
            if (!this.drawOnce || !this.format.SingleFrame)
            {
                this.PrepareNextFrame();
                Int32Rect dirtyRect = new Int32Rect(0, 0, this.format.Width, this.format.Height);
                this.writeableBitmap.Lock();
                this.writeableBitmap.AddDirtyRect(dirtyRect);
                Marshal.Copy(this.frameData, 0, this.writeableBitmap.BackBuffer, this.frameData.Length);
                this.writeableBitmap.Unlock();

                if (!this.drawOnce)
                {
                    this.drawOnce = true;
                }
            }

            // 画动效
            if (this.effectRenderer == null)
            {
                this.effectRenderer = EffectRendererFactory.Create(this.wallpaper.Effect);
                this.effectRenderer.Initialize(this);
            }
            this.effectRenderer.DrawFrame();
        }

        #endregion
    }
}
