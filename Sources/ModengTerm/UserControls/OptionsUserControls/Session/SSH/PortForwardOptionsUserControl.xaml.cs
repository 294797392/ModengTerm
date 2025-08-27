using DotNEToolkit;
using ModengTerm.Base;
using ModengTerm.Base.DataModels.Ssh;
using ModengTerm.Terminal.DataModels;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Windows.SSH;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFToolkit.MVVM;

namespace ModengTerm.UserControls.OptionsUserControl.SSH
{
    /// <summary>
    /// PortForwardOptionsUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class PortForwardOptionsUserControl : UserControl
    {
        public PortForwardOptionsUserControl()
        {
            InitializeComponent();
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            CreatePortForwardWindow createPortForwardWindow = new CreatePortForwardWindow();
            createPortForwardWindow.Owner = Window.GetWindow(this);
            if ((bool)createPortForwardWindow.ShowDialog())
            {
                BindableCollection<PortForward> portForwards = DataGridPortForwards.DataContext as BindableCollection<PortForward>;
                PortForward portForward = createPortForwardWindow.PortForward;
                portForwards.Add(portForward);
            }
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            PortForward portForward = DataGridPortForwards.SelectedItem as PortForward;
            if (portForward == null)
            {
                MTMessageBox.Info("请选择要编辑的端口转发规则");
                return;
            }

            CreatePortForwardWindow createPortForwardWindow = new CreatePortForwardWindow(portForward.CopyTo<PortForward>());
            createPortForwardWindow.Owner = Window.GetWindow(this);
            if ((bool)createPortForwardWindow.ShowDialog())
            {
                BindableCollection<PortForward> portForwards = DataGridPortForwards.DataContext as BindableCollection<PortForward>;
                int index = portForwards.IndexOf(portForward);
                portForwards.RemoveAt(index);
                portForwards.Insert(index, portForward);
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            PortForward portForward = DataGridPortForwards.SelectedItem as PortForward;
            if (portForward == null)
            {
                return;
            }

            if (!MTMessageBox.Confirm("确定要删除吗?"))
            {
                return;
            }

            BindableCollection<PortForward> portForwards = DataGridPortForwards.DataContext as BindableCollection<PortForward>;
            portForwards.Remove(portForward);
        }
    }
}

