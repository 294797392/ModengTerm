using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addons;
using ModengTerm.Base;
using System.Drawing;
using System;
using System.Windows;

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
            this.RegisterCommand("RecordAddon.StartRecord", ExecuteStartRecordCommand);
            this.RegisterCommand("RecordAddon.StopRecord", ExecuteStopRecordCommand);
            this.RegisterCommand("RecordAddon.OpenRecord", ExecuteOpenRecordCommand);
        }

        protected override void OnDeactive()
        {
        }

        #endregion

        #region 实例方法

        private RecordContext StartRecord(IClientTab tab, RecordOptionsVM options)
        {
            Playback playback = new Playback()
            {
                Id = this.GetObjectId(),
                SessionId = tab.ID.ToString(),
                FullPath = options.FullPath
            };

            PlaybackStream stream = new PlaybackStream();
            int code = stream.OpenWrite(options.FullPath);
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

            this.eventRegistry.SubscribeTabEvent(TabEvent.SHELL_RENDERED, this.OnShellRendered, tab);
            this.eventRegistry.SubscribeTabEvent(TabEvent.TAB_CLOSED, this.OnShellClosed, tab);

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

            this.eventRegistry.UnsubscribeTabEvent(TabEvent.SHELL_RENDERED, this.OnShellRendered, tab);
            this.eventRegistry.UnsubscribeTabEvent(TabEvent.TAB_CLOSED, this.OnShellClosed, tab);
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

            RecordOptionsVM options = new RecordOptionsVM();

            RecordOptionsWindow recordOptionsWindow = new RecordOptionsWindow();
            recordOptionsWindow.Owner = Application.Current.MainWindow;
            recordOptionsWindow.DataContext = options;
            if (!(bool)recordOptionsWindow.ShowDialog())
            {
                return;
            }

            context = this.StartRecord(e.ActiveTab, options);
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
            throw new RefactorImplementedException();
            //ShellSessionVM shellSessionVM = VTApp.Context.MainWindowVM.SelectedSession as ShellSessionVM;
            //OpenRecordWindow openRecordWindow = new OpenRecordWindow(shellSessionVM.Session);
            //openRecordWindow.Owner = Window.GetWindow(shellSessionVM.Content);
            //openRecordWindow.Show();
        }

        private void OnShellRendered(TabEventArgs e)
        {
            TabEventShellRendered shellRendered = e as TabEventShellRendered;

            byte[] frameData = new byte[shellRendered.Length];
            Buffer.BlockCopy(shellRendered.Buffer, 0, frameData, 0, frameData.Length);

            RecordContext context = e.Sender.GetData<RecordContext>(this, KEY_CONTEXT);

            context.Stream.WriteFrame(shellRendered.Timestamp, frameData);
        }

        private void OnShellClosed(TabEventArgs e) 
        {
            this.StopRecord(e.Sender);
        }

        #endregion
    }
}
