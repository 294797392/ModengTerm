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
        /// 改变PanelItem的显示/隐藏状态
        /// </summary>
        public static RoutedUICommand ChangePanelItemVisibleCommand { get; private set; }

        /// <summary>
        /// 打开会话命令
        /// </summary>
        public static RoutedUICommand OpenSessionCommand { get; private set; }

        /// <summary>
        /// 执行插件命令
        /// </summary>
        public static RoutedUICommand ExecuteAddonCommand { get; private set; }

        static MCommands()
        {
            ChangePanelItemVisibleCommand = new RoutedUICommand();
            OpenSessionCommand = new RoutedUICommand();
            ExecuteAddonCommand = new RoutedUICommand();
        }
    }
}
