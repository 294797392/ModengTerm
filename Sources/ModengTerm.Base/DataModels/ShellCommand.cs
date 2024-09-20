using DotNEToolkit.DataModels;
using ModengTerm.Base.Enumerations;
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
        [EnumDataType(typeof(ShellCommandTypeEnum))]
        public int Type { get; set; }
    }
}
