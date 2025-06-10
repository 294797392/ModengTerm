using ModengTerm.Document;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Base.DataModels.Terminal
{
    /// <summary>
    /// 存储VTColorIndex和RgbKey的映射关系
    /// RgbKeys的索引对应VTColorIndex的值
    /// </summary>
    public class VTColorTable
    {
        /// <summary>
        /// 所有颜色列表
        /// </summary>
        [JsonProperty("rgbKeys")]
        public List<string> RgbKeys { get; set; }

        /// <summary>
        /// 根据VTColorIndex获取对应的颜色
        /// </summary>
        /// <param name="colorIndex"></param>
        /// <returns></returns>
        public VTColor GetColor(VTColorIndex colorIndex)
        {
            return VTColor.CreateFromRgbKey(this.RgbKeys[(int)colorIndex]);
        }
    }
}
