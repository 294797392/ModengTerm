using DotNEToolkit;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPFToolkit.MVVM;
using XTerminal.Base;

namespace XTerminal.ViewModels.SFTP
{
    public abstract class FileSystemTreeVM : TreeViewModel<FileSystemTreeVMContext>
    {
        #region 实例变量

        private string initialDirectory;
        private string currentDirectory;

        /// <summary>
        /// 记录打开的历史目录
        /// </summary>
        private Queue<string> historyDirs;

        #endregion

        #region 属性

        public SftpClient SftpClient { get; internal set; }

        /// <summary>
        /// 初始目录
        /// </summary>
        public string InitialDirectory
        {
            get { return this.initialDirectory; }
            set
            {
                if (this.initialDirectory != value)
                {
                    this.initialDirectory = value;
                    this.NotifyPropertyChanged("InitialDirectory");
                }
            }
        }

        /// <summary>
        /// 当前目录
        /// </summary>
        public string CurrentDirectory
        {
            get { return this.currentDirectory; }
            set
            {
                if (this.currentDirectory != value)
                {
                    this.currentDirectory = value;
                    this.NotifyPropertyChanged("CurrentDirectory");
                }
            }
        }

        /// <summary>
        /// 后退菜单，显示的名字是...
        /// </summary>
        public DirectoryNodeVM BackupDirectory { get; private set; }

        /// <summary>
        /// 右键菜单列表
        /// </summary>
        public BindableCollection<ContextMenuItemVM> ContextMenus { get; private set; }

        #endregion

        #region 构造方法

        public FileSystemTreeVM()
        {
            this.ContextMenus = new BindableCollection<ContextMenuItemVM>();
            this.historyDirs = new Queue<string>();
        }

        #endregion

        #region 公开接口

        public void Initialize()
        {
            this.CurrentDirectory = this.InitialDirectory;
            this.BackupDirectory = new DirectoryNodeVM(this.Context)
            {
                ID = Guid.Empty,
                Name = "..."
            };

            this.EnterDirectory(this.CurrentDirectory);
        }

        public void Release()
        { }

        #endregion

        #region 实例方法

        /// <summary>
        /// 获取当前目录的上一级目录
        /// </summary>
        /// <returns></returns>
        private string GetParentDirectory(string directory)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            if (directoryInfo.Parent == null)
            {
                return string.Empty;
            }
            else
            {
                return directoryInfo.Parent.FullName;
            }
        }

        /// <summary>
        /// 新增一个历史目录
        /// </summary>
        /// <param name="directory"></param>
        private void AddHistoryDirectory(string directory)
        {
            if (this.historyDirs.Count == XTermConsts.MaxHistoryDirectory)
            {
                this.historyDirs.Dequeue();
            }

            this.historyDirs.Enqueue(directory);
        }

        private void TransferSelectedItem(object parameter)
        {
            FileSystemTreeNodeVM selectedNode = parameter as FileSystemTreeNodeVM;
            if (selectedNode == null)
            {
                return;
            }
        }

        #endregion

        #region 事件处理器

        /// <summary>
        /// 进入某个目录
        /// </summary>
        /// <param name="directory"></param>
        public void EnterDirectory(string directory)
        {
            // 重新加载文件系统树形列表
            this.ClearRootNode();

            this.BackupDirectory.FullPath = this.GetParentDirectory(directory);
            if (!string.IsNullOrEmpty(this.BackupDirectory.FullPath))
            {
                // 没有父目录，说明到顶级目录了
                // 有父目录才把返回上一级加进去
                this.AddRootNode(this.BackupDirectory);
            }

            // 获取子目录列表
            IEnumerable<FileSystemTreeNodeVM> fileList = this.GetDirectory(directory);
            foreach (FileSystemTreeNodeVM fsNode in fileList)
            {
                this.AddRootNode(fsNode);
            }

            // 更新当前目录
            this.CurrentDirectory = directory;
        }

        /// <summary>
        /// 进入当前选中的目录里
        /// </summary>
        public void EnterDirectory()
        {
            FileSystemTreeNodeVM selectedNode = this.Context.SelectedItem as FileSystemTreeNodeVM;
            if (selectedNode == null)
            {
                return;
            }

            if (selectedNode.Type == FileSystemNodeTypeEnum.File)
            {
                // 打开的是文件，什么都不做
                return;
            }

            // 要打开的目录
            string targetDirectory = selectedNode.FullPath;

            this.EnterDirectory(targetDirectory);
        }

        /// <summary>
        /// 进入上级目录
        /// </summary>
        public void EnterParentDirectory()
        {
            string directory = this.GetParentDirectory(this.CurrentDirectory);
            if (string.IsNullOrEmpty(directory))
            {
                // 已经是顶级目录了
                return;
            }

            this.EnterDirectory(directory);
        }

        /// <summary>
        /// 返回到上一个打开的目录
        /// </summary>
        public void ReturnDirectory()
        {
        }

        /// <summary>
        /// 前进到下一个打开的目录
        /// </summary>
        public void ForwardDirectory()
        { }

        #endregion

        #region 抽象方法

        /// <summary>
        /// 加载指定文件节点的子节点
        /// </summary>
        /// <param name="parentDirectory"></param>
        public abstract void AppendSubDirectory(FileSystemTreeNodeVM parentDirectory);

        /// <summary>
        /// 枚举指定目录下的文件和目录列表
        /// </summary>
        /// <param name="directory"></param>
        public abstract IEnumerable<FileSystemTreeNodeVM> GetDirectory(string directory);

        /// <summary>
        /// 传输指定的文件
        /// </summary>
        /// <param name="fsNode"></param>
        /// <returns></returns>
        //public abstract int TransferFile(FileSystemTreeNodeVM fsNode);

        #endregion
    }
}
