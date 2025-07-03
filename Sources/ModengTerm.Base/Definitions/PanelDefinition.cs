using Newtonsoft.Json;

namespace ModengTerm.Base.Definitions
{
    public class HotkeyDefinition
    {
        [JsonProperty("scope")]
        public HotkeyScopes Scope { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }
    }

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
        //     图标
        [JsonProperty("icon")]
        public string Icon { get; set; }

        /// <summary>
        /// 指定面板什么时候显示
        /// 参考vscode的activationEvents事件
        /// onClientInitialize - 在客户端启动之后就显示
        /// onTabOpened:terminal - 在打开terminal类型的tab的时候显示
        /// 这些事件保持与TabEvent和ClientEvent一致
        /// </summary>
        [JsonProperty("showEvents")]
        public List<string> ShowEvents { get; private set; }

        [JsonProperty("hideEvents")]
        public List<string> HideEvents { get; private set; }

        [JsonProperty("keyShowEvents")]
        public List<HotkeyDefinition> HotkeyShowEvents { get; private set; }

        [JsonProperty("keyHideEvents")]
        public List<HotkeyDefinition> HotkeyHideEvents { get; private set; }

        public PanelDefinition()
        {
            this.ShowEvents = new List<string>();
            this.HideEvents = new List<string>();
            this.HotkeyShowEvents = new List<HotkeyDefinition>();
            this.HotkeyHideEvents = new List<HotkeyDefinition>();
        }
    }
}
