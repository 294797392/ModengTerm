using DotNEToolkit;
using Microsoft.Win32;
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

namespace ModengTerm.UserControls.OptionsUserControl.Session
{
    /// <summary>
    /// CommandLineOptionsUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class LocalConsoleOptionsUserControl : UserControl, IPreferencePanel
    {
        public LocalConsoleOptionsUserControl()
        {
            InitializeComponent();

            this.InitializeUserControl();
        }

        private void InitializeUserControl()
        {
            ComboBoxConsoleEngins.ItemsSource = VTBaseUtils.GetEnumValues<Win32ConsoleEngineEnum>();
            ComboBoxConsoleEngins.SelectedIndex = 0;
        }

        #region 事件处理器

        private void ButtonBrowserCommandLinePath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            if ((bool)(dialog.ShowDialog()))
            {
                TextBoxStartupPath.Text = dialog.FileName;
            }
        }

        private void ButtonBrowserStartupDirectory_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TextBoxStartupDirectory.Text = folderBrowserDialog.SelectedPath;
            }
        }

        #endregion

        #region IPreferencePanel

        public Dictionary<string, object> GetOptions()
        {
            if (string.IsNullOrEmpty(TextBoxStartupPath.Text))
            {
                MTMessageBox.Info("请输入正确的启动路径");
                return null;
            }

            Dictionary<string, object> options = new Dictionary<string, object>()
            {
                { PredefinedOptions.CONSOLE_STARTUP_PATH, TextBoxStartupPath.Text },
                { PredefinedOptions.CONSOLE_STARTUP_ARGUMENT, TextBoxStartupArguments.Text },
                { PredefinedOptions.CONSOLE_STARTUP_DIR, TextBoxStartupDirectory.Text },
                { PredefinedOptions.CONSOLE_ENGINE, ComboBoxConsoleEngins.SelectedItem.ToString() }
            };

            return options;
        }

        public void SetOptions(Dictionary<string, object> options)
        {
            TextBoxStartupPath.Text = options.GetOptions<string>(PredefinedOptions.CONSOLE_STARTUP_PATH);
            TextBoxStartupArguments.Text = options.GetOptions<string>(PredefinedOptions.CONSOLE_STARTUP_ARGUMENT);
            TextBoxStartupDirectory.Text = options.GetOptions<string>(PredefinedOptions.CONSOLE_STARTUP_DIR);
            ComboBoxConsoleEngins.SelectedItem = options.GetOptions<Win32ConsoleEngineEnum>(PredefinedOptions.CONSOLE_ENGINE);
        }

        #endregion
    }
}
