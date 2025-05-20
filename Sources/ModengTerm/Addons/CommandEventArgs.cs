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
        public static readonly string BroadcastAddonId = string.Empty;

        /// <summary>
        /// 发送命令的窗口
        /// </summary>
        public Window MainWindow { get { return Application.Current.MainWindow; } }

        /// <summary>
        /// 提供控制应用程序的通用接口
        /// </summary>
        public ApplicationManager Manager { get; private set; }

        /// <summary>
        /// 访问服务的代理
        /// </summary>
        public ServiceAgent ServiceAgent { get { return MTermApp.Context.ServiceAgent; } }

        /// <summary>
        /// 当前选中的会话
        /// TODO：删除OpenedSession
        /// 插件不可以直接访问ViewModel，防止在不熟悉ViewModel运行流程的情况下对ViewModel做了一些不正确的操作
        /// 提供专门的接口供插件调用
        /// </summary>
        public OpenedSessionVM OpenedSession { get; set; }

        /// <summary>
        /// 执行命令的插件
        /// 如果是广播命令，则为string.Empty
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
