using ModengTerm.Addon.Controls;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Document;
using ModengTerm.Document.Enumerations;
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

namespace ModengTerm.UserControls.OptionsUserControls.Terminal
{
    /// <summary>
    /// MouseOptionsUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class MouseOptionsUserControl : UserControl, IPreferencePanel
    {
        public MouseOptionsUserControl()
        {
            InitializeComponent();

            this.InitializeUserControl();
        }

        private void InitializeUserControl()
        {
            ComboBoxCursorSpeeds.ItemsSource = VTBaseUtils.GetEnumValues<VTCursorSpeeds>();
            ComboBoxCursorSpeeds.SelectedIndex = 0;
            ComboBoxCursorStyles.ItemsSource = VTBaseUtils.GetEnumValues<VTCursorStyles>();
            ComboBoxCursorStyles.SelectedIndex = 0;
        }

        public Dictionary<string, object> GetOptions()
        {
            uint scrollDelta;
            if (!uint.TryParse(TextBoxScrollDelta.Text, out scrollDelta))
            {
                MTMessageBox.Info("请输入正确的鼠标滚动行数");
                return null;
            }

            Dictionary<string, object> options = new Dictionary<string, object>() 
            {
                { PredefinedOptions.CURSOR_SPEED, (VTCursorSpeeds)ComboBoxCursorSpeeds.SelectedItem },
                { PredefinedOptions.CURSOR_STYLE, (VTCursorStyles)ComboBoxCursorStyles.SelectedItem },
                { PredefinedOptions.CURSOR_SCROLL_DELTA, scrollDelta },
            };

            return options;
        }

        public void SetOptions(Dictionary<string, object> options)
        {
            ComboBoxCursorSpeeds.SelectedItem = options.GetOptions<VTCursorSpeeds>(PredefinedOptions.CURSOR_SPEED);
            ComboBoxCursorStyles.SelectedItem = options.GetOptions<VTCursorStyles>(PredefinedOptions.CURSOR_STYLE);
            TextBoxScrollDelta.Text = options.GetOptions<string>(PredefinedOptions.CURSOR_SCROLL_DELTA);
        }
    }
}
