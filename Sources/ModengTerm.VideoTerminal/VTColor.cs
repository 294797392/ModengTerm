using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal
{
    public class NamedColor : VTColor
    {
        public NamedColor(string name)
        {
            this.Name = name;
        }
    }

    public class Xterm256Color : VTColor
    {
        public byte R { get; private set; }

        public byte G { get; private set; }

        public byte B { get; private set; }

        public Xterm256Color(byte r, byte g, byte b)
        {
            this.Name = string.Format("{0},{1},{2}", r, g, b);
            this.R = r;
            this.G = g;
            this.B = b;
        }
    }

    public abstract class VTColor
    {
        /// <summary>
        /// Key -> Color
        /// 使用32位整形表示key
        /// 如果颜色使用RGB表示，那么RGB占32位整形的低24位
        /// </summary>
        private static Dictionary<int, VTColor> colorMap = new Dictionary<int, VTColor>();

        public static readonly VTColor DarkBlack = new NamedColor("DarkBlack");
        public static readonly VTColor DarkRed = new NamedColor("DarkRed");
        public static readonly VTColor DarkGreen = new NamedColor("DarkGreen");
        public static readonly VTColor DarkYellow = new NamedColor("DarkYellow");
        public static readonly VTColor DarkBlue = new NamedColor("DarkBlue");
        public static readonly VTColor DarkMagenta = new NamedColor("DarkMagenta");
        public static readonly VTColor DarkCyan = new NamedColor("DarkCyan");
        public static readonly VTColor DarkWhite = new NamedColor("DarkWhite");
        public static readonly VTColor BrightBlack = new NamedColor("BrightBlack");
        public static readonly VTColor BrightRed = new NamedColor("BrightRed");
        public static readonly VTColor BrightGreen = new NamedColor("BrightGreen");
        public static readonly VTColor BrightYellow = new NamedColor("BrightYellow");
        public static readonly VTColor BrightBlue = new NamedColor("BrightBlue");
        public static readonly VTColor BrightMagenta = new NamedColor("BrightMagenta");
        public static readonly VTColor BrightCyan = new NamedColor("BrightCyan");
        public static readonly VTColor BrightWhite = new NamedColor("BrightWhite");

        public string Name { get; protected set; }

        public static VTColor Create(byte r, byte g, byte b)
        {
            int key = 0 | r << 8 | g << 8 | b;
            VTColor color;
            if (!colorMap.TryGetValue(key, out color))
            {
                color = new Xterm256Color(r, g, b);
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