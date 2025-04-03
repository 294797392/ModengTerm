using ModengTerm.Base.Enumerations;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using WPFToolkit.MVVM;

namespace ModengTerm.Base.Definitions
{
    public class PanelItemDefinition : MenuDefinition
    {
        /// <summary>
        /// 支持的会话类型
        /// </summary>
        [JsonProperty("sessionTypes")]
        public List<int> SessionTypes { get; set; }

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
        /// 容器贴哪个边
        /// 暂时用不到
        /// </summary>
        [EnumDataType(typeof(SideWindowDock))]
        [JsonProperty("dock")]
        public int Dock { get; set; }

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
