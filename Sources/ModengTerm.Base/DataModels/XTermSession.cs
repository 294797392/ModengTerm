using DotNEToolkit;
using DotNEToolkit.DataModels;
using ModengTerm.Base.Enumerations;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;

namespace ModengTerm.Base.DataModels
{
    /// <summary>
    /// 存储会话数据的数据模型
    /// </summary>
    public class XTermSession : ModelBase
    {
        /// <summary>
        /// 要连接的会话类型
        /// </summary>
        [EnumDataType(typeof(SessionTypeEnum))]
        [JsonProperty("type")]
        public int Type { get; set; }

        /// <summary>
        /// 插件配置项
        /// Key由每个插件自己定义
        /// 插件Id -> 插件配置项列表
        /// </summary>
        [JsonProperty("options")]
        public Dictionary<string, Dictionary<string, object>> Options { get; set; }

        /// <summary>
        /// 会话所属的分组Id
        /// 如果为空，则表示没有分组
        /// </summary>
        [JsonProperty("groupId")]
        public string GroupId { get; set; }

        public XTermSession()
        {
            this.Options = new Dictionary<string, Dictionary<string, object>>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public T GetOption<T>(string key)
        {
            Dictionary<string, object> options = this.Options["3738D66F-9B8A-BB45-C823-22DAF39AAAF6"];
            return options.GetOptions<T>(key);
        }
    }
}
