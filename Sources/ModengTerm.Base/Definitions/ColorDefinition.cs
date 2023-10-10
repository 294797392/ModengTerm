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
        /// 如果背景是壁纸，那么该字段表示和壁纸相同色调的颜色值
        /// 如果背景是纯色，那么该字段表示颜色值
        /// 格式为r,g,b
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }

        /// <summary>
        /// 如果用的是壁纸，那么这个字段表示壁纸的Uri
        /// </summary>
        [JsonProperty("uri")]
        public string Uri { get; set; }

        public ColorDefinition()
        { }

        public ColorDefinition(string name, string value, string uri)
        {
            this.Name = name;
            this.Value = value;
            this.Uri = uri;
        }
    }
}
