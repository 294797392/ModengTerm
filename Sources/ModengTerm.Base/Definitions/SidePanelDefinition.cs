using ModengTerm.Base.Enumerations;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ModengTerm.Base.Definitions
{
    public class SidePanelItemDefinition
    {
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [JsonProperty("icon")]
        public string Icon { get; set; }

        /// <summary>
        /// 界面的类名
        /// </summary>
        [JsonProperty("className")]
        public string ClassName { get; set; }

        /// <summary>
        /// ViewModel的类名
        /// </summary>
        [JsonProperty("vmClassName")]
        public string VMClassName { get; set; }

        /// <summary>
        /// 支持的会话类型
        /// </summary>
        [JsonProperty("sessionTypes")]
        public List<int> SessionTypes { get; set; }

        public SidePanelItemDefinition() 
        {
            this.SessionTypes = VTBaseUtils.GetEnumValues<SessionTypeEnum>().Select(v => (int)v).ToList();
        }
    }

    public class SidePanelDefinition
    {
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 容器贴哪个边
        /// </summary>
        [EnumDataType(typeof(SideWindowDock))]
        [JsonProperty("dock")]
        public int Dock { get; set; }

        /// <summary>
        /// 侧边容器里的窗口列表
        /// </summary>
        [JsonProperty("windows")]
        public List<SidePanelItemDefinition> Windows { get; set; }

        public SidePanelDefinition()
        {
            this.Windows = new List<SidePanelItemDefinition>();
        }
    }
}
