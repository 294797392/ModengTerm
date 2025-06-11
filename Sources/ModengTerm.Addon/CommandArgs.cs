using ModengTerm.Base;
using ModengTerm.Base.ServiceAgents;
using System.Collections.Generic;
using System.Windows;

namespace ModengTerm.Addons
{
    public class CommandArgs
    {
        public static readonly CommandArgs Instance = new CommandArgs();

        /// <summary>
        /// 事件参数
        /// 不同的事件拥有不同类型的参数
        /// </summary>
        public object Argument { get; set; }

        /// <summary>
        /// 发送命令的窗口
        /// </summary>
        public Window MainWindow { get { return Application.Current.MainWindow; } }

        /// <summary>
        /// 访问服务的代理
        /// </summary>
        public ServiceAgent ServiceAgent { get { return VTApp.Context.ServiceAgent; } }

        /// <summary>
        /// 要执行的命令
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// 触发命令的插件Id
        /// 如果为空，则说明是AddonCommand
        /// </summary>
        public string AddonId { get; set; }

        public CommandArgs() 
        {
        }
    }
}
