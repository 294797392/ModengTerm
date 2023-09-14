using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Base.Definitions
{
    public class ColorDefinition
    {
        /// <summary>
        /// 颜色ID
        /// </summary>
        [JsonProperty("id")]
        public string ID { get; set; }

        /// <summary>
        /// 颜色名字
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 颜色值
        /// 格式为r,g,b
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
