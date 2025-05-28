using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace ModengTerm.Base.Definitions
{
    /// <summary>
    /// 定义可以激活插件的所有事件
    /// </summary>
    public enum ActiveEvent
    {
        /// <summary>
        /// 应用程序初始化之后触发
        /// </summary>
        Startup,

        /// <summary>
        /// 打开会话之后触发
        /// SessionOpenedArgument
        /// </summary>
        SshSessionOpened,
        LocalSessionOpened,
        SerialPortSessionOpened,
        TcpSessionOpened
    }

    /// <summary>
    /// 定义一个插件的所有内容
    /// </summary>
    public class AddonDefinition
    {
        /// <summary>
        /// 插件Id
        /// </summary>
        [JsonProperty("id")]
        public string ID { get; set; }

        /// <summary>
        /// 插件名字
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 反射类名
        /// </summary>
        [JsonProperty("classEntry")]
        public string ClassEntry { get; set; }

        /// <summary>
        /// 初始化插件的事件
        /// </summary>
        public List<ActiveEvent> Actives { get; set; }

        /// <summary>
        /// 顶部菜单
        /// </summary>
        [JsonProperty("toolbarMenus")]
        public List<AddonMenuDefinition> ToolbarMenus { get; private set; }

        /// <summary>
        /// 右键菜单
        /// </summary>
        [JsonProperty("contextMenus")]
        public List<AddonMenuDefinition> ContextMenus { get; private set; }

        /// <summary>
        /// PanelItem
        /// </summary>
        [JsonProperty("sessionPanels")]
        public List<PanelItemDefinition> SessionPanels { get; private set; }

        [JsonProperty("globalPanels")]
        public List<PanelItemDefinition> GlobalPanels { get; private set; }

        public AddonDefinition()
        {
            this.ToolbarMenus = new List<AddonMenuDefinition>();
            this.ContextMenus = new List<AddonMenuDefinition>();
            this.SessionPanels = new List<PanelItemDefinition>();
            this.GlobalPanels = new List<PanelItemDefinition>();
        }
    }
}