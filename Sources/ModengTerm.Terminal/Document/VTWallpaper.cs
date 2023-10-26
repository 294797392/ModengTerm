using ModengTerm.Base;
using ModengTerm.Terminal.DataModels;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ModengTerm.Terminal.Document
{
    public class VTWallpaper : VTFramedElement<IDrawingWallpaper>
    {
        #region 实例变量

        private string uri;
        private int frameIndex;// 当前显示的帧序号

        #endregion

        #region 属性

        /// <summary>
        /// 背景颜色
        /// </summary>
        public string BackgroundColor { get; set; }

        /// <summary>
        /// 壁纸Uri
        /// </summary>
        public string Uri
        {
            get { return this.uri; }
            set
            {
                if (this.uri != value)
                {
                    this.uri = value;
                }
            }
        }

        /// <summary>
        /// 壁纸类型
        /// </summary>
        public WallpaperTypeEnum PaperType { get; set; }

        /// <summary>
        /// 当前要显示的帧
        /// </summary>
        public GifFrame Frame { get; private set; }

        /// <summary>
        /// 如果背景是Live模式，那么这里存储Gif图片的元数据
        /// </summary>
        public GifMetadata Metadata { get; set; }

        /// <summary>
        /// 特效
        /// </summary>
        public EffectTypeEnum Effect { get; set; }

        public override VTDocumentElements Type => VTDocumentElements.Wallpaper;

        /// <summary>
        /// 背景透明度
        /// </summary>
        public double Opacity { get; set; }

        /// <summary>
        /// 背景显示区域
        /// 相对于DrawingWindow的区域
        /// </summary>
        public VTRect Rect { get; set; }

        #endregion

        #region 构造方法

        public VTWallpaper(IDrawingDocument drawingDocument)
            : base(drawingDocument)
        {
        }

        #endregion

        #region VTElement

        protected override void OnInitialize()
        {
            this.DrawingObject.Uri = this.Uri;
            this.DrawingObject.BackgroundColor = this.BackgroundColor;
            this.DrawingObject.PaperType = this.PaperType;
            this.DrawingObject.EffectType = this.Effect;
            this.DrawingObject.Rect = this.Rect;
        }

        protected override void OnRelease()
        {
        }

        public override void RequestInvalidate()
        {
            switch (this.PaperType)
            {
                case WallpaperTypeEnum.Live:
                    {
                        this.Frame = this.Metadata.Frames[this.frameIndex++];

                        if (this.frameIndex >= this.Metadata.Frames.Count)
                        {
                            this.frameIndex = 0;
                        }
                        else
                        {
                            GifFrame nextFrame = this.Metadata.Frames[this.frameIndex];
                            this.Delay = nextFrame.Delay;
                        }

                        this.DrawingObject.Draw();

                        break;
                    }

                case WallpaperTypeEnum.Image:
                case WallpaperTypeEnum.Color:
                    {
                        if (this.DrawingObject.Rect.Width != this.Rect.Width ||
                            this.DrawingObject.Rect.Height != this.Rect.Height)
                        {
                            this.DrawingObject.Rect = this.Rect;
                        }

                        this.DrawingObject.Draw();
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion
    }
}
