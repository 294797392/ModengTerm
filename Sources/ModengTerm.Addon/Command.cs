using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon
{
    public class Command
    {
        /// <summary>
        /// 命令要调用的委托
        /// </summary>
        public AddonCommandDelegate Delegate { get; set; }

        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public object UserData { get; set; }
    }
}
