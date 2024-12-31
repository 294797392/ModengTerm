using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Base.Definitions
{
    public class FontSizeDefinition
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
        /// </summary>
        [JsonProperty("value")]
        public int Value { get; set; }
    }
}
