using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Base.DataModels.Session
{
    public enum TerminalTypeEnum
    {
        VT100,
        VT220,
        XTerm,
        XTerm256Color
    }

    /// <summary>
    /// 保存终端的配置信息
    /// </summary>
    public class TerminalOptions
    {
        /// <summary>
        /// 终端类型
        /// </summary>
        [EnumDataType(typeof(TerminalTypeEnum))]
        [JsonProperty("type")]
        public int Type { get; set; }

        /// <summary>
        /// 终端的列数
        /// </summary>
        [JsonProperty("columns")]
        public int Columns { get; set; }

        /// <summary>
        /// 终端的行数
        /// </summary>
        [JsonProperty("rows")]
        public int Rows { get; set; }

        /// <summary>
        /// DECAWM模式
        /// </summary>
        [JsonIgnore]
        public bool DECPrivateAutoWrapMode { get; set; }

        public string GetTerminalName()
        {
            switch ((TerminalTypeEnum)this.Type)
            {
                case TerminalTypeEnum.VT100: return "vt100";
                case TerminalTypeEnum.VT220: return "vt220";
                case TerminalTypeEnum.XTerm: return "xterm";
                case TerminalTypeEnum.XTerm256Color: return "xterm-256color";
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
