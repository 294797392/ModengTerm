using ModengTerm.Base.Addon;
using ModengTerm.Base.Enumerations;
using Newtonsoft.Json;

namespace ModengTerm.Base.Definitions
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

        public PanelMetadata()
        {
            this.OpenHotkeys = new List<string>();
            this.CloseHotkeys = new List<string>();
            this.Commands = new List<string>();
        }
    }

    public enum SidePanelScopes
    {
        /// <summary>
        /// 侧边栏属于客户端，整个app运行期间只有一个实例
        /// </summary>
        Client,

        /// <summary>
        /// 侧边栏属于会话
        /// 每个会话都有一个单独的侧边栏实例
        /// </summary>
        Session,
    }

    public class SidePanelMetadata : PanelMetadata
    {
        /// <summary>
        /// 侧边栏窗口所属者
        /// </summary>
        [JsonProperty("scope")]
        public SidePanelScopes Scope { get; set; }

        /// <summary>
        /// 如果侧边栏所属者是Client，那么这个字段表示在打开哪些Session的时候显示侧边栏
        /// </summary>
        [JsonProperty("sessionTypes")]
        public List<SessionTypeEnum> SessionTypes { get; private set; }

        /// <summary>
        /// 侧边栏窗口的位置
        /// </summary>
        [JsonProperty("dock")]
        public SidePanelDocks Dock { get; set; }

        public SidePanelMetadata()
        {
            this.SessionTypes = new List<SessionTypeEnum>();
        }
    }

    public class OverlayPanelMetadata : PanelMetadata
    {
        [JsonProperty("dock")]
        public OverlayPanelDocks Dock { get; set; }
    }
}
