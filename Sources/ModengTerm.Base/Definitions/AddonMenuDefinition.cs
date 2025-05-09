using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ModengTerm.Base.Definitions
{
    /// <summary>
    /// 定义一个插件菜单
    /// </summary>
    public class AddonMenuDefinition
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
        /// 该菜单所支持的会话类型
        /// </summary>
        [JsonProperty("sessionTypes")]
        public List<int> SessionTypes { get; private set; }

        /// <summary>
        /// 子菜单
        /// </summary>
        [JsonProperty("child")]
        public List<AddonMenuDefinition> Children { get; private set; }

        /// <summary>
        /// 根菜单的Id
        /// 空表示自己就是根菜单
        /// </summary>
        [JsonProperty("pid")]
        public string ParentId { get; set; }

        /// <summary>
        /// 菜单所对应的插件Id
        /// </summary>
        [JsonIgnore]
        public string AddonId { get; set; }

        public AddonMenuDefinition()
        {
            this.SessionTypes = new List<int>();
            this.Children = new List<AddonMenuDefinition>();
        }
    }
}
