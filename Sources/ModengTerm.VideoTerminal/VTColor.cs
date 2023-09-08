using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal
{
    public class VTColor
    {
        /// <summary>
        /// Key -> Color
        /// 使用32位整形表示key
        /// 如果颜色使用RGB表示，那么RGB占32位整形的低24位
        /// </summary>
        private static Dictionary<int, VTColor> colorMap = new Dictionary<int, VTColor>();

        public static readonly VTColor DarkBlack = new VTColor("DarkBlack");
        public static readonly VTColor DarkRed = new VTColor("DarkRed");
        public static readonly VTColor DarkGreen = new VTColor("DarkGreen");
        public static readonly VTColor DarkYellow = new VTColor("DarkYellow");
        public static readonly VTColor DarkBlue = new VTColor("DarkBlue");
        public static readonly VTColor DarkMagenta = new VTColor("DarkMagenta");
        public static readonly VTColor DarkCyan = new VTColor("DarkCyan");
        public static readonly VTColor DarkWhite = new VTColor("DarkWhite");
        public static readonly VTColor BrightBlack = new VTColor("BrightBlack");
        public static readonly VTColor BrightRed = new VTColor("BrightRed");
        public static readonly VTColor BrightGreen = new VTColor("BrightGreen");
        public static readonly VTColor BrightYellow = new VTColor("BrightYellow");
        public static readonly VTColor BrightBlue = new VTColor("BrightBlue");
        public static readonly VTColor BrightMagenta = new VTColor("BrightMagenta");
        public static readonly VTColor BrightCyan = new VTColor("BrightCyan");
        public static readonly VTColor BrightWhite = new VTColor("BrightWhite");

        public byte R { get; private set; }

        public byte G { get; private set; }

        public byte B { get; private set; }

        public string Name { get; private set; }

        /// <summary>
        /// 如果为true，那么使用RGB三个属性表示颜色
        /// 如果为false，那么就是预定义颜色
        /// </summary>
        public bool HasRgb { get; private set; }

        private VTColor(string name)
        {
            this.Name = name;
        }

        private VTColor(byte r, byte g, byte b)
        {
            this.Name = string.Format("R{0} G{1} B{2}", r, g, b);
            this.HasRgb = true;
        }

        public static VTColor Create(byte r, byte g, byte b)
        {
            int key = 0 | r << 8 | g << 8 | b;
            VTColor color;
            if (!colorMap.TryGetValue(key, out color))
            {
                color = new VTColor(r, g, b);
                colorMap[key] = color;
            }
            return color;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}