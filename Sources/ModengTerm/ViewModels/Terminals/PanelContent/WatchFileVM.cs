using ModengTerm.Base.Enumerations;
using ModengTerm.Document.Drawing;
using ModengTerm.Terminal.FileWatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        }

        #endregion

        #region 实例变量

        private bool autoScroll;
        private string filePath;
        private object fileListLock;
        private bool fileListChanged;
        private Task watchTask;
        private List<string> addedFiles;
        private List<string> deleteFiles;

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

        /// <summary>
        /// 渲染接口
        /// </summary>
        public GraphicsInterface GraphicsInterface { get; set; }

        #endregion

        #region SessionPanelContentVM

        public override void OnInitialize()
        {
            base.OnInitialize();

            this.fileListLock = new object();
            this.addedFiles = new List<string>();
            this.deleteFiles = new List<string>();
            this.FileList = new BindableCollection<FileStatusVM>();
        }

        public override void OnRelease()
        {
            base.OnRelease();
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

        private void WatchFileThreadProc()
        {
            List<FileWatcher> watchers = new List<FileWatcher>();

            while (true)
            {
                if (this.fileListChanged)
                {
                    lock (this.fileListLock)
                    {
                        this.HandleWatchListChanged(watchers);
                        this.fileListChanged = false;
                    }
                }

                if (watchers.Count == 0) 
                {
                    break;
                }

                foreach (FileWatcher watcher in watchers)
                {
                    List<string> lines = null;

                    try
                    {
                        lines = watcher.ReadLine();
                    }
                    catch (Exception ex) 
                    {
                        logger.Error("读取文件内容异常", ex);
                        continue;
                    }

                    if (lines == null || lines.Count == 0) 
                    {
                        continue;
                    }

                    // 显示文件内容
                }

                Thread.Sleep(1000);
            }

            logger.InfoFormat("退出文件监控线程");
        }

        private FileWatcher CreateFileWatcher()
        {
            switch ((SessionTypeEnum)this.OpenedSession.Session.Type)
            {
                case SessionTypeEnum.SSH: return new SshFileWatcher();
                default: throw new NotImplementedException();
            }
        }

        private void HandleWatchListChanged(List<FileWatcher> watchers)
        {
            foreach (string toDelete in this.deleteFiles)
            {
                // 添加了然后又被删除了
                if (this.addedFiles.Remove(toDelete))
                {
                    continue;
                }

                FileWatcher fileWatcher = watchers.FirstOrDefault(v => v.FilePath == toDelete);
                if (fileWatcher != null)
                {
                    watchers.Remove(fileWatcher);
                    fileWatcher.Release();
                }
            }

            foreach (string toAdd in this.addedFiles)
            {
                FileWatcher fileWatcher = this.CreateFileWatcher();
                fileWatcher.Initialize();
                watchers.Add(fileWatcher);
            }

            this.deleteFiles.Clear();
            this.addedFiles.Clear();
        }

        #endregion

        #region 公开接口

        public void DeleteFile()
        {
            FileStatusVM selectedFile = this.FileList.SelectedItem;
            if (selectedFile == null)
            {
                return;
            }

            this.FileList.Remove(selectedFile);

            lock (this.fileListLock) 
            {
                this.deleteFiles.Add(selectedFile.FilePath);
                this.fileListChanged = true;
            }

            if (this.FileList.Count == 0)
            {
                // 等待线程结束，防止线程还没结束的时候又添加了一个监控
                Task.WaitAll(this.watchTask);
            }
            else
            { 
            }
        }

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

            fileStatusVM = new FileStatusVM()
            {
                ID = Guid.NewGuid().ToString(),
                FilePath = filePath
            };

            this.FileList.Add(fileStatusVM);

            lock (this.fileListLock)
            {
                this.addedFiles.Add(filePath);
                this.fileListChanged = true;
            }

            if (this.FileList.Count == 1)
            {
                this.watchTask = Task.Factory.StartNew(this.WatchFileThreadProc);
            }
            else
            {

            }
        }

        #endregion
    }
}