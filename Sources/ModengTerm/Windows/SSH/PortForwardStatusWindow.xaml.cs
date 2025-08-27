using ModengTerm.Base;
using ModengTerm.Base.Enumerations;
using ModengTerm.Controls;
using ModengTerm.Terminal.DataModels;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.Engines;
using ModengTerm.ViewModel.Terminal;
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
using ModengTerm.Base.Enumerations.Ssh;
using ModengTerm.Base.DataModels.Ssh;

namespace ModengTerm.Windows.SSH
{
    /// <summary>
    /// PortForwardStatusWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PortForwardStatusWindow : MdWindow
    {
        private ShellSessionVM shellSession;

        public PortForwardStatusWindow(ShellSessionVM shellSession)
        {
            InitializeComponent();

            this.InitializeWindow(shellSession);
        }

        private void InitializeWindow(ShellSessionVM shellSession)
        {
            this.shellSession = shellSession;

            this.RefreshList();
        }

        private void RefreshList()
        {
            object result;
            int code = this.shellSession.Control(SSHControlCodes.GetForwardPortStates, null, out result);
            if (code != ResponseCode.SUCCESS)
            {
                return;
            }

            List<PortForwardState> portForwardStates = result as List<PortForwardState>;

            DataGridPortForwards.DataContext = portForwardStates;
            DataGridPortForwards.ItemsSource = portForwardStates;
        }

        private void ButtonStartAll_Click(object sender, RoutedEventArgs e)
        {
            this.shellSession.Control(SSHControlCodes.StartAllPortForward);
            this.RefreshList();
        }

        private void ButtonStopAll_Click(object sender, RoutedEventArgs e)
        {
            this.shellSession.Control(SSHControlCodes.StopAllPortForward);
            this.RefreshList();
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            PortForwardState portForwardState = DataGridPortForwards.SelectedItem as PortForwardState;
            if (portForwardState == null)
            {
                return;
            }

            if (portForwardState.Status == PortForwardStatusEnum.Opened)
            {
                return;
            }

            this.shellSession.Control(SSHControlCodes.StartPortForward, portForwardState);
            this.RefreshList();
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            PortForwardState portForwardState = DataGridPortForwards.SelectedItem as PortForwardState;
            if (portForwardState == null)
            {
                return;
            }

            if (portForwardState.Status != PortForwardStatusEnum.Opened)
            {
                return;
            }

            this.shellSession.Control(SSHControlCodes.StopPortForward, portForwardState);
            this.RefreshList();
        }
    }
}
