using ModengTerm.Addon.Interactive;
using ModengTerm.Base;
using ModengTerm.Base.ServiceAgents;
using System.Collections.Generic;
using System.Windows;

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
        /// 要执行的命令
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// 触发命令的插件Id
        /// 如果为空，则说明是AddonCommand
        /// </summary>
        public string AddonId { get; set; }

        /// <summary>
        /// 当前选中的Tab页
        /// </summary>
        public IClientTab ActiveTab { get; set; }

        private CommandArgs() 
        {
        }
    }
}
