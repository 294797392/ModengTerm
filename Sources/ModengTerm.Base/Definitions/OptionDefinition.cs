using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Base.Definitions
{
    public class OptionDefinition
    {
        /// <summary>
        /// 唯一标志符
        /// </summary>
        [JsonProperty("id")]
        public string ID { get; set; }

        /// <summary>
        /// 选项名字
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 选项对应的页面入口类
        /// </summary>
        [JsonProperty("entryClass")]
        public string EntryClass { get; set; }

        /// <summary>
        /// 子选项页面
        /// </summary>
        [JsonProperty("children")]
        public List<OptionDefinition> Children { get; private set; }

        public OptionDefinition()
        {
            this.Children = new List<OptionDefinition>();
        }
    }
}
