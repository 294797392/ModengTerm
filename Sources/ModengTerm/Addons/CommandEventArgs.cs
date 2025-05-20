using ModengTerm.Base.ServiceAgents;
using ModengTerm.Terminal.ViewModels;
using ModengTerm.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModengTerm.Addons
{
    public class CommandEventArgs
    {
        public static readonly CommandEventArgs Instance = new CommandEventArgs();

        /// <summary>
        /// 发送命令的窗口
        /// </summary>
        public Window MainWindow { get { return Application.Current.MainWindow; } }

        /// <summary>
        /// 提供控制应用程序的通用接口
        /// </summary>
        public ApplicationManager Manager { get; private set; }

        public ServiceAgent ServiceAgent { get { return MTermApp.Context.ServiceAgent; } }

        /// <summary>
        /// 当前选中的会话
        /// </summary>
        public OpenedSessionVM OpenedSession { get; set; }

        /// <summary>
        /// 执行命令的插件
        /// </summary>
        public string AddonId { get; set; }

        /// <summary>
        /// 要执行的命令
        /// </summary>
        public string Command { get; set; }

        public CommandEventArgs() 
        {
            this.Manager = new ApplicationManager();
        }
    }
}
