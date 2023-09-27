using ModengTerm.Base;
using ModengTerm.Terminal.DataModels;
using ModengTerm.Terminal.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using XTerminal.Document;

namespace ModengTerm.Terminal.Document
{
    public class VTWallpaper : VTFramedElement
    {
        private string uri;
        private VTRect vtc;
        private bool dirty;
        private int frameIndex;// 当前显示的帧序号

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
                    this.SetDirty(true);
                }
            }
        }

        /// <summary>
        /// 壁纸类型
        /// </summary>
        public WallpaperTypeEnum PaperType { get; set; }

        public VTRect Rect
        {
            get { return this.vtc; }
            set
            {
                this.vtc = value;
                this.SetDirty(true);
            }
        }

        /// <summary>
        /// 当前要显示的帧
        /// </summary>
        public GifFrame Frame { get; private set; }

        /// <summary>
        /// 如果背景是Live模式，那么这里存储Gif图片的元数据
        /// </summary>
        public GifMetadata GifMetadata { get; set; }

        public override VTDocumentElements Type => VTDocumentElements.Wallpaper;

        private void SetDirty(bool dirty)
        {
            if (this.dirty != dirty)
            {
                this.dirty = dirty;
            }
        }

        public override void RequestInvalidate()
        {
            switch (this.PaperType)
            {
                case WallpaperTypeEnum.Live:
                    {
                        this.Frame = this.GifMetadata.Frames[this.frameIndex++];

                        if (this.frameIndex >= this.GifMetadata.Frames.Count)
                        {
                            this.frameIndex = 0;
                        }
                        else
                        {
                            GifFrame nextFrame = this.GifMetadata.Frames[this.frameIndex];
                            this.Delay = nextFrame.Delay;
                        }

                        this.DrawingObject.Draw();

                        break;
                    }

                case WallpaperTypeEnum.PureColor:
                    {
                        if (this.dirty)
                        {
                            this.DrawingObject.Draw();

                            this.dirty = false;
                        }
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
