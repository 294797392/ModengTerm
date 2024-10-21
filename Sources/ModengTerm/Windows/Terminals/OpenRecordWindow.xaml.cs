using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.ServiceAgents;
using ModengTerm.Controls;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.ViewModels;
using ModengTerm.UserControls.Terminals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Printing;
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
using WPFToolkit.MVVM;

namespace ModengTerm.Terminal.Windows
{
    /// <summary>
    /// OpenRecordWindow.xaml 的交互逻辑
    /// </summary>
    public partial class OpenRecordWindow : MdWindow
    {
        #region 实例变量

        private ServiceAgent serviceAgent;
        private bool isPlaying;
        private BindableCollection<Playback> playbacks;

        #endregion

        #region 属性

        #endregion

        #region 构造方法

        public OpenRecordWindow(XTermSession session)
        {
            InitializeComponent();

            this.InitializeWindow(session);
        }

        #endregion

        #region 实例方法

        private void InitializeWindow(XTermSession session)
        {
            this.serviceAgent = MTermApp.Context.ServiceAgent;

            List<Playback> playbackList = this.serviceAgent.GetPlaybacks(session.ID);
            this.playbacks.AddRange(playbackList);

            ComboBoxPlaybackList.ItemsSource = this.playbacks;
        }

        #endregion

        #region 事件处理器

        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            if (this.isPlaying)
            {
                PlaybackUserControl.Close();
                this.isPlaying = false;
            }

            Playback playback = ComboBoxPlaybackList.SelectedItem as Playback;
            if (playback == null)
            {
                MTMessageBox.Info("请选择要回放的文件");
                return;
            }

            string playbackFilePath = VTermUtils.GetPlaybackFilePath(playback);
            if (!System.IO.File.Exists(playbackFilePath))
            {
                MTMessageBox.Info("回放文件不存在");
                return;
            }

            this.isPlaying = true;

            PlaybackUserControl.Open(playback);
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            Playback playback = ComboBoxPlaybackList.SelectedItem as Playback;
            if (playback == null)
            {
                return;
            }

            if (this.isPlaying) 
            {
                MTMessageBox.Info("当前回放正在播放中, 无法删除, 请先停止当前回放");
                return;
            }

            if (!MTMessageBox.Confirm("确定要删除{0}吗?", playback.Name))
            {
                return;
            }

            int code = this.serviceAgent.DeletePlayback(playback.ID);
            if (code != ResponseCode.SUCCESS) 
            {
                MTMessageBox.Info("删除失败, {0}", code);
                return;
            }

            this.playbacks.Remove(playback);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.isPlaying) 
            {
                PlaybackUserControl.Close();

                this.isPlaying = false;
            }
        }

        #endregion
    }
}
