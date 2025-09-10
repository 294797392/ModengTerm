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
        /// 要执行的命令Key
        /// </summary>
        public string CommandKey { get; set; }

        private CommandArgs() 
        {
        }
    }
}
