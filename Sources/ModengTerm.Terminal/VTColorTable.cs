using ModengTerm.Document;
using ModengTerm.Terminal.Parsing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal
{
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
