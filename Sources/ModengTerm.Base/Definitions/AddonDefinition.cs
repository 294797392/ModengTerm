using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Base.Definitions
{
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
        /// 插件所拥有的菜单
        /// </summary>
        [JsonProperty("toolbarMenus")]
        public List<MenuItemDefinition> ToolbarMenus { get; private set; }

        /// <summary>
        /// 右键菜单
        /// </summary>
        [JsonProperty("contextMenus")]
        public List<MenuItemDefinition> ContextMenus { get; private set; }

        /// <summary>
        /// 和Session关联的PanelItem
        /// </summary>
        [JsonProperty("sessionPanelItems")]
        public List<PanelItemDefinition> SessionPanelItems { get; private set; }

        /// <summary>
        /// 全局PanelItem
        /// </summary>
        [JsonProperty("panelItems")]
        public List<PanelItemDefinition> PanelItems { get; private set; }

        public AddonDefinition()
        {
            this.ToolbarMenus = new List<MenuItemDefinition>();
            this.ContextMenus = new List<MenuItemDefinition>();
            this.SessionPanelItems = new List<PanelItemDefinition>();
            this.PanelItems = new List<PanelItemDefinition>();
        }
    }
}