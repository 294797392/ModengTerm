using ModengTerm.Addon.Controls;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations.Terminal;
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

namespace ModengTerm.UserControls.OptionsUserControl.RawTcp
{
    /// <summary>
    /// RawTcpOptionsUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class RawTcpOptionsUserControl : UserControl, IPreferencePanel
    {
        public RawTcpOptionsUserControl()
        {
            InitializeComponent();

            this.InitializeUserControl();
        }

        private void InitializeUserControl()
        {
            ComboBoxTcpTypes.ItemsSource = VTBaseUtils.GetEnumValues<RawTcpTypeEnum>();
        }

        public Dictionary<string, object> GetOptions()
        {
            if (!VTBaseUtils.IsValidIpAddress(TextBoxAddress.Text))
            {
                MTMessageBox.Info("请输入正确的IP地址");
                return null;
            }

            if (!VTBaseUtils.IsValidNetworkPort(TextBoxPort.Text))
            {
                MTMessageBox.Info("请输入正确的端口号");
                return null;
            }

            Dictionary<string, object> options = new Dictionary<string, object>();

            options[PredefinedOptions.RAW_TCP_TYPE] = (RawTcpTypeEnum)ComboBoxTcpTypes.SelectedItem;
            options[PredefinedOptions.RAW_TCP_ADDRESS] = TextBoxAddress.Text;
            options[PredefinedOptions.RAW_TCP_PORT] = TextBoxPort.Text;

            return options;
        }

        public void SetOptions(Dictionary<string, object> options)
        {
            ComboBoxTcpTypes.SelectedItem = options.GetOptions<RawTcpTypeEnum>(PredefinedOptions.RAW_TCP_TYPE);
            TextBoxAddress.Text = options.GetOptions<string>(PredefinedOptions.RAW_TCP_ADDRESS);
            TextBoxPort.Text = options.GetOptions<string>(PredefinedOptions.RAW_TCP_PORT);
        }
    }
}
