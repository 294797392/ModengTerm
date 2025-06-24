using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addons;
using ModengTerm.Base;
using ModengTerm.Controls;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal;
using ModengTerm.UserControls.Terminals;
using System;
using System.Collections.Generic;
using System.Windows;
using WPFToolkit.MVVM;

namespace ModengTerm.OfficialAddons.Record
{
    /// <summary>
    /// OpenRecordWindow.xaml 的交互逻辑
    /// </summary>
    public partial class OpenRecordWindow : MdWindow
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("OpenRecordWindow");

        #region 实例变量

        private bool isPlaying;
        private BindableCollection<Playback> playbacks;
        private IClientTab tab;
        private StorageService storage;

        #endregion

        #region 属性

        #endregion

        #region 构造方法

        public OpenRecordWindow(IClientTab tab)
        {
            InitializeComponent();

            this.InitializeWindow(tab);
        }

        #endregion

        #region 实例方法

        private void InitializeWindow(IClientTab tab)
        {
            this.tab = tab;
            ClientFactory facory = ClientFactory.GetFactory();
            this.storage = facory.GetStorageService();

            List<Playback> playbacks = this.storage.GetObjects<Playback>(this.tab.ID.ToString());
            this.playbacks = new BindableCollection<Playback>();
            this.playbacks.AddRange(playbacks);
            ComboBoxPlaybackList.ItemsSource = this.playbacks;
            ComboBoxPlaybackList.SelectedIndex = 0;
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

            if (!System.IO.File.Exists(playback.FullPath))
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

            int code = this.storage.DeleteObject(playback.Id);
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
