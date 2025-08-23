using DotNEToolkit;
using Microsoft.Win32;
using ModengTerm.Addon.Controls;
using ModengTerm.Addon.Interactive;
using ModengTerm.Base;
using ModengTerm.Controls;
using ModengTerm.Terminal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
using System.Windows.Shapes;

namespace ModengTerm.OfficialAddons.Logger
{
    /// <summary>
    /// LoggerOptionsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoggerOptionsPanel : TabedSidePanel
    {
        #region 实例变量

        private LoggerVM viewModel;

        #endregion

        #region 构造方法

        public LoggerOptionsPanel()
        {
            InitializeComponent();

            this.InitializePanel();
        }

        #endregion

        #region 实例方法

        private void InitializePanel()
        {
        }

        #endregion

        #region 事件处理器

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            LoggerAgent.Context.Start(this.viewModel);
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            LoggerAgent.Context.Stop(this.Tab);
        }

        private void ButtonBrowse_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            //dialog.Filter = "文本文件(*.txt)|*.txt|html文件(*.html)|*.html";
            dialog.Filter = "文本文件(*.txt)|*.txt";
            dialog.FileName = TextBoxLogPath.Text;
            if ((bool)dialog.ShowDialog())
            {
                TextBoxLogPath.Text = dialog.FileName;
            }
        }

        private void ButtonOpenDirectory_Click(object sender, RoutedEventArgs e)
        {
            string fullPath = TextBoxLogPath.Text;
            if (string.IsNullOrEmpty(fullPath))
            {
                return;
            }

            if (!File.Exists(fullPath))
            {
                string dir = System.IO.Path.GetDirectoryName(fullPath);
                System.Diagnostics.Process.Start("explorer.exe", dir);
            }
            else
            {
                System.Diagnostics.Process.Start("explorer.exe", string.Format("/select,{0}", fullPath));
            }
        }

        private void ButtonOpenLogFile_Click(object sender, RoutedEventArgs e)
        {
            string fullPath = TextBoxLogPath.Text;
            if (string.IsNullOrEmpty(fullPath))
            {
                return;
            }

            if (!File.Exists(fullPath))
            {
                MTMessageBox.Info("日志文件不存在");
                return;
            }

            System.Diagnostics.Process.Start("explorer.exe", fullPath);
        }

        #endregion

        #region TabedSidePanel

        public override void Initialize()
        {
            this.viewModel = new LoggerVM(this.Tab);
            base.DataContext = this.viewModel;

            string fileName = string.Format("{0}_{1}.log", this.Tab.Name, DateTime.Now.ToString(DateTimeFormat.yyyyMMddhhmmss));
            string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            this.viewModel.FilePath = fullPath;
        }

        public override void Release()
        {
            LoggerAgent.Context.Stop(this.Tab);
        }

        public override void Load()
        {
        }

        public override void Unload()
        {
        }

        #endregion
    }

    public class LoggerStatus2TextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            LoggerStatus status = (LoggerStatus)value;

            switch (status)
            {
                case LoggerStatus.Start: return "记录日志中...";
                case LoggerStatus.Stop: return "请点击开始记录按钮开始记录日志";
                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class LoggerStatus2BrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            LoggerStatus status = (LoggerStatus)value;

            switch (status)
            {
                case LoggerStatus.Start: return Brushes.Green;
                case LoggerStatus.Stop: return Brushes.Red;
                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
