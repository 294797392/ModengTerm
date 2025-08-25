using ModengTerm.Addon.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace ModengTerm.OfficialAddons.Snippet
{
    public class Snippet : StorageObject
    {
        /// <summary>
        /// 脚本名字
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 脚本内容
        /// </summary>
        [JsonProperty("script")]
        public string Script { get; set; }

        /// <summary>
        /// 脚本类型
        /// </summary>
        [JsonProperty("type")]
        public SnippetTypes Type { get; set; }
    }
}
