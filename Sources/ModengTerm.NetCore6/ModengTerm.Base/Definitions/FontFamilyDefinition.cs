using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Base.Definitions
{
    public class FontFamilyDefinition
    {
        /// <summary>
        /// 字体名字
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 字体名字
        /// 加载字体的时候使用这个属性来加载
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
