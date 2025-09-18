using ModengTerm.Addon.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon
{
    public class CommandArgs
    {
        public static readonly CommandArgs Instance = new CommandArgs();

        /// <summary>
        /// 在注册命令的时候传递的用户数据
        /// </summary>
        public object UserData { get; set; }

        /// <summary>
        /// 要执行的命令Key
        /// </summary>
        public string CommandKey { get; set; }

        /// <summary>
        /// 当前激活的Tab
        /// </summary>
        public IClientTab ActiveTab { get; set; }

        private CommandArgs()
        {
        }
    }

    /// <summary>
    /// 点击顶部菜单栏或者右键菜单所执行的事件处理器
    /// </summary>
    /// <param name="e"></param>
    public delegate void CommandDelegate(CommandArgs e);
}
