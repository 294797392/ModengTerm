using DotNEToolkit.DataModels;
using ModengTerm.Base.Enumerations.Ssh;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ModengTerm.Base.DataModels.Ssh
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
