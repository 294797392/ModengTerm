using DotNEToolkit.DataModels;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ModengTerm.Base.DataModels
{
    public enum LineTerminators
    {
        None,
        CRLF,
        LF,
        CR
    }

    public class ScriptItem : ModelBase
    {
        [JsonProperty("expect")]
        public string Expect { get; set; }

        [JsonProperty("send")]
        public string Send { get; set; }

        /// <summary>
        /// 换行符
        /// </summary>
        [JsonProperty("terminator")]
        [EnumDataType(typeof(LineTerminators))]
        public int Terminator { get; set; }
    }
}
