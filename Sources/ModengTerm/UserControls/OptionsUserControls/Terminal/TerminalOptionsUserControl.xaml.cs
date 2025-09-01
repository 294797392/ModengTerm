using ModengTerm.Addon.Controls;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Definitions;
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
    /// TerminalOptionsUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class TerminalOptionsUserControl : UserControl, IPreferencePanel
    {
        public TerminalOptionsUserControl()
        {
            InitializeComponent();

            this.InitializeUserControl();
        }

        private void InitializeUserControl()
        {
            ComboBoxSizeModes.ItemsSource = VTBaseUtils.GetEnumValues<TerminalSizeModeEnum>();
            ComboBoxSizeModes.SelectedIndex = 0;
            ComboBoxEncodings.ItemsSource = Encoding.GetEncodings().OrderBy(v => v.DisplayName);
            ComboBoxEncodings.SelectedIndex = 0;
            ComboBoxRightClickActions.ItemsSource = VTBaseUtils.GetEnumValues<RightClickActions>();
            ComboBoxRightClickActions.SelectedIndex = 0;
        }

        public Dictionary<string, object> GetOptions()
        {
            int row, column;
            if (!int.TryParse(TextBoxRows.Text, out row))
            {
                MTMessageBox.Info("请输入正确的终端行数");
                return null;
            }

            if (!int.TryParse(TextBoxColumns.Text, out column))
            {
                MTMessageBox.Info("请输入正确的终端列数");
                return null;
            }

            int rollback;
            if (!int.TryParse(TextBoxMaxRollback.Text, out rollback))
            {
                MTMessageBox.Info("请输入正确的回滚行数");
                return null;
            }

            EncodingInfo encoding = ComboBoxEncodings.SelectedItem as EncodingInfo;
            if (encoding == null) 
            {
                MTMessageBox.Info("请选择编码方式");
                return null;
            }

            int bufferSize;
            if (!int.TryParse(TextBoxBufferSize.Text, out bufferSize))
            {
                MTMessageBox.Info("请输入正确的缓冲区大小");
                return null;
            }

            Dictionary<string, object> options = new Dictionary<string, object>()
            {
                { PredefinedOptions.TERM_ROW, row },
                { PredefinedOptions.TERM_COL, column },
                { PredefinedOptions.TERM_MAX_ROLLBACK, rollback },
                { PredefinedOptions.TERM_DISABLE_BELL, CheckBoxDisableBell.IsChecked.Value },
                { PredefinedOptions.TERM_SIZE_MODE, (TerminalSizeModeEnum)ComboBoxSizeModes.SelectedItem },
                { PredefinedOptions.TERM_ENCODING, encoding.Name },
                { PredefinedOptions.TERM_RIGHT_CLICK_ACTION, (RightClickActions)ComboBoxRightClickActions.SelectedItem },
                { PredefinedOptions.TERM_READ_BUFFER_SIZE, bufferSize }
            };

            return options;
        }

        public void SetOptions(Dictionary<string, object> options)
        {
            TextBoxRows.Text = options.GetOptions<string>(PredefinedOptions.TERM_ROW);
            TextBoxColumns.Text = options.GetOptions<string>(PredefinedOptions.TERM_COL);
            TextBoxMaxRollback.Text = options.GetOptions<string>(PredefinedOptions.TERM_MAX_ROLLBACK);
            CheckBoxDisableBell.IsChecked = options.GetOptions<bool>(PredefinedOptions.TERM_DISABLE_BELL);
            ComboBoxSizeModes.SelectedItem = options.GetOptions<TerminalSizeModeEnum>(PredefinedOptions.TERM_SIZE_MODE);
            string encodingName = options.GetOptions<string>(PredefinedOptions.TERM_ENCODING);
            ComboBoxEncodings.SelectedItem = Encoding.GetEncodings().FirstOrDefault(v => v.Name == encodingName);
            ComboBoxRightClickActions.SelectedItem = options.GetOptions<RightClickActions>(PredefinedOptions.TERM_RIGHT_CLICK_ACTION);
            TextBoxBufferSize.Text = options.GetOptions<string>(PredefinedOptions.TERM_READ_BUFFER_SIZE);
        }

        private void ComboBoxSizeModes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TerminalSizeModeEnum sizeMode = (TerminalSizeModeEnum)ComboBoxSizeModes.SelectedItem;

            switch (sizeMode)
            {
                case TerminalSizeModeEnum.Fixed:
                    {
                        TextBoxRows.IsEnabled = true;
                        TextBoxColumns.IsEnabled = true;
                        break;
                    }

                case TerminalSizeModeEnum.AutoFit:
                    {
                        TextBoxRows.IsEnabled = false;
                        TextBoxColumns.IsEnabled = false;
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
