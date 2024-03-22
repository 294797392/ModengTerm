using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.ServiceAgents;
using ModengTerm.ServiceAgents.DataModels;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.ViewModels;
using System;
using System.Collections.Generic;
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

namespace ModengTerm.Terminal.Windows
{
    /// <summary>
    /// OpenRecordWindow.xaml 的交互逻辑
    /// </summary>
    public partial class OpenRecordWindow : Window
    {
        #region 实例变量

        private PlaybackVM playbackVM;

        #endregion

        #region 属性

        /// <summary>
        /// 访问服务的代理
        /// </summary>
        public ServiceAgent ServiceAgent { get; set; }

        /// <summary>
        /// 要显示的录像列表所属的Session
        /// </summary>
        public XTermSession Session { get; set; }

        /// <summary>
        /// 是否显示所有回放列表
        /// 如果是，则显示所有Session的回放列表
        /// 如果否，则只显示传进来的Session的回放列表
        /// </summary>
        public bool DisplayAllPlaybackList { get; set; }

        #endregion

        #region 构造方法

        public OpenRecordWindow()
        {
            InitializeComponent();
        }

        #endregion

        public void InitializeWindow()
        {
            List<PlaybackFile> playbackList = this.ServiceAgent.GetPlaybackFiles(this.Session.ID);

            ComboBoxPlaybackList.ItemsSource = playbackList;
        }

        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            if (this.playbackVM != null)
            {
                // 如果打开了先关闭
                this.playbackVM.Close();
                this.playbackVM = null;
            }

            PlaybackFile playbackFile = ComboBoxPlaybackList.SelectedItem as PlaybackFile;
            if (playbackFile == null)
            {
                MTMessageBox.Info("请选择要回放的文件");
                return;
            }

            string playbackFilePath = TermUtils.GetPlaybackFilePath(playbackFile);
            if (!System.IO.File.Exists(playbackFilePath))
            {
                MTMessageBox.Info("回放文件不存在");
                return;
            }

            TerminalContentUserControl.Session = playbackFile.Session;
            TerminalContentUserControl.Open();

            this.playbackVM = playbackVM;
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            PlaybackFile playbackFile = ComboBoxPlaybackList.SelectedItem as PlaybackFile;
            if (playbackFile == null)
            {
                return;
            }

            //string playbackFilePath = VTUtils.GetPlaybackFilePath(this.Session.ID, playbackFile);
            //if (this.shellSessionVM != null)
            //{
            //    if (this.shellSessionVM.PlaybackFilePath == playbackFilePath)
            //    {
            //        if (this.shellSessionVM.PlaybackStatus != PlaybackStatusEnum.Idle)
            //        {
            //            MTMessageBox.Info("当前回放正在播放中, 无法删除, 请先停止当前回放");
            //            return;
            //        }
            //    }
            //}

            //try
            //{
            //    File.Delete(playbackFilePath);
            //}
            //catch (Exception ex)
            //{

            //}
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.playbackVM != null)
            {
                this.playbackVM.Close();
                this.playbackVM = null;
            }
        }
    }
}
