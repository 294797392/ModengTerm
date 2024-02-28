using ModengTerm.Document.Enumerations;
using ModengTerm.Document.Utility;
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

namespace ModengTerm.Document
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
            get { return uri; }
            set
            {
                if (uri != value)
                {
                    uri = value;
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

        public override VTElementTypes Type => VTElementTypes.Wallpaper;

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

        public VTWallpaper(IDrawingCanvas drawingDocument)
            : base(drawingDocument)
        {
        }

        #endregion

        #region VTElement

        protected override void OnInitialize()
        {
            DrawingObject.Uri = Uri;
            DrawingObject.BackgroundColor = BackgroundColor;
            DrawingObject.PaperType = PaperType;
            DrawingObject.EffectType = Effect;
            DrawingObject.Rect = Rect;
        }

        protected override void OnRelease()
        {
        }

        public override void RequestInvalidate()
        {
            switch (PaperType)
            {
                case WallpaperTypeEnum.Live:
                    {
                        Frame = Metadata.Frames[frameIndex++];

                        if (frameIndex >= Metadata.Frames.Count)
                        {
                            frameIndex = 0;
                        }
                        else
                        {
                            GifFrame nextFrame = Metadata.Frames[frameIndex];
                            Delay = nextFrame.Delay;
                        }

                        DrawingObject.Draw();

                        break;
                    }

                case WallpaperTypeEnum.Image:
                case WallpaperTypeEnum.Color:
                    {
                        if (DrawingObject.Rect.Width != Rect.Width ||
                            DrawingObject.Rect.Height != Rect.Height)
                        {
                            DrawingObject.Rect = Rect;
                        }

                        DrawingObject.Draw();
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion
    }
}
