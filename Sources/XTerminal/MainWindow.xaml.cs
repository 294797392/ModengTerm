using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFToolkit.Utility;
using XTerminal.Base;
using XTerminal.Base.DataModels;
using XTerminal.UserControls;

namespace XTerminal
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// SessionID -> TerminalControl
        /// </summary>
        private Dictionary<string, TerminalUserControl> termControlCache;

        public MainWindow()
        {
            InitializeComponent();

            this.InitializeWindow();
        }

        private void InitializeWindow()
        {
            this.termControlCache = new Dictionary<string, TerminalUserControl>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SessionListWindow sessionListWindow = new SessionListWindow();
            sessionListWindow.Owner = this;
            if ((bool)sessionListWindow.ShowDialog())
            {
                XTermSession session = sessionListWindow.SelectedSession;

                // 创建TerminalControl
                TerminalUserControl terminalControl = this.GetTerminalControl(session.ID);
                ContentControlTerminal.Content = terminalControl;

                // 打开Session
                int code = XTermApp.Context.OpenSession(session, terminalControl.CanvasPanel);
                if (code != ResponseCode.SUCCESS)
                {
                    MessageBoxUtils.Error("打开会话失败, {0}", ResponseCode.GetMessage(code));
                }
            }
        }

        private TerminalUserControl GetTerminalControl(string sessionID)
        {
            TerminalUserControl terminalUserControl;
            if (!this.termControlCache.TryGetValue(sessionID, out terminalUserControl))
            {
                terminalUserControl = new TerminalUserControl();
                this.termControlCache[sessionID] = terminalUserControl;
            }
            return terminalUserControl;
        }
    }
}
