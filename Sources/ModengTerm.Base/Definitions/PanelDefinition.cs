using Newtonsoft.Json;
using WPFToolkit.MVVM;

namespace ModengTerm.Base.Definitions
{
    public class PanelDefinition
    {
        //
        // 摘要:
        //     菜单ID
        [JsonProperty("id")]
        public string ID { get; set; }

        //
        // 摘要:
        //     菜单名字
        [JsonProperty("name")]
        public string Name { get; set; }

        //
        // 摘要:
        //     界面类名
        [JsonProperty("className")]
        public string ClassName { get; set; }

        //
        // 摘要:
        //     ViewModel类名
        [JsonProperty("vmClassName")]
        public string VMClassName { get; set; }

        //
        // 摘要:
        //     图标
        [JsonProperty("icon")]
        public string Icon { get; set; }

        public PanelDefinition()
        {
        }
    }
}
