using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document
{
    public enum VTColorIndex
    {
        BrightBlack,
        BrightRed,
        BrightGreen,
        BrightYellow,
        BrightBlue,
        BrightMagenta,
        BrightCyan,
        BrightWhite,

        DarkBlack,
        DarkRed,
        DarkGreen,
        DarkYellow,
        DarkBlue,
        DarkMagenta,
        DarkCyan,
        DarkWhite
    }

    /// <summary>
    /// 存储VTColorIndex和RgbKey的映射关系
    /// </summary>
    public class VTColorTable
    {
        [JsonProperty("rgbKeys")]
        public List<string> RgbKeys { get; set; }

        public VTColor GetColor(VTColorIndex colorIndex)
        {
            return VTColor.CreateFromRgbKey(this.RgbKeys[(int)colorIndex]);
        }
    }
}
