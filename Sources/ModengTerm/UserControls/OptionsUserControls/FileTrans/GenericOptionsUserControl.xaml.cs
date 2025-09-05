using ModengTerm.Addon.Controls;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
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

namespace ModengTerm.UserControls.OptionsUserControls.FileTrans
{
    /// <summary>
    /// GenericOptionsUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class GenericOptionsUserControl : UserControl, IPreferencePanel
    {
        public GenericOptionsUserControl()
        {
            InitializeComponent();
        }

        #region IPreferencePanel

        public Dictionary<string, object> GetOptions()
        {
            if (string.IsNullOrEmpty(TextBoxServerInitialDirectory.Text))
            {
                MTMessageBox.Info("请输入正确的服务器初始目录");
                return null;
            }

            if (TextBoxServerInitialDirectory.Text == "~/")
            {
                MTMessageBox.Info("无法使用~/作为目录");
                return null;
            }

            if (string.IsNullOrEmpty(TextBoxClientInitialDirectory.Text))
            {
                MTMessageBox.Info("请输入正确的本地初始目录");
                return null;
            }

            Dictionary<string, object> options = new Dictionary<string, object>() 
            {
                { PredefinedOptions.FS_GENERAL_SERVER_INITIAL_DIR, TextBoxServerInitialDirectory.Text },
                { PredefinedOptions.FS_GENERAL_CLIENT_INITIAL_DIR, TextBoxClientInitialDirectory.Text }
            };

            return options;
        }

        public void SetOptions(Dictionary<string, object> options)
        {
            TextBoxServerInitialDirectory.Text = options.GetOptions<string>(PredefinedOptions.FS_GENERAL_SERVER_INITIAL_DIR);
            TextBoxClientInitialDirectory.Text = options.GetOptions<string>(PredefinedOptions.FS_GENERAL_CLIENT_INITIAL_DIR);
        }

        #endregion
    }
}
