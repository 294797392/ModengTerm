using ModengTerm.Terminal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using XTerminal.Document;
using XTerminal.Enumerations;

namespace ModengTerm
{
    public static class MTermUtils
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("ModengTermUtils");

        private static readonly string[] Splitter = new string[] { "," };


        /// <summary>
        /// RGB字符串 -> Brush
        /// </summary>
        private static readonly Dictionary<string, Brush> BrushMap = new Dictionary<string, Brush>();

        /// <summary>
        /// VTColor转Brush
        /// </summary>
        /// <param name="vtc"></param>
        /// <param name="colorTable"></param>
        /// <returns></returns>
        public static Brush GetBrush(VTColor vtc, Dictionary<string, string> colorTable)
        {
            string rgbKey = string.Empty;

            if (vtc is Xterm256Color)
            {
                rgbKey = vtc.Name;
            }
            else if (vtc is NamedColor)
            {
                rgbKey = colorTable[vtc.Name];
            }

            return MTermUtils.GetBrush(rgbKey);
        }

        /// <summary>
        /// rgb字符串转Brush
        /// </summary>
        /// <param name="rgb">
        /// 用逗号分隔的rgb字符串
        /// 例如：255,255,255
        /// </param>
        /// <returns></returns>
        public static Brush GetBrush(string rgbKey)
        {
            Brush brush;
            if (!BrushMap.TryGetValue(rgbKey, out brush))
            {
                string[] values = rgbKey.Split(Splitter, StringSplitOptions.RemoveEmptyEntries);

                byte r = byte.Parse(values[0]);
                byte g = byte.Parse(values[1]);
                byte b = byte.Parse(values[2]);

                Color color = Color.FromRgb(r, g, b);
                brush = new SolidColorBrush(color);
                BrushMap[rgbKey] = brush;
            }

            return brush;
        }
    }
}
