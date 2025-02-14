using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Terminal.FileWatch;
using ModengTerm.UserControls;
using ModengTerm.UserControls.TerminalUserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                string encodingName = this.session.GetOption<string>(OptionKeyEnum.TERM_READ_ENCODING, OptionDefaultValues.TERM_READ_ENCODING);
                this.encoding = Encoding.GetEncoding(encodingName);

                switch ((SessionTypeEnum)this.session.Type)
                {
                    case SessionTypeEnum.SSH:
                        {
                            this.watcher = new SshFileWatcher();
                            break;
                        }

                    default:
                        throw new NotImplementedException();
                }

                this.watcher.Initialize(this.filePath);
                this.GenericDocumentUserControl.Initialize(this.session);
            }

            public void Release()
            {
                this.watcher.Release();
                this.GenericDocumentUserControl.Release();
            }

            public int Read(byte[] buffer, int offset, int size)
            {
                if (this.watcher.Avaliable == 0)
                {
                    return 0;
                }

                try
                {
                    return this.watcher.Read(buffer, offset, size);
                }
                catch (Exception ex) 
                {
                    logger.Error("读取文件异常", ex);
                    return -1;
                }
            }

            public void DrawText(byte[] bytes, int size) 
            {
                string text = this.encoding.GetString(bytes, 0, size);
                this.GenericDocumentUserControl.DrawText(text);
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
            this.FileList = new BindableCollection<FileItemVM>();
            this.watchEvent = new ManualResetEvent(false);
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

        private FileItemVM CreateFileItem(string filePath)
        {
            FileItemVM fileItem = new FileItemVM(this.OpenedSession.Session)
            {
                ID = Guid.NewGuid().ToString(),
                FilePath = filePath,
                GenericDocumentUserControl = new GenericDocumentUserControl()
            };

            this.grid.Children.Add(fileItem.GenericDocumentUserControl);

            return fileItem;
        }

        #endregion

        #region 公开接口

        public void AddFile()
        {
            if (string.IsNullOrEmpty(this.filePath))
            {
                return;
            }

            FileItemVM fileItem = this.FileList.FirstOrDefault(v => v.FilePath == this.filePath);
            if (fileItem != null)
            {
                return;
            }

            fileItem = this.CreateFileItem(this.filePath);
            fileItem.Initialize();

            lock (this.fileListLock)
            {
                this.FileList.Add(fileItem);
                this.fileListChanged = true;
            }

            if (this.FileList.Count == 1 && !this.isRunning)
            {
                this.isRunning = true;
                Task.Factory.StartNew(this.WatchFileThreadProc);
            }
            else
            {

            }

            this.watchEvent.Set();
        }

        public void DeleteFile()
        {
            FileItemVM fileItem = this.FileList.SelectedItem;
            if (fileItem == null)
            {
                return;
            }

            lock (this.fileListLock)
            {
                this.FileList.Remove(fileItem);
                this.fileListChanged = true;
            }

            fileItem.Release();

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
            List<FileItemVM> fileList = new List<FileItemVM>();
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