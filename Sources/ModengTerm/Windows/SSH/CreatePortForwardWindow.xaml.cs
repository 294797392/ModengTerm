using ModengTerm.Base;
using ModengTerm.Terminal.DataModels;
using ModengTerm.Terminal.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

namespace ModengTerm.Windows.SSH
{
    /// <summary>
    /// CreatePortForwardWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CreatePortForwardWindow : Window
    {
        /// <summary>
        /// 编辑之后的端口转发信息
        /// </summary>
        public PortForward PortForward { get; private set; }

        public CreatePortForwardWindow(PortForward portForward = null)
        {
            InitializeComponent();

            this.InitializeWindow(portForward);
        }

        private void InitializeWindow(PortForward portForward = null)
        {
            BindableCollection<PortForwardTypeEnum> portForwardTypes = new BindableCollection<PortForwardTypeEnum>();
            portForwardTypes.AddRange(MTermUtils.GetEnumValues<PortForwardTypeEnum>());
            ComboBoxTypes.ItemsSource = portForwardTypes;
            ComboBoxTypes.SelectedIndex = 0;

            if (portForward == null) 
            {
                portForward = new PortForward()
                {
                    ID = Guid.NewGuid().ToString()
                };
            }

            this.PortForward = portForward;

            ComboBoxTypes.SelectedItem = (PortForwardTypeEnum)portForward.Type;
            TextBoxSourceAddress.Text = portForward.SourceAddress;
            TextBoxSourcePort.Text = portForward.SourcePort.ToString();
            TextBoxDestAddress.Text = portForward.DestinationAddress;
            TextBoxDestPort.Text = portForward.DestinationPort.ToString();
            CheckBoxAutoOpen.IsChecked = portForward.AutoOpen;
        }

        private bool CheckIPAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return false;
            }

            IPAddress ipaddr;
            if (!IPAddress.TryParse(address, out ipaddr))
            {
                return false;
            }

            return true;
        }

        private bool CheckPort(string port)
        {
            int p;
            if (!int.TryParse(port, out p))
            {
                return false;
            }

            return p > 0 && p <= 65535;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            base.DialogResult = false;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxTypes.SelectedItem == null)
            {
                MTMessageBox.Info("请选择类型");
                return;
            }

            this.PortForward.Description = TextBoxDescription.Text;
            this.PortForward.AutoOpen = CheckBoxAutoOpen.IsChecked.Value;

            string sourceAddr = TextBoxSourceAddress.Text;
            string sourcePort = TextBoxSourcePort.Text;
            string destAddr = TextBoxDestAddress.Text;
            string destPort = TextBoxDestPort.Text;

            PortForwardTypeEnum portForwardType = (PortForwardTypeEnum)ComboBoxTypes.SelectedItem;

            switch (portForwardType)
            {
                case PortForwardTypeEnum.Remote:
                case PortForwardTypeEnum.Local:
                    {
                        if (!this.CheckIPAddress(sourceAddr))
                        {
                            MTMessageBox.Info("请输入正确的源地址");
                            return;
                        }

                        if (!this.CheckPort(sourcePort))
                        {
                            MTMessageBox.Info("请输入正确的端口号");
                            return;
                        }

                        if (!this.CheckIPAddress(destAddr))
                        {
                            MTMessageBox.Info("请输入正确的目标地址");
                            return;
                        }

                        if (!this.CheckPort(destPort))
                        {
                            MTMessageBox.Info("请输入正确的端口号");
                            return;
                        }

                        this.PortForward.Type = (int)portForwardType;
                        this.PortForward.SourceAddress = sourceAddr;
                        this.PortForward.SourcePort = int.Parse(sourcePort);
                        this.PortForward.DestinationAddress = destAddr;
                        this.PortForward.DestinationPort = int.Parse(destPort);

                        break;
                    }

                case PortForwardTypeEnum.Dynamic:
                    {
                        if (!this.CheckIPAddress(sourceAddr))
                        {
                            MTMessageBox.Info("请输入正确的源地址");
                            return;
                        }

                        if (!this.CheckPort(sourcePort))
                        {
                            MTMessageBox.Info("请输入正确的端口号");
                            return;
                        }

                        this.PortForward.Type = (int)portForwardType;
                        this.PortForward.SourceAddress = sourceAddr;
                        this.PortForward.SourcePort = int.Parse(sourcePort);

                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            base.DialogResult = true;
        }
    }
}
