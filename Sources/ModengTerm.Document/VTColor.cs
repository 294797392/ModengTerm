using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document
{
    /// <summary>
    /// 使用R,G,B,A或者RgbKey表示一个颜色
    /// </summary>
    public class VTColor
    {
        /// <summary>
        /// Key -> Color
        /// 使用32位整形表示key
        /// 如果颜色使用RGB表示，那么RGB占32位整形的低24位
        /// </summary>
        private static readonly Dictionary<string, VTColor> colorMap = new Dictionary<string, VTColor>();
        private static readonly string[] Splitter = new string[] { "," };

        /// <summary>
        /// 格式：r,g,b,a
        /// </summary>
        public string Key { get; protected set; }

        public byte A { get; private set; }

        public byte R { get; private set; }

        public byte G { get; private set; }

        public byte B { get; private set; }

        /// <summary>
        /// 获取该颜色的Html表示的字符串
        /// </summary>
        public string Html { get; set; }

        public VTColor(byte r, byte g, byte b, byte a)
        {
            this.Key = string.Format("{0},{1},{2},{3}", r, g, b, a);
            this.A = a;
            this.R = r;
            this.G = g;
            this.B = b;
            this.Html = ColorTranslator.ToHtml(Color.FromArgb(r, g, b));
        }

        public static VTColor CreateFromRgb(byte r, byte g, byte b, byte a)
        {
            string rgbKey = string.Format("{0},{1},{2},{3}", r, g, b, a);
            return VTColor.CreateFromRgbKey(rgbKey);
        }

        /// <summary>
        /// 从r,g,b字符串创建一个VTColor
        /// </summary>
        /// <param name="colorName"></param>
        /// <returns></returns>
        public static VTColor CreateFromRgbKey(string rgbKey)
        {
            VTColor vtc;
            if (!colorMap.TryGetValue(rgbKey, out vtc))
            {
                string[] values = rgbKey.Split(Splitter, StringSplitOptions.RemoveEmptyEntries);

                byte r = byte.Parse(values[0]);
                byte g = byte.Parse(values[1]);
                byte b = byte.Parse(values[2]);
                byte a = byte.Parse(values[3]);
                vtc = new VTColor(r, g, b, a);
                colorMap[rgbKey] = vtc;
            }
            return vtc;
        }

        public override string ToString()
        {
            return this.Key;
        }
    }
}