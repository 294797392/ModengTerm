using ModengTerm.Base.Addon;
using ModengTerm.Base.Enumerations;
using Newtonsoft.Json;

namespace ModengTerm.Base.Metadatas
{
    public abstract class PanelMetadata
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
        //     图标
        [JsonProperty("icon")]
        public string Icon { get; set; }

        /// <summary>
        /// 打开该面板的快捷键
        /// </summary>
        [JsonProperty("openHotkeys")]
        public List<string> OpenHotkeys { get; private set; }

        /// <summary>
        /// 关闭该面板的快捷键
        /// </summary>
        [JsonProperty("closeHotkeys")]
        public List<string> CloseHotkeys { get; private set; }

        /// <summary>
        /// 用来打开或关闭Panel的Command
        /// </summary>
        [JsonProperty("commands")]
        public List<string> Commands { get; private set; }

        /// <summary>
        /// 指定面板在哪个类型的会话里显示
        /// 如果不指定，那么面板的显示范围是整个客户端
        /// </summary>
        [JsonProperty("scopes")]
        public List<SessionTypeEnum> Scopes { get; private set; }

        public PanelMetadata()
        {
            this.OpenHotkeys = new List<string>();
            this.CloseHotkeys = new List<string>();
            this.Commands = new List<string>();
            this.Scopes = new List<SessionTypeEnum>();
        }
    }

    public class SidePanelMetadata : PanelMetadata
    {
        /// <summary>
        /// 侧边栏窗口的位置
        /// </summary>
        [JsonProperty("dock")]
        public SidePanelDocks Dock { get; set; }

        public SidePanelMetadata()
        {
        }
    }

    public class OverlayPanelMetadata : PanelMetadata
    {
        [JsonProperty("dock")]
        public OverlayPanelDocks Dock { get; set; }
    }
}
