using DotNEToolkit.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base.Enumerations;

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

        public static SessionOption Create<TValue>(OptionKeyEnum key, TValue value)
        {
            SessionOption option = new SessionOption();
            option.Key = (int)key;
            option.Value = value.ToString();
            return option;
        }
    }
}
