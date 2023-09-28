using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ModengTerm.Base
{
    public class GifFrame
    {
        /// <summary>
        /// 延迟时间
        /// </summary>
        public int Delay { get; set; }

        /// <summary>
        /// 原始像素数据
        /// </summary>
        public byte[] Data { get; set; }

        public int PixelWidth { get; set; }

        public int PixelHeight { get; set; }

        public int Left { get; set; }

        public int Top { get; set; }

        /// <summary>
        /// 处置方法，表示如何处理上一张图片，如替换为背景色等
        /// </summary>
        public int Disposal { get; set; }

        /// <summary>
        /// 调色板信息
        /// </summary>
        public BitmapPalette BitmapPalette { get; set; }

        public int TransparencyFlag { get; set; }

        /// <summary>
        /// /grctlext/TransparentColorIndex
        /// </summary>
        public int TransparencyColorIndex { get; set; }
    }

    public class GifMetadata
    {
        /// <summary>
        /// 所有帧列表
        /// </summary>
        public List<GifFrame> Frames { get; private set; }

        /// <summary>
        /// 调色板信息
        /// </summary>
        public BitmapPalette BitmapPalette { get; set; }

        /// <summary>
        /// 帧格式
        /// </summary>
        public PixelFormat PixelFormat { get; set; }

        /// <summary>
        /// 图像宽度
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 图像高度
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// 背景颜色索引
        /// </summary>
        public byte BackgroundColorIndex { get; set; }

        public GifMetadata()
        {
            this.Frames = new List<GifFrame>();
        }
    }

    /// <summary>
    /// 解析GIF文件
    /// https://blog.lindexi.com/post/WPF-%E9%80%9A%E8%BF%87-GifBitmapDecoder-%E8%B0%83%E7%94%A8-WIC-%E8%A7%A3%E6%9E%90-Gif-%E5%92%8C%E8%BF%9B%E8%A1%8C%E5%8A%A8%E7%94%BB%E6%92%AD%E6%94%BE%E7%9A%84%E7%AE%80%E5%8D%95%E6%96%B9%E6%B3%95.html
    /// https://learn.microsoft.com/en-us/windows/win32/wic/-wic-native-image-format-metadata-queries#gif-metadata?WT.mc_id=WD-MVP-5003260
    /// </summary>
    public static class GifParser
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("GifParser");

        public static GifMetadata GetFrames(string gifPath)
        {
            if (!File.Exists(gifPath))
            {
                return new GifMetadata();
            }

            using (FileStream fs = File.Open(gifPath, FileMode.Open))
            {
                return GifParser.GetFrames(fs);
            }
        }

        public static GifMetadata GetFrames(Stream gifStream)
        {
            GifBitmapDecoder decoder = new GifBitmapDecoder(gifStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);

            GifMetadata gifMetadata = new GifMetadata();
            gifMetadata.Width = Convert.ToInt32(decoder.Metadata.GetQuery("/logscrdesc/Width"));
            gifMetadata.Height = Convert.ToInt32(decoder.Metadata.GetQuery("/logscrdesc/Height"));
            gifMetadata.BackgroundColorIndex = Convert.ToByte(decoder.Metadata.GetQuery("/logscrdesc/BackgroundColorIndex"));
            gifMetadata.BitmapPalette = decoder.Palette;
            gifMetadata.PixelFormat = PixelFormats.Indexed8; // 索引格式，目前写死

            foreach (BitmapFrame bitmapFrame in decoder.Frames)
            {
                BitmapMetadata frameMetadata = bitmapFrame.Metadata as BitmapMetadata;

                GifFrame frame = new GifFrame()
                {
                    Delay = Convert.ToInt32(frameMetadata.GetQuery("/grctlext/Delay")) * 10,
                    PixelHeight = bitmapFrame.PixelHeight,
                    PixelWidth = bitmapFrame.PixelWidth,
                    Left = Convert.ToInt32(frameMetadata.GetQuery("/imgdesc/Left")),
                    Top = Convert.ToInt32(frameMetadata.GetQuery("/imgdesc/Top")),
                    Disposal = Convert.ToInt32(frameMetadata.GetQuery("/grctlext/Disposal")),
                    TransparencyFlag = Convert.ToInt32(frameMetadata.GetQuery("/grctlext/TransparencyFlag")),
                    TransparencyColorIndex = Convert.ToInt32(frameMetadata.GetQuery("/grctlext/TransparentColorIndex")),
                    BitmapPalette = bitmapFrame.Palette
                };

                int bytesPerPixel = (bitmapFrame.Format.BitsPerPixel / 8);
                byte[] frameData = null;

                switch (frame.TransparencyFlag)
                {
                    case 0:
                        {
                            // 如果此图像未设置动画，则这些位将被设置为0，这表示不希望指定处置方法。
                            frameData = new byte[frame.PixelWidth * frame.PixelHeight * bytesPerPixel];
                            bitmapFrame.CopyPixels(frameData, frame.PixelWidth, 0);
                            break;
                        }

                    case 1:
                        {
                            // 这告诉解码器将图像留在原位并在其上绘制下一个图像
                            frameData = new byte[frame.PixelWidth * frame.PixelHeight * bytesPerPixel];
                            bitmapFrame.CopyPixels(frameData, frame.PixelWidth, 0);

                            GifFrame prevFrame = gifMetadata.Frames.LastOrDefault();
                            for (int i = 0; i < frameData.Length; i++)
                            {
                                if (frameData[i] == frame.TransparencyColorIndex)
                                {
                                    frameData[i] = prevFrame.Data[i];
                                }
                            }
                            break;
                        }

                    case 2:
                        {
                            // 值为2意味着画布应恢复为背景颜色
                            frameData = Enumerable.Repeat<byte>(gifMetadata.BackgroundColorIndex, gifMetadata.Width * gifMetadata.Height * bytesPerPixel).ToArray();
                            break;
                        }

                    case 3:
                        {
                            // 值3被定义为意味着解码器应该将画布恢复到绘制当前图像之前的先前状态
                            // 恢复到上上一张的状态？
                            frameData = gifMetadata.Frames[gifMetadata.Frames.Count - 2].Data.ToArray();
                            break;
                        }

                    default:
                        throw new NotImplementedException();
                }

                frame.Data = frameData;

                //logger.ErrorFormat("{0},{1},{2},{3},{4},{5},{6}", frame.TransparencyColorIndex, frame.TransparencyFlag, frame.Data.Length, frame.PixelWidth, frame.PixelHeight, frame.Left, frame.Top);

                gifMetadata.Frames.Add(frame);
            }

            return gifMetadata;
        }
    }
}