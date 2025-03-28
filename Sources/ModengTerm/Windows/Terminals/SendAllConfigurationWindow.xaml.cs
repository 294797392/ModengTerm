using ModengTerm.Base;
using ModengTerm.Controls;
using ModengTerm.Terminal.ViewModels;
using ModengTerm.ViewModels;
using ModengTerm.ViewModels.Terminals;
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
using WPFToolkit.MVVM;

namespace ModengTerm.Windows.Terminals
{
    /// <summary>
    /// SendAllConfigurationWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SendAllConfigurationWindow : MdWindow
    {
        #region 实例变量

        private ShellSessionVM shellSession;
        private BindableCollection<SyncInputSessionVM> syncSlaves;
        private BindableCollection<SyncInputSessionVM> selectedSlaves;
        private MainWindowVM mainWindowVM;

        #endregion

        #region 构造方法

        public SendAllConfigurationWindow(ShellSessionVM shellSession)
        {
            InitializeComponent();

            this.InitializeWindow(shellSession);
        }

        #endregion

        #region 实例方法

        private void InitializeWindow(ShellSessionVM shellSession)
        {
            this.shellSession = shellSession;
            this.mainWindowVM = MTermApp.Context.MainWindowVM;
            this.syncSlaves = new BindableCollection<SyncInputSessionVM>();
            this.selectedSlaves = new BindableCollection<SyncInputSessionVM>();

            foreach (ShellSessionVM session in this.mainWindowVM.ShellSessions)
            {
                if (session == shellSession)
                {
                    continue;
                }

                SyncInputSessionVM slave = new SyncInputSessionVM()
                {
                    ID = session.ID,
                    Name = session.Name,
                    Description = session.Description,
                };

                this.syncSlaves.Add(slave);

                if (shellSession.SyncInputSessions.FirstOrDefault(v => v.ID == session.ID) != null)
                {
                    this.selectedSlaves.Add(slave);
                }
            }

            ListBoxSlaveList.DataContext = this.syncSlaves;
            ListBoxSelectedSlaves.DataContext = this.selectedSlaves;
        }

        #endregion

        #region 事件处理器

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            this.shellSession.SyncInputSessions.Clear();
            foreach (SyncInputSessionVM syncSlave in this.selectedSlaves)
            {
                this.shellSession.SyncInputSessions.Add(syncSlave);
            }

            base.DialogResult = true;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            base.DialogResult = false;
        }

        private void ButtonAddSlave_Click(object sender, RoutedEventArgs e)
        {
            SyncInputSessionVM selectedSlave = ListBoxSlaveList.SelectedItem as SyncInputSessionVM;
            if (selectedSlave == null) 
            {
                return;
            }

            if (this.selectedSlaves.Contains(selectedSlave))
            {
                return;
            }

            this.selectedSlaves.Add(selectedSlave);
        }

        private void ButtonRemoveSlave_Click(object sender, RoutedEventArgs e)
        {
            SyncInputSessionVM selectedSlave = ListBoxSelectedSlaves.SelectedItem as SyncInputSessionVM;
            if (selectedSlave == null)
            {
                return;
            }

            this.selectedSlaves.Remove(selectedSlave);
        }

        private void MenuItemClearSelected_Click(object sender, RoutedEventArgs e)
        {
            this.selectedSlaves.Clear();
        }

        #endregion
    }
}
