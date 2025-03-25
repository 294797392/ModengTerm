using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ModengTerm.Base.Definitions
{
    /// <summary>
    /// 指定Panel的方向
    /// </summary>
    public enum SideWindowDock
    {
        Top,
        Bottom,
        Left,
        Right
    }

    public class MenuItemDefinition
    {
        /// <summary>
        /// 菜单ID
        /// </summary>
        [JsonProperty("id")]
        public string ID { get; set; }

        /// <summary>
        /// 标题菜单的ParentID
        /// 如果为空表示没有父菜单，直接显示到根节点
        /// 如果为-1表示在标题上不显示该菜单
        /// </summary>
        [JsonProperty("titleParentId")]
        public string TitleParentID { get; set; }

        /// <summary>
        /// 右键菜单的ParentID
        /// 如果为-1表示不显示右键菜单
        /// </summary>
        [JsonProperty("contextParentId")]
        public string ContextParentID { get; set; }

        /// <summary>
        /// 指定右键菜单的序号
        /// </summary>
        [JsonProperty("contextOrdinal")]
        public int Ordinal { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("handlerEntry")]
        public string HandlerEntry { get; set; }

        /// <summary>
        /// 当点击菜单的时候回调函数
        /// </summary>
        [JsonProperty("clickMethod")]
        public string MethodName { get; set; }

        /// <summary>
        /// 该菜单所支持的会话类型
        /// </summary>
        [JsonProperty("sessionTypes")]
        public List<int> SessionTypes { get; private set; }

        public MenuItemDefinition()
        {
            this.SessionTypes = new List<int>();
            this.Ordinal = 99999;
        }
    }
}
