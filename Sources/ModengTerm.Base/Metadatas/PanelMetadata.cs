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

    /// <summary>
    /// 定义侧边栏面板所属范围
    /// </summary>
    public enum SidePanelScopes
    {
        /// <summary>
        /// 侧边栏面板属于客户端，整个app运行期间只有一个实例
        /// </summary>
        Client,

        /// <summary>
        /// 侧边栏面板属于控制台会话
        /// 每个控制台会话都有一个单独的侧边栏面板实例
        /// </summary>
        ConsoleTab,

        /// <summary>
        /// 侧边栏面板属于Ssh会话
        /// 每个Ssh会话都有一个单独的侧边栏面板实例
        /// </summary>
        SshTab,

        /// <summary>
        /// 侧边栏面板属于串口会话
        /// 每个串口会话都有一个单独的侧边栏面板实例
        /// </summary>
        SerialPortTab,

        /// <summary>
        /// 侧边栏面板属于Sftp会话
        /// 每个Sftp会话都有一个单独的侧边栏面板实例
        /// </summary>
        SftpTab,

        /// <summary>
        /// 侧边栏面板属于Tcp会话
        /// 每个Tcp会话都有一个单独的侧边栏面板实例
        /// </summary>
        TcpTab
    }

    public class SidePanelMetadata : PanelMetadata
    {
        /// <summary>
        /// 侧边栏窗口所属者
        /// </summary>
        [JsonProperty("scopes")]
        public List<SidePanelScopes> Scopes { get; set; }

        /// <summary>
        /// 侧边栏窗口的位置
        /// </summary>
        [JsonProperty("dock")]
        public SidePanelDocks Dock { get; set; }

        public SidePanelMetadata()
        {
            this.Scopes = new List<SidePanelScopes>();
        }
    }

    public class OverlayPanelMetadata : PanelMetadata
    {
        [JsonProperty("dock")]
        public OverlayPanelDocks Dock { get; set; }
    }
}
