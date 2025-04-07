using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModengTerm
{
    public static class MCommands
    {
        /// <summary>
        /// 快捷输入ShellCommand命令
        /// </summary>
        public static RoutedUICommand SendCommand { get; private set; }

        /// <summary>
        /// 改变PanelItem的显示/隐藏状态
        /// </summary>
        public static RoutedUICommand ChangePanelItemVisibleCommand { get; private set; }

        /// <summary>
        /// 打开会话命令
        /// </summary>
        public static RoutedUICommand OpenSessionCommand { get; private set; }

        static MCommands()
        {
            SendCommand = new RoutedUICommand();
            ChangePanelItemVisibleCommand = new RoutedUICommand();
            OpenSessionCommand = new RoutedUICommand();
        }
    }
}
