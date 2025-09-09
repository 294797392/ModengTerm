using ModengTerm.Addon.Controls;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
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
            if (!uint.TryParse(TextBoxUploadBufferSize.Text, out uploadBufferSize) || uploadBufferSize == 0)
            {
                MTMessageBox.Info("请输入正确的上传缓冲区大小");
                return null;
            }

            if (!uint.TryParse(TextBoxDownloadBufferSize.Text, out downloadBufferSize) || downloadBufferSize == 0)
            {
                MTMessageBox.Info("请输入正确的下载缓冲区大小");
                return null;
            }

            uint threads;
            if (!uint.TryParse(TextBoxWorkingThreads.Text, out threads) || threads == 0)
            {
                MTMessageBox.Info("请输入正确的线程数（1-10）");
                return null;
            }

            if (threads > 10) 
            {
                MTMessageBox.Info("最多支持10个线程同时工作");
                return null;
            }

            Dictionary<string, object> options = new Dictionary<string, object>()
            {
                { PredefinedOptions.FS_TRANS_UPLOAD_BUFFER_SIZE, uploadBufferSize },
                { PredefinedOptions.FS_TRANS_DOWNLOAD_BUFFER_SIZE, downloadBufferSize },
                { PredefinedOptions.FS_TRANS_THREADS, threads }
            };

            return options;
        }

        public void SetOptions(Dictionary<string, object> options)
        {
            TextBoxUploadBufferSize.Text = options.GetOptions<string>(PredefinedOptions.FS_TRANS_UPLOAD_BUFFER_SIZE);
            TextBoxDownloadBufferSize.Text = options.GetOptions<string>(PredefinedOptions.FS_TRANS_DOWNLOAD_BUFFER_SIZE);
            TextBoxWorkingThreads.Text = options.GetOptions<string>(PredefinedOptions.FS_TRANS_THREADS);
        }
    }
}
