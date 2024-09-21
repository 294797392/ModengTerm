using DotNEToolkit.DataModels;
using ModengTerm.Base.Enumerations.Terminal;
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
        public string Command { get; set; }

        /// <summary>
        /// 命令类型
        /// </summary>
        [EnumDataType(typeof(CommandTypeEnum))]
        public int Type { get; set; }
    }
}
