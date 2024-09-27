using DotNEToolkit.DataModels;
using ModengTerm.Terminal.Enumerations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.DataModels
{
    /// <summary>
    /// 存储端口转发信息
    /// </summary>
    public class PortForward : ModelBase
    {
        /// <summary>
        /// 端口转发类型
        /// </summary>
        [EnumDataType(typeof(PortForwardTypeEnum))]
        [JsonProperty("type")]
        public int Type { get; set; }

        /// <summary>
        /// 源主机
        /// </summary>
        [JsonProperty("srcAddress")]
        public string SourceAddress { get; set; }

        /// <summary>
        /// 源端口号
        /// </summary>
        [JsonProperty("srcPort")]
        public int SourcePort { get; set; }

        /// <summary>
        /// 目标主机
        /// </summary>
        [JsonProperty("destAddress")]
        public string DestinationAddress { get; set; }

        /// <summary>
        /// 目标端口号
        /// </summary>
        [JsonProperty("destPort")]
        public int DestinationPort { get; set; }

        /// <summary>
        /// 是否在会话启动的时候打开
        /// </summary>
        [JsonProperty("autoOpen")]
        public bool AutoOpen { get; set; }
    }
}
