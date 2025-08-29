using ModengTerm.Addon.Controls;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
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
using WPFToolkit.Utility;

namespace XTerminal.UserControls.OptionsUserControl
{
    /// <summary>
    /// SerialPortOptionsUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class SerialPortOptionsUserControl : UserControl, IPreferencePanel
    {
        public SerialPortOptionsUserControl()
        {
            InitializeComponent();

            this.InitializeUserControl();
        }

        private void InitializeUserControl()
        {
            ComboBoxPortNames.ItemsSource = SerialPort.GetPortNames();
            ComboBoxBaudRate.ItemsSource = VTBaseConsts.SerialPortBaudRates;
            ComboBoxDataBits.ItemsSource = VTBaseConsts.SerialPortDataBits;
            ComboBoxStopBits.ItemsSource = VTBaseUtils.GetEnumValues<StopBits>();
            ComboBoxParityList.ItemsSource = VTBaseUtils.GetEnumValues<Parity>();
            ComboBoxHandshake.ItemsSource = VTBaseUtils.GetEnumValues<Handshake>();
        }

        public Dictionary<string, object> GetOptions()
        {
            if (ComboBoxPortNames.SelectedItem == null)
            {
                MTMessageBox.Info("请输入正确的端口号");
                return null;
            }

            int baudRate;
            if (ComboBoxBaudRate.SelectedItem == null ||
                !int.TryParse(ComboBoxBaudRate.SelectedItem.ToString(), out baudRate))
            {
                MTMessageBox.Info("请输入正确的波特率");
                return null;
            }

            Dictionary<string, object> options = new Dictionary<string, object>()
            {
                { PredefinedOptions.SERIAL_PORT_NAME, ComboBoxPortNames.SelectedItem.ToString() },
                { PredefinedOptions.SERIAL_PORT_BAUD_RATE, (int)ComboBoxBaudRate.SelectedItem },
                { PredefinedOptions.SERIAL_PORT_DATA_BITS, (int)ComboBoxDataBits.SelectedItem },
                { PredefinedOptions.SERIAL_PORT_STOP_BITS, (StopBits)ComboBoxStopBits.SelectedItem },
                { PredefinedOptions.SERIAL_PORT_PARITY, (Parity)ComboBoxParityList.SelectedItem },
                { PredefinedOptions.SERIAL_PORT_HANDSHAKE, (Handshake)ComboBoxHandshake.SelectedItem }
            };

            return options;
        }

        public void SetOptions(Dictionary<string, object> options)
        {
            ComboBoxPortNames.SelectedItem = options.GetOptions<string>(PredefinedOptions.SERIAL_PORT_NAME);
            ComboBoxBaudRate.SelectedItem = options.GetOptions<int>(PredefinedOptions.SERIAL_PORT_BAUD_RATE);
            ComboBoxDataBits.SelectedItem = options.GetOptions<int>(PredefinedOptions.SERIAL_PORT_DATA_BITS);
            ComboBoxStopBits.SelectedItem = options.GetOptions<StopBits>(PredefinedOptions.SERIAL_PORT_STOP_BITS);
            ComboBoxParityList.SelectedItem = options.GetOptions<Parity>(PredefinedOptions.SERIAL_PORT_PARITY);
            ComboBoxHandshake.SelectedItem = options.GetOptions<Handshake>(PredefinedOptions.SERIAL_PORT_HANDSHAKE);
        }

        private void ButtonRefreshPortList_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxPortNames.ItemsSource = SerialPort.GetPortNames();
        }
    }
}
