using ModengTerm.Base.Enumerations;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ModengTerm.Base.DataModels
{
    public class SessionOption
    {
        /// <summary>
        /// 选项类型
        /// </summary>
        [EnumDataType(typeof(OptionKeyEnum))]
        [JsonProperty("key")]
        public int Key { get; set; }

        /// <summary>
        /// 选项的值
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
