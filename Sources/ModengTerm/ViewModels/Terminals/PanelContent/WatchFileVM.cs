using ModengTerm.Base.Enumerations;
using ModengTerm.Terminal;
using ModengTerm.Terminal.FileWatch;
using ModengTerm.Terminal.Session;
using ModengTerm.UserControls.TerminalUserControls;
using ModengTerm.UserControls.TerminalUserControls.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels.Terminals.PanelContent
{
    public class WatchFileVM : SessionPanelContentVM
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("WatchFileVM");

        #endregion

        #region Inner Class

        public class FileStatusVM : ItemViewModel
        {
            private string filePath;

            public string FilePath
            {
                get { return this.filePath; }
                set
                {
                    if (this.filePath != value)
                    {
                        this.filePath = value;
                        this.NotifyPropertyChanged("FilePath");
                    }
                }
            }

            public VideoTerminal VideoTerminal { get; set; }

            public DocumentControl DocumentControl { get; set; }

            public FileWatcher Watcher { get; set; }
        }

        #endregion

        #region 实例变量

        private bool autoScroll;
        private string filePath;
        private object fileListLock;
        private bool fileListChanged;
        private ManualResetEvent watchEvent;
        private bool isRunning;
        private Grid grid;

        #endregion

        #region 属性

        /// <summary>
        /// 当前正在监控的文件列表
        /// </summary>
        public BindableCollection<FileStatusVM> FileList { get; private set; }

        /// <summary>
        /// 是否自动滚动
        /// </summary>
        public bool AutoScroll
        {
            get { return this.autoScroll; }
            set
            {
                if (value != this.autoScroll)
                {
                    this.autoScroll = value;
                    this.NotifyPropertyChanged("AutoScroll");
                }
            }
        }

        /// <summary>
        /// 当前选中的文件路径
        /// </summary>
        public string FilePath
        {
            get { return this.filePath; }
            set
            {
                if (value != this.filePath)
                {
                    this.filePath = value;
                    this.NotifyPropertyChanged("FilePath");
                }
            }
        }

        #endregion

        #region SessionPanelContentVM

        public override void OnInitialize()
        {
            base.OnInitialize();

            this.fileListLock = new object();
            this.FileList = new BindableCollection<FileStatusVM>();
            this.watchEvent = new ManualResetEvent(false);
            this.isRunning = true;
            WatchFileUserControl watchFileUserControl = this.Content as WatchFileUserControl;
            this.grid = watchFileUserControl.GridDocuments;
        }

        public override void OnRelease()
        {
            base.OnRelease();

            this.isRunning = false;
        }

        public override void OnLoaded()
        {
            base.OnLoaded();
        }

        public override void OnUnload()
        {
            base.OnUnload();
        }

        public override void OnReady()
        {

        }

        #endregion

        #region 实例方法

        private FileWatcher CreateFileWatcher()
        {
            switch ((SessionTypeEnum)this.OpenedSession.Session.Type)
            {
                case SessionTypeEnum.SSH: return new SshFileWatcher();
                default: throw new NotImplementedException();
            }
        }

        private FileStatusVM CreateFileStatusVM(string filePath)
        {
            DocumentControl documentControl = new DocumentControl();

            VTOptions vtOptions = new VTOptions()
            {
                AlternateDocument = documentControl,
                MainDocument = documentControl,
                //Session = options.Session,
                Width = this.grid.ActualWidth,
                Height = this.grid.ActualHeight,
                SessionTransport = new SessionTransport()
            };
            VideoTerminal videoTerminal = new VideoTerminal();
            videoTerminal.Initialize(vtOptions);

            FileStatusVM fileStatusVM = new FileStatusVM()
            {
                ID = Guid.NewGuid().ToString(),
                FilePath = filePath,
                Watcher = this.CreateFileWatcher(),
                VideoTerminal = videoTerminal,
                DocumentControl = documentControl
            };

            this.grid.Children.Add(documentControl);

            return fileStatusVM;
        }

        #endregion

        #region 公开接口

        public void AddFile()
        {
            string filePath = this.FilePath;

            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            FileStatusVM fileStatusVM = this.FileList.FirstOrDefault(v => v.FilePath == filePath);
            if (fileStatusVM != null)
            {
                return;
            }

            fileStatusVM = this.CreateFileStatusVM(filePath);
            fileStatusVM.Watcher.Initialize(filePath);

            lock (this.fileListLock)
            {
                this.FileList.Add(fileStatusVM);
                this.fileListChanged = true;
            }

            if (this.FileList.Count == 1)
            {
                Task.Factory.StartNew(this.WatchFileThreadProc);
            }
            else
            {

            }

            this.watchEvent.Set();
        }

        public void DeleteFile()
        {
            FileStatusVM selectedFile = this.FileList.SelectedItem;
            if (selectedFile == null)
            {
                return;
            }

            lock (this.fileListLock)
            {
                this.FileList.Remove(selectedFile);
                this.fileListChanged = true;
            }

            selectedFile.Watcher.Release();

            if (this.FileList.Count == 0)
            {
                this.watchEvent.Reset();
            }
            else
            {
            }
        }

        #endregion

        #region 事件处理器

        private void WatchFileThreadProc()
        {
            List<FileStatusVM> fileList = new List<FileStatusVM>();
            byte[] buffer = new byte[16384];

            while (this.isRunning)
            {
                this.watchEvent.WaitOne();

                if (this.fileListChanged)
                {
                    lock (this.fileListLock)
                    {
                        fileList.Clear();
                        fileList.AddRange(this.FileList);
                        this.fileListChanged = false;
                    }
                }

                foreach (FileStatusVM fileStatus in fileList)
                {
                    FileWatcher watcher = fileStatus.Watcher;

                    int n = 0;

                    try
                    {
                        n = watcher.Read(buffer, 0, buffer.Length);
                        //lines = watcher.ReadLine();
                    }
                    catch (Exception ex)
                    {
                        logger.Error("读取文件内容异常", ex);
                        continue;
                    }

                    if (n <= 0)
                    {
                        continue;
                    }

                    // 显示文件内容
                }

                Thread.Sleep(1000);
            }

            logger.InfoFormat("退出文件监控线程");
        }

        #endregion
    }
}