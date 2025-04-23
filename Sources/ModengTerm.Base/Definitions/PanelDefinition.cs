using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using WPFToolkit.MVVM;

namespace ModengTerm.Base.Definitions
{
    public enum PanelItemOwner
    {
        /// <summary>
        /// PanelItem属于主窗口
        /// </summary>
        Window,

        /// <summary>
        /// PanelItem属于会话
        /// </summary>
        Session
    }

    public class PanelItemDefinition : MenuDefinition
    {
        /// <summary>
        /// 支持的会话类型
        /// </summary>
        [JsonProperty("sessionTypes")]
        public List<int> SessionTypes { get; set; }

        /// <summary>
        /// PanelItem属于
        /// </summary>
        [EnumDataType(typeof(PanelItemOwner))]
        [JsonProperty("owner")]
        public int Owner { get; set; }

        public PanelItemDefinition()
        {
            this.SessionTypes = new List<int>();
        }
    }

    public class PanelDefinition
    {
        /// <summary>
        /// 唯一编号
        /// </summary>
        [JsonProperty("id")]
        public string ID { get; set; }

        /// <summary>
        /// 窗口名称
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 侧边容器里的窗口列表
        /// </summary>
        [JsonProperty("items")]
        public List<PanelItemDefinition> Items { get; set; }

        public PanelDefinition()
        {
            this.Items = new List<PanelItemDefinition>();
        }
    }
}
