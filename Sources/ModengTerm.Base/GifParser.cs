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

        public double Width { get; set; }

        public double Height { get; set; }

        public double Left { get; set; }

        public double Top { get; set; }
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
        public double Width { get; set; }

        /// <summary>
        /// 图像高度
        /// </summary>
        public double Height { get; set; }

        public GifMetadata()
        {
            this.Frames = new List<GifFrame>();
        }
    }

    /// <summary>
    /// 解析GIF文件
    /// https://blog.lindexi.com/post/WPF-%E9%80%9A%E8%BF%87-GifBitmapDecoder-%E8%B0%83%E7%94%A8-WIC-%E8%A7%A3%E6%9E%90-Gif-%E5%92%8C%E8%BF%9B%E8%A1%8C%E5%8A%A8%E7%94%BB%E6%92%AD%E6%94%BE%E7%9A%84%E7%AE%80%E5%8D%95%E6%96%B9%E6%B3%95.html
    /// </summary>
    public static class GifParser
    {
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
            GifMetadata result = new GifMetadata();

            GifBitmapDecoder decoder = new GifBitmapDecoder(gifStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);

            result.BitmapPalette = decoder.Palette;

            foreach (BitmapFrame bitmapFrame in decoder.Frames)
            {
                BitmapMetadata frameMetadata = bitmapFrame.Metadata as BitmapMetadata;

                byte[] frameData = new byte[bitmapFrame.PixelWidth * bitmapFrame.PixelHeight * (bitmapFrame.Format.BitsPerPixel / 8)];
                bitmapFrame.CopyPixels(frameData, bitmapFrame.PixelWidth, 0);

                GifFrame frame = new GifFrame()
                {
                    Delay = Convert.ToInt32(frameMetadata.GetQuery("/grctlext/Delay")) * 10,
                    Height = bitmapFrame.Height,
                    Width = bitmapFrame.Width,
                    Left = Convert.ToInt32(frameMetadata.GetQuery("/imgdesc/Left")),
                    Top = Convert.ToInt32(frameMetadata.GetQuery("/imgdesc/Top")),
                    Data = frameData
                };

                result.Frames.Add(frame);

                result.PixelFormat = bitmapFrame.Format;
                result.Width = bitmapFrame.Width;
                result.Height = bitmapFrame.Height;
            }

            return result;
        }
    }
}
