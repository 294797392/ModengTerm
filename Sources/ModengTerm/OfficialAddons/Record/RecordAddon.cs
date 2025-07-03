using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addons;
using ModengTerm.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Documents;

namespace ModengTerm.OfficialAddons.Record
{
    public class RecordAddon : AddonModule
    {
        private class RecordContext
        {
            /// <summary>
            /// 存储回放信息的数据模型
            /// </summary>
            public Playback Playback { get; set; }

            /// <summary>
            /// 处理回放信息的数据流
            /// </summary>
            public PlaybackStream Stream { get; set; }

            public RecordOptionsVM Options { get; set; }
        }

        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("RecordAddon");

        private const string KEY_CONTEXT = "recordcontext";

        #endregion

        #region AddonModule

        protected override void OnActive(ActiveContext e)
        {
            this.eventRegistry.SubscribeEvent("onClientTabClosed:ssh|local|serial|tcp", this.OnClientTabClosed);
            this.RegisterCommand("RecordAddon.StartRecord", ExecuteStartRecordCommand);
            this.RegisterCommand("RecordAddon.StopRecord", ExecuteStopRecordCommand);
            this.RegisterCommand("RecordAddon.OpenRecord", ExecuteOpenRecordCommand);
        }

        protected override void OnDeactive()
        {
            this.eventRegistry.UnsubscribeEvent("onClientTabClosed", this.OnClientTabClosed);
        }

        #endregion

        #region 实例方法

        private RecordContext StartRecord(IClientTab tab, RecordOptionsVM options)
        {
            Playback playback = new Playback()
            {
                Name = Path.GetFileNameWithoutExtension(options.FilePath),
                Id = this.GetObjectId(),
                SessionId = tab.ID.ToString(),
                FullPath = options.FilePath
            };

            PlaybackStream stream = new PlaybackStream();
            int code = stream.OpenWrite(options.FilePath);
            if (code != ResponseCode.SUCCESS)
            {
                MTMessageBox.Info("启动录制失败, {0}", code);
                return null;
            }

            if ((code = this.storageService.AddObject<Playback>(playback)) != ResponseCode.SUCCESS)
            {
                stream.Close();
                MTMessageBox.Info("保存录像对象失败, {0}", code);
                return null;
            }

            RecordContext context = new RecordContext()
            {
                Playback = playback,
                Stream = stream,
                Options = options
            };

            tab.SetData(this, KEY_CONTEXT, context);

            this.eventRegistry.SubscribeTabEvent("onTabShellRendered", this.OnShellRendered, tab);

            return context;
        }

        private void StopRecord(IClientTab tab)
        {
            RecordContext context = tab.GetData<RecordContext>(this, KEY_CONTEXT);
            if (context == null) 
            {
                return;
            }

            // TODO：此时文件可能正在被写入，playbackStream里做了异常处理，所以直接这么写
            // 需要优化
            context.Stream.Close();

            tab.SetData(this, KEY_CONTEXT, null);

            this.eventRegistry.UnsubscribeTabEvent("onTabShellRendered", this.OnShellRendered, tab);
        }

        #endregion

        #region 事件处理器

        private void ExecuteStartRecordCommand(CommandArgs e)
        {
            RecordContext context = e.ActiveTab.GetData<RecordContext>(this, KEY_CONTEXT);
            if (context != null)
            {
                MTMessageBox.Info("该会话已经启动录制功能");
                return;
            }

            RecordOptionsWindow recordOptionsWindow = new RecordOptionsWindow(e.ActiveTab);
            recordOptionsWindow.Owner = Application.Current.MainWindow;
            if (!(bool)recordOptionsWindow.ShowDialog())
            {
                return;
            }

            context = this.StartRecord(e.ActiveTab, recordOptionsWindow.Options);
            if (context == null)
            {
                return;
            }
        }

        private void ExecuteStopRecordCommand(CommandArgs e)
        {
            this.StopRecord(e.ActiveTab);
        }

        private void ExecuteOpenRecordCommand(CommandArgs e)
        {
            OpenRecordWindow openRecordWindow = new OpenRecordWindow(e.ActiveTab);
            openRecordWindow.Owner = Application.Current.MainWindow;
            openRecordWindow.Show();
        }

        private void OnShellRendered(TabEventArgs e, object userData)
        {
            TabEventShellRendered shellRendered = e as TabEventShellRendered;

            byte[] frameData = new byte[shellRendered.Length];
            Buffer.BlockCopy(shellRendered.Buffer, 0, frameData, 0, frameData.Length);

            RecordContext context = e.Sender.GetData<RecordContext>(this, KEY_CONTEXT);

            context.Stream.WriteFrame(shellRendered.Timestamp, frameData);
        }

        private void OnClientTabClosed(ClientEventArgs e, object userData) 
        {
            ClientEventTabClosed tabClosed = e as ClientEventTabClosed;

            this.StopRecord(tabClosed.ClosedTab);
        }

        #endregion
    }
}
