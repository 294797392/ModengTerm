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
using XTerminal.Base.DataModels;
using XTerminal.ViewModels;

namespace XTerminal.Windows
{
    /// <summary>
    /// TerminalWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WorkbenchWindow : Window
    {
        private WorkbenchVM viewModel;

        public WorkbenchWindow()
        {
            InitializeComponent();

            this.InitializeWindow();
        }

        private void InitializeWindow()
        {
            this.viewModel = new WorkbenchVM();
        }

        /// <summary>
        /// 打开一个会话
        /// </summary>
        /// <param name="session"></param>
        public void OpenSession(Base.DataModels.XTermSession session)
        {
            OpenedSessionVM openedSessionVM = new OpenedSessionVM(session)
            {
                ID = Guid.NewGuid().ToString(),
                Name = session.Name,
                Description = session.Description,
                CanvasPanel = TerminalUserControl.CanvasPanel,
                Status = Session.SessionStatusEnum.Disconnected
            };
            this.viewModel.OpenedSessions.Add(openedSessionVM);
            openedSessionVM.Open();
        }
    }
}
