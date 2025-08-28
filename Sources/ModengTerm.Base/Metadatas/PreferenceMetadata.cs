using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.Base.Metadatas
{
    /// <summary>
    /// 定义配置项元数据信息
    /// </summary>
    public class PreferenceMetadata
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

        //
        // 摘要:
        //     子菜单
        [JsonProperty("child")]
        public List<PreferenceMetadata> Children { get; private set; }

        /// <summary>
        /// 面板所属者
        /// </summary>
        [JsonProperty("scopes")]
        public List<SessionTypeEnum> Scopes { get; private set; }

        /// <summary>
        /// 配置项的默认值
        /// </summary>
        [JsonProperty("defaultOptions")]
        public Dictionary<string, object> DefaultOptions { get; private set; }

        public PreferenceMetadata()
        {
            this.Children = new List<PreferenceMetadata>();
            this.Scopes = new List<SessionTypeEnum>();
            this.DefaultOptions = new Dictionary<string, object>();
        }
    }
}

