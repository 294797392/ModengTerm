using DotNEToolkit.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Base.DataModels
{
    /// <summary>
    /// 存储一个会话的详细信息
    /// </summary>
    public class Session : ModelBase
    {
        /// <summary>
        /// session所属分组
        /// </summary>
        [JsonProperty("groupId")]
        public string GroupID { get; set; }
    }
}
