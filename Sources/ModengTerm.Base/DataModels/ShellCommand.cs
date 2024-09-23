using DotNEToolkit.DataModels;
using ModengTerm.Base.Enumerations.Terminal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Base.DataModels
{
    public class ShellCommand : ModelBase
    {
        /// <summary>
        /// 命令内容
        /// </summary>
        [JsonProperty("command")]
        public string Command { get; set; }

        /// <summary>
        /// 命令类型
        /// </summary>
        [JsonProperty("type")]
        [EnumDataType(typeof(CommandTypeEnum))]
        public int Type { get; set; }

        /// <summary>
        /// 是否自动换行
        /// </summary>
        [JsonProperty("crlf")]
        public bool AutoCRLF { get; set; }
    }
}
