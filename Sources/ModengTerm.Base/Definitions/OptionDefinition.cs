using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Base.Definitions
{
    public class OptionDefinition
    {
        /// <summary>
        /// 唯一标志符
        /// </summary>
        [JsonProperty("id")]
        public string ID { get; private set; }

        /// <summary>
        /// 选项名字
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 控件类型
        /// </summary>
        [JsonProperty("entry")]
        public string Entry { get; set; }

        /// <summary>
        /// 子选项页面
        /// </summary>
        [JsonProperty("children")]
        public List<OptionDefinition> Children { get; set; }

        public OptionDefinition() 
        {
        }
    }
}
