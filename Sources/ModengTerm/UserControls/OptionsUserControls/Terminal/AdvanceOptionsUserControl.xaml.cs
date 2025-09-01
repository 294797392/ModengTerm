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

namespace ModengTerm.UserControls.OptionsUserControl.Terminal
{
    /// <summary>
    /// AdvanceOptionsUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class AdvanceOptionsUserControl : UserControl, IPreferencePanel
    {
        public AdvanceOptionsUserControl()
        {
            InitializeComponent();

            this.InitializeUserControl();
        }

        private void InitializeUserControl()
        {
            ComboBoxRenderModes.ItemsSource = VTBaseUtils.GetEnumValues<RenderModeEnum>();
            ComboBoxRenderModes.SelectedIndex = 0;
        }

        #region IPreferencePanel

        public Dictionary<string, object> GetOptions()
        {
            Dictionary<string, object> options = new Dictionary<string, object>()
            {
                { PredefinedOptions.TERM_ADV_RENDER_MODE, (RenderModeEnum)ComboBoxRenderModes.SelectedItem },
                { PredefinedOptions.TERM_ADV_SEND_COLOR, VTBaseUtils.Color2RgbKey(ColorPickerSendColor.SelectedColor.Value) },
                { PredefinedOptions.TERM_ADV_RECV_COLOR, VTBaseUtils.Color2RgbKey(ColorPickerRecvColor.SelectedColor.Value) },
                { PredefinedOptions.TERM_ADV_RENDER_SEND, CheckBoxDisplaySend.IsChecked.Value },
                { PredefinedOptions.TERM_ADV_CLICK_TO_CURSOR, CheckBoxClickToCursor.IsChecked.Value },
                { PredefinedOptions.TERM_ADV_AUTO_WRAP_MODE, CheckBoxAutoWrapMode.IsChecked.Value }
            };

            return options;
        }

        public void SetOptions(Dictionary<string, object> options)
        {
            ComboBoxRenderModes.SelectedItem = options.GetOptions<RenderModeEnum>(PredefinedOptions.TERM_ADV_RENDER_MODE);
            ColorPickerSendColor.SelectedColor = VTBaseUtils.RgbKey2Color(options.GetOptions<string>(PredefinedOptions.TERM_ADV_SEND_COLOR));
            ColorPickerRecvColor.SelectedColor = VTBaseUtils.RgbKey2Color(options.GetOptions<string>(PredefinedOptions.TERM_ADV_RECV_COLOR));
            CheckBoxDisplaySend.IsChecked = options.GetOptions<bool>(PredefinedOptions.TERM_ADV_RENDER_SEND);
            CheckBoxClickToCursor.IsChecked = options.GetOptions<bool>(PredefinedOptions.TERM_ADV_CLICK_TO_CURSOR);
            CheckBoxAutoWrapMode.IsChecked = options.GetOptions<bool>(PredefinedOptions.TERM_ADV_AUTO_WRAP_MODE);
        }

        #endregion
    }
}
