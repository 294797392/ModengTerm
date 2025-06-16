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
        /// 侧边栏面板
        /// </summary>
        [JsonProperty("panels")]
        public List<PanelDefinition> Panels { get; private set; }


        public AddonDefinition()
        {
            this.ToolbarMenus = new List<AddonMenuDefinition>();
            this.ContextMenus = new List<AddonMenuDefinition>();
            this.Panels = new List<PanelDefinition>();
        }
    }
}