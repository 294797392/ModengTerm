using ModengTerm.Base.Enumerations;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Windows.Media.Animation;

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
        /// 如果该菜单为二级或更高级菜单，那么不会使用这个字段
        /// </summary>
        [JsonProperty("pid")]
        public string ParentId { get; set; }

        /// <summary>
        /// 指定菜单在哪些位置显示
        /// </summary>
        [JsonProperty("positions")]
        public List<MenuPositionEnum> Positions { get; private set; }

        public MenuMetadata()
        {
            this.Children = new List<MenuMetadata>();
            this.Positions = new List<MenuPositionEnum>();
        }

        public MenuMetadata(string name, string command)
        {
            this.ID = Guid.NewGuid().ToString();
            this.Name = name;
            this.Command = command;
            this.Children = new List<MenuMetadata>();
            this.Positions = new List<MenuPositionEnum>();
        }
    }
}
