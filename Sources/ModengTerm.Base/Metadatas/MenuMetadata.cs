using ModengTerm.Base.Enumerations;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ModengTerm.Base.Metadatas
{
    /// <summary>
    /// 定义一个插件菜单
    /// </summary>
    public class MenuMetadata
    {
        /// <summary>
        /// 菜单ID
        /// </summary>
        [JsonProperty("id")]
        public string ID { get; set; }

        /// <summary>
        /// 菜单名字
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 点击菜单所触发的命令
        /// </summary>
        [JsonProperty("command")]
        public string Command { get; set; }

        /// <summary>
        /// 子菜单
        /// </summary>
        [JsonProperty("child")]
        public List<MenuMetadata> Children { get; private set; }

        /// <summary>
        /// 根菜单的Id
        /// 空表示自己就是根菜单
        /// </summary>
        [JsonProperty("pid")]
        public string ParentId { get; set; }

        /// <summary>
        /// 指定菜单在哪些区域里显示
        /// </summary>
        [JsonProperty("scopes")]
        public List<MenuScopeEnum> Scopes { get; set; }

        public MenuMetadata()
        {
            this.Children = new List<MenuMetadata>();
            this.Scopes = new List<MenuScopeEnum>();
        }
    }
}
