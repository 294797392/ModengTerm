using DotNEToolkit.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Base.DataModels
{
    /// <summary>
    /// 存储一个会话的详细信息
    /// </summary>
    public class SessionDM : ModelBase
    {
        /// <summary>
        /// 会话所属分组
        /// </summary>
        [JsonProperty("groupId")]
        public string GroupID { get; set; }

        /// <summary>
        /// 会话类型
        /// </summary>
        [JsonProperty("type")]
        public int Type { get; set; }

        /// <summary>
        /// 被JSON序列化后的Session数据
        /// </summary>
        [JsonProperty("properties")]
        public string Properties { get; set; }
    }
}
