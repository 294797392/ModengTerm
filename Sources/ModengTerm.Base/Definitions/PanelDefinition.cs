using ModengTerm.Base.Addon;
using Newtonsoft.Json;

namespace ModengTerm.Base.Definitions
{
    public abstract class PanelDefinition
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

        public PanelDefinition()
        {
            this.OpenHotkeys = new List<string>();
            this.CloseHotkeys = new List<string>();
            this.Commands = new List<string>();
        }
    }

    public class SidePanelDefinition : PanelDefinition
    {
        /// <summary>
        /// 指定面板什么时候显示
        /// 参考vscode的activationEvents事件
        /// onClientInitialize - 在客户端启动之后就显示
        /// onTabOpened:terminal - 在打开terminal类型的tab的时候显示
        /// 这些事件保持与TabEvent和ClientEvent一致
        /// </summary>
        [JsonProperty("attachEvents")]
        public List<string> AttachEvents { get; private set; }

        [JsonProperty("detachEvents")]
        public List<string> DetachEvents { get; private set; }

        [JsonProperty("dock")]
        public SidePanelDocks Dock { get; set; }

        public SidePanelDefinition()
        {
            this.AttachEvents = new List<string>();
            this.DetachEvents = new List<string>();
        }
    }

    public class OverlayPanelDefinition : PanelDefinition
    {
        [JsonProperty("dock")]
        public OverlayPanelDocks Dock { get; set; }
    }
}
