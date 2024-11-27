using DotNEToolkit.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Base.DataModels
{
    /// <summary>
    /// 窗口主题
    /// </summary>
    public class AppTheme : ModelBase
    {
        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("previews")]
        public string[] Previews { get; set; }
    }
}
