using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Terminal.FileWatch;
using ModengTerm.UserControls;
using ModengTerm.UserControls.TerminalUserControls;
using ModengTerm.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using WPFToolkit.MVVM;

namespace ModengTerm.Addons.SystemMonitor
{
    public class WatchFileVM : PanelContentVM
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("WatchFileVM");

        #endregion

        #region Inner Class

        public class FileItemVM : ItemViewModel
        {
            #region 类变量

            private static log4net.ILog logger = log4net.LogManager.GetLogger("FileItemVM");

            #endregion

            #region 实例变量

            private XTermSession session;
            private FileWatcher watcher;
            private string filePath;
            private Encoding encoding;

            #endregion

            #region 属性

            public string FilePath
            {
                get { return filePath; }
                set
                {
                    if (filePath != value)
                    {
                        filePath = value;
                        NotifyPropertyChanged("FilePath");
                    }
                }
            }

            public GenericDocumentUserControl GenericDocumentUserControl { get; set; }

            #endregion

            #region 构造方法

            public FileItemVM(XTermSession session)
            {
                this.session = session;
            }

            #endregion

            #region 公开接口

            public void Initialize()
            {
                string encodingName = session.GetOption(OptionKeyEnum.TERM_READ_ENCODING, OptionDefaultValues.TERM_READ_ENCODING);
                encoding = Encoding.GetEncoding(encodingName);

                switch ((SessionTypeEnum)session.Type)
                {
                    case SessionTypeEnum.SSH:
                        {
                            watcher = new SshFileWatcher();
                            break;
                        }

                    default:
                        throw new NotImplementedException();
                }

                watcher.Initialize(filePath);
                GenericDocumentUserControl.Initialize(session);
            }

            public void Release()
            {
                watcher.Release();
                GenericDocumentUserControl.Release();
            }

            public int Read(byte[] buffer, int offset, int size)
            {
                if (watcher.Avaliable == 0)
                {
                    return 0;
                }

                try
                {
                    return watcher.Read(buffer, offset, size);
                }
                catch (Exception ex)
                {
                    logger.Error("读取文件异常", ex);
                    return -1;
                }
            }

            public void DrawText(byte[] bytes, int size)
            {
                string text = encoding.GetString(bytes, 0, size);
                GenericDocumentUserControl.DrawText(text);
            }

            #endregion
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
        public BindableCollection<FileItemVM> FileList { get; private set; }

        /// <summary>
        /// 是否自动滚动
        /// </summary>
        public bool AutoScroll
        {
            get { return autoScroll; }
            set
            {
                if (value != autoScroll)
                {
                    autoScroll = value;
                    NotifyPropertyChanged("AutoScroll");
                }
            }
        }

        /// <summary>
        /// 当前选中的文件路径
        /// </summary>
        public string FilePath
        {
            get { return filePath; }
            set
            {
                if (value != filePath)
                {
                    filePath = value;
                    NotifyPropertyChanged("FilePath");
                }
            }
        }

        #endregion

        #region SessionPanelContentVM

        public override void OnInitialize()
        {
            base.OnInitialize();

            fileListLock = new object();
            FileList = new BindableCollection<FileItemVM>();
            watchEvent = new ManualResetEvent(false);
            WatchFileUserControl watchFileUserControl = Content as WatchFileUserControl;
            grid = watchFileUserControl.GridDocuments;
        }

        public override void OnRelease()
        {
            base.OnRelease();

            isRunning = false;
        }

        public override void OnLoaded()
        {
            base.OnLoaded();
        }

        public override void OnUnload()
        {
            base.OnUnload();
        }

        //public override void OnReady()
        //{

        //}

        #endregion

        #region 实例方法

        private FileItemVM CreateFileItem(string filePath)
        {
            //FileItemVM fileItem = new FileItemVM(this.OpenedSession.Session)
            //{
            //    ID = Guid.NewGuid().ToString(),
            //    FilePath = filePath,
            //    GenericDocumentUserControl = new GenericDocumentUserControl()
            //};

            //this.grid.Children.Add(fileItem.GenericDocumentUserControl);

            //return fileItem;
            throw new NotImplementedException();
        }

        #endregion

        #region 公开接口

        public void AddFile()
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            FileItemVM fileItem = FileList.FirstOrDefault(v => v.FilePath == filePath);
            if (fileItem != null)
            {
                return;
            }

            fileItem = CreateFileItem(filePath);
            fileItem.Initialize();

            lock (fileListLock)
            {
                FileList.Add(fileItem);
                fileListChanged = true;
            }

            if (FileList.Count == 1 && !isRunning)
            {
                isRunning = true;
                Task.Factory.StartNew(WatchFileThreadProc);
            }
            else
            {

            }

            watchEvent.Set();
        }

        public void DeleteFile()
        {
            FileItemVM fileItem = FileList.SelectedItem;
            if (fileItem == null)
            {
                return;
            }

            lock (fileListLock)
            {
                FileList.Remove(fileItem);
                fileListChanged = true;
            }

            fileItem.Release();

            if (FileList.Count == 0)
            {
                watchEvent.Reset();
            }
            else
            {
            }
        }

        #endregion

        #region 事件处理器

        private void WatchFileThreadProc()
        {
            List<FileItemVM> fileList = new List<FileItemVM>();
            byte[] buffer = new byte[16384];

            while (isRunning)
            {
                watchEvent.WaitOne();

                if (fileListChanged)
                {
                    lock (fileListLock)
                    {
                        fileList.Clear();
                        fileList.AddRange(FileList);
                        fileListChanged = false;
                    }
                }

                foreach (FileItemVM fileItem in fileList)
                {
                    int n = fileItem.Read(buffer, 0, buffer.Length);

                    if (n <= 0)
                    {
                        continue;
                    }

                    // 显示文件内容
                    fileItem.DrawText(buffer, n);
                }

                Thread.Sleep(1000);
            }

            logger.InfoFormat("退出文件监控线程");
        }

        #endregion
    }
}