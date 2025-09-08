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
    /// TransmissionOptionsUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class TransmissionOptionsUserControl : UserControl, IPreferencePanel
    {
        public TransmissionOptionsUserControl()
        {
            InitializeComponent();
        }

        public Dictionary<string, object> GetOptions()
        {
            uint uploadBufferSize, downloadBufferSize;
            if (!uint.TryParse(TextBoxUploadBufferSize.Text, out uploadBufferSize))
            {
                MTMessageBox.Info("请输入正确的上传缓冲区大小");
                return null;
            }

            if (!uint.TryParse(TextBoxDownloadBufferSize.Text, out downloadBufferSize))
            {
                MTMessageBox.Info("请输入正确的下载缓冲区大小");
                return null;
            }

            Dictionary<string, object> options = new Dictionary<string, object>()
            {
                { PredefinedOptions.FS_TRANS_UPLOAD_BUFFER_SIZE, uploadBufferSize },
                { PredefinedOptions.FS_TRANS_DOWNLOAD_BUFFER_SIZE, downloadBufferSize }
            };

            return options;
        }

        public void SetOptions(Dictionary<string, object> options)
        {
            TextBoxUploadBufferSize.Text = options.GetOptions<string>(PredefinedOptions.FS_TRANS_UPLOAD_BUFFER_SIZE);
            TextBoxDownloadBufferSize.Text = options.GetOptions<string>(PredefinedOptions.FS_TRANS_DOWNLOAD_BUFFER_SIZE);
        }
    }
}
