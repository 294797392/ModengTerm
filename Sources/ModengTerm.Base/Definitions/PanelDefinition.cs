using Newtonsoft.Json;

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
        [JsonProperty("activeEvents")]
        public List<string> ActiveEvents { get; private set; }

        [JsonProperty("deactiveEvents")]
        public List<string> DeactiveEvents { get; private set; }

        public PanelDefinition()
        {
            this.ActiveEvents = new List<string>();
            this.DeactiveEvents = new List<string>();
        }
    }
}
