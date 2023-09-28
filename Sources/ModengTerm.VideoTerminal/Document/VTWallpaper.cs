using ModengTerm.Base;
using ModengTerm.Terminal.DataModels;
using ModengTerm.Terminal.Enumerations;
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
using XTerminal.Document;

namespace ModengTerm.Terminal.Document
{
    public class VTWallpaper : VTFramedElement
    {
        #region 实例变量

        private string uri;
        private VTRect vtc;
        private int frameIndex;// 当前显示的帧序号

        #endregion

        #region 属性

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
        /// 在set访问器里会消除文档的边距
        /// </summary>
        public VTRect Rect
        {
            get { return this.vtc; }
            set
            {
                // -5,-5,+5,+5是为了消除边距
                this.vtc = new VTRect(-5, -5, value.Width + 5, value.Height + 5);
            }
        }

        /// <summary>
        /// 当前要显示的帧
        /// </summary>
        public GifFrame Frame { get; private set; }

        /// <summary>
        /// 如果背景是Live模式，那么这里存储Gif图片的元数据
        /// </summary>
        public GifMetadata Metadata { get; set; }

        public override VTDocumentElements Type => VTDocumentElements.Wallpaper;

        #endregion

        #region VTDocumentElement

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
