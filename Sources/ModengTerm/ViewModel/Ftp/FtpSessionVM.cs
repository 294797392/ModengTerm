using DotNEToolkit;
using DotNEToolkit.Packaging;
using log4net.Repository.Hierarchy;
using ModengTerm.Addon;
using ModengTerm.Addon.ClientBridges;
using ModengTerm.Addon.Interactive;
using ModengTerm.Base;
using ModengTerm.Base.Addon;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.DataModels.Ssh;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.Enumerations.Ssh;
using ModengTerm.Base.Metadatas;
using ModengTerm.Ftp;
using ModengTerm.Ftp.DataModels;
using ModengTerm.Ftp.Enumerations;
using ModengTerm.Ftp.FileSystems;
using ModengTerm.Terminal.Modem;
using ModengTerm.ViewModel.Session;
using ModengTerm.Windows.Ftp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using WPFToolkit.DragDrop;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel.Ftp
{
    /// <summary>
    /// 文件传输会话ViewModel
    /// </summary>
    public class FtpSessionVM : OpenedSessionVM, IClientFtpTab
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("FtpSessionVM");

        /// <summary>
        /// 定义要删除的服务器文件
        /// </summary>
        private class DeleteItem
        {
            /// <summary>
            /// 完整路径
            /// </summary>
            public string FullPath { get; set; }

            /// <summary>
            /// 目录还是文件
            /// </summary>
            public FsItemTypeEnum Type { get; set; }

            public DeleteItem(string fullPath, FsItemTypeEnum type)
            {
                this.FullPath = fullPath;
                this.Type = type;
            }
        }

        #endregion

        #region 实例变量

        private FileListVM serverFileList; // 服务器文件列表
        private FileListVM localFileList; // 客户端文件列表
        private FileSystemTransport serverFsTransport; // 访问服务器文件系统的类
        private FileSystemTransport localFsTransport; // 访问客户端文件系统的类

        private TaskTreeVM taskTree; // 当前正在传输的任务树形列表

        private FtpAgent ftpAgent; // 文件传输代理

        private FileSystemWatcher watcher; // 监控从服务器下载的文件内容是否有变化，如果有变化则实时上传服务器

        #endregion

        #region 属性

        /// <summary>
        /// 本地文件树形列表
        /// </summary>
        public FileListVM ClientFsTree { get { return this.localFileList; } }

        public FileSystemTransport LocalFileSystemTransport { get { return this.localFsTransport; } }

        /// <summary>
        /// 服务器文件树形列表
        /// </summary>
        public FileListVM ServerFsTree { get { return this.serverFileList; } }

        public FileSystemTransport ServerFileSystemTransport { get { return this.serverFsTransport; } }

        /// <summary>
        /// 当前所有在队列里的文件状态
        /// </summary>
        public TaskTreeVM TaskTree { get { return this.taskTree; } }

        #endregion

        #region 构造方法

        public FtpSessionVM(XTermSession session) :
            base(session)
        {
        }

        #endregion

        #region OpenedSessionVM

        protected override int OnOpen()
        {
            this.localFileList = new FileListVM();
            this.localFileList.Context.Role = FtpRoleEnum.Local;
            this.serverFileList = new FileListVM();
            this.serverFileList.Context.Role = FtpRoleEnum.Remote;
            this.taskTree = new TaskTreeVM();

            // 初始化文件列表
            this.serverFileList.CurrentDirectory = this.session.GetOption<string>(PredefinedOptions.FS_GENERAL_SERVER_INITIAL_DIR);
            this.serverFileList.ContextMenus.AddRange(VMUtils.CreateDefaultMenuItems(VTBaseConsts.FtpServerFileItemMenus));
            this.serverFileList.FileListContextMenus.AddRange(VMUtils.CreateDefaultMenuItems(VTBaseConsts.FtpServerFileListMenus));
            this.localFileList.CurrentDirectory = this.session.GetOption<string>(PredefinedOptions.FS_GENERAL_CLIENT_INITIAL_DIR);
            this.localFileList.ContextMenus.AddRange(VMUtils.CreateDefaultMenuItems(VTBaseConsts.FtpClientFileItemMenus));
            this.localFileList.FileListContextMenus.AddRange(VMUtils.CreateDefaultMenuItems(VTBaseConsts.FtpCLientFileListMenus));

            FileSystemOptions options = this.CreateOptions();
            FileSystemTransport transport = new FileSystemTransport();
            transport.StatusChanged += this.Transport_StatusChanged;
            transport.OpenAsync(options);

            this.serverFsTransport = transport;
            this.localFsTransport = this.CreateLocalFsTransport();

            this.ftpAgent = new FtpAgent();
            this.ftpAgent.RemoteFileSystemOptions = options;
            this.ftpAgent.Threads = this.session.GetOption<int>(PredefinedOptions.FS_TRANS_THREADS);
            this.ftpAgent.UploadBufferSize = this.session.GetOption<int>(PredefinedOptions.FS_TRANS_UPLOAD_BUFFER_SIZE) * 1024;
            this.ftpAgent.DownloadBufferSize = this.session.GetOption<int>(PredefinedOptions.FS_TRANS_DOWNLOAD_BUFFER_SIZE) * 1024;
            this.ftpAgent.ProcessStateChanged += this.FtpAgent_ProcessStateChanged;
            this.ftpAgent.Initialize();

            this.InitializeAddressbar(FtpRoleEnum.Local, this.localFileList.CurrentDirectory);
            this.InitializeAddressbar(FtpRoleEnum.Remote, this.serverFileList.CurrentDirectory);
            this.LoadFileListAsync(this.localFileList, this.localFileList.CurrentDirectory);

            return ResponseCode.SUCCESS;
        }

        protected override void OnClose()
        {
            this.ftpAgent.ProcessStateChanged -= this.FtpAgent_ProcessStateChanged;
            this.ftpAgent.Release();

            this.serverFsTransport.StatusChanged -= this.Transport_StatusChanged;
            this.serverFsTransport.Close();
        }

        #endregion

        #region 实例方法

        private FileSystemOptions CreateOptions()
        {
            switch ((SessionTypeEnum)this.session.Type)
            {
                case SessionTypeEnum.Sftp:
                    {
                        return new SftpFileSystemOptions()
                        {
                            AuthenticationType = this.session.GetOption<SSHAuthTypeEnum>(PredefinedOptions.SSH_AUTH_TYPE),
                            UserName = this.session.GetOption<string>(PredefinedOptions.SSH_USER_NAME),
                            Password = this.session.GetOption<string>(PredefinedOptions.SSH_PASSWORD),
                            PrivateKeyId = this.session.GetOption<string>(PredefinedOptions.SSH_PRIVATE_KEY_ID),
                            Passphrase = this.session.GetOption<string>(PredefinedOptions.SSH_Passphrase),
                            ServerPort = this.session.GetOption<int>(PredefinedOptions.SSH_SERVER_PORT),
                            ServerAddress = this.session.GetOption<string>(PredefinedOptions.SSH_SERVER_ADDR),
                            InitialDirectory = this.session.GetOption<string>(PredefinedOptions.FS_GENERAL_SERVER_INITIAL_DIR)
                        };
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        private FileSystemTransport CreateLocalFsTransport()
        {
            Win32FileSystemOptions localFsOptions = new Win32FileSystemOptions();
            localFsOptions.InitialDirectory = this.session.GetOption<string>(PredefinedOptions.FS_GENERAL_CLIENT_INITIAL_DIR);
            FileSystemTransport localFsTransport = new FileSystemTransport();
            localFsTransport.OpenAsync(localFsOptions);
            return localFsTransport;
        }

        #region 本地文件操作

        /// <summary>
        /// 移动本地文件
        /// </summary>
        /// <param name="srcFsItems"></param>
        /// <param name="dstDir"></param>
        private void MoveClientFiles(List<FileItemVM> srcFsItems, string dstDir)
        {
            foreach (FileItemVM fileItem in srcFsItems)
            {
                try
                {
                    if (fileItem.Type == FsItemTypeEnum.Directory)
                    {
                        Directory.Move(fileItem.FullPath, dstDir);
                    }
                    else
                    {
                        string dstFilePath = Path.Combine(dstDir, fileItem.Name);
                        File.Move(fileItem.FullPath, dstFilePath);
                    }

                    this.localFileList.Remove(fileItem);
                }
                catch (Exception ex)
                {
                    logger.Error("MoveFiles异常", ex);
                }
            }
        }

        #endregion

        #region 上传/下载文件

        /// <summary>
        /// 传输文件
        /// </summary>
        /// <param name="srcFileItems">要传输的原始文件列表</param>
        /// <param name="targetDirectory">要传输到的目录</param>
        private void TransferFiles(List<FileItemVM> srcFileItems, string targetDirectory, FileSystemTransport srcFsTransport, bool upload)
        {
            TaskTypeEnum directoryTask = upload ? TaskTypeEnum.CreateDirectory : TaskTypeEnum.CreateLocalDirectory;
            TaskTypeEnum fileTask = upload ? TaskTypeEnum.UploadFile : TaskTypeEnum.DownloadFile;

            List<TaskTreeNodeVM> taskVms = this.CreateTasks(srcFileItems, targetDirectory, srcFsTransport, directoryTask, fileTask);
            List<AgentTask> agentTasks = this.CreateAgentTasks(null, taskVms);
            taskVms.ForEach(v => taskTree.Add(v));
            this.ftpAgent.EnqueueTask(agentTasks);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcDirectory"></param>
        /// <param name="targetDirectory">如果是上传或者下载任务，那么要上传到或者下载到的目录</param>
        /// <param name="srcFsTransport"></param>
        /// <param name="directoryTask"></param>
        /// <param name="fileTask"></param>
        /// <param name="tvctx"></param>
        /// <returns></returns>
        private List<TaskTreeNodeVM> CreateTasksRecursively(string srcDirectory, string targetDirectory, FileSystemTransport srcFsTransport, TaskTypeEnum directoryTask, TaskTypeEnum fileTask)
        {
            List<TaskTreeNodeVM> tasks = new List<TaskTreeNodeVM>();

            List<FsItemInfo> fsItems = srcFsTransport.ListItems(srcDirectory);

            foreach (FsItemInfo fsInfo in fsItems)
            {
                TaskTreeNodeVM taskVm = new TaskTreeNodeVM(this.taskTree.Context)
                {
                    ID = Guid.NewGuid().ToString(),
                    Name = fsInfo.FullPath,
                    SourceFullPath = fsInfo.FullPath,
                    TargetFullPath = string.Format("{0}/{1}", targetDirectory, fsInfo.Name),
                    SourceItemType = fsInfo.Type
                };

                if (fsInfo.Type == FsItemTypeEnum.Directory)
                {
                    taskVm.Icon = Icons.Folder;
                }
                else
                {
                    taskVm.Icon = Icons.GetFileIcon(fsInfo.FullPath);
                }

                tasks.Add(taskVm);

                if (taskVm.SourceItemType == FsItemTypeEnum.Directory)
                {
                    taskVm.OpType = directoryTask;
                    List<TaskTreeNodeVM> subTasks = this.CreateTasksRecursively(fsInfo.FullPath, taskVm.TargetFullPath, srcFsTransport, directoryTask, fileTask);
                    subTasks.ForEach(v => taskVm.Add(v));
                }
                else
                {
                    taskVm.OpType = fileTask;
                }
            }

            return tasks;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcFileItems"></param>
        /// <param name="targetDirectory">如果是上传或者下载任务，那么要上传到或者下载到的目录</param>
        /// <param name="srcFsTransport"></param>
        /// <param name="directoryTask"></param>
        /// <param name="fileTask"></param>
        /// <param name="tvctx"></param>
        /// <returns></returns>
        private List<TaskTreeNodeVM> CreateTasks(List<FileItemVM> srcFileItems, string targetDirectory, FileSystemTransport srcFsTransport, TaskTypeEnum directoryTask, TaskTypeEnum fileTask)
        {
            List<TaskTreeNodeVM> tasks = new List<TaskTreeNodeVM>();

            foreach (FileItemVM srcFileItem in srcFileItems)
            {
                // 不可以上传“返回上级目录”节点
                if (srcFileItem.Type == FsItemTypeEnum.ParentDirectory)
                {
                    continue;
                }

                TaskTreeNodeVM taskVm = new TaskTreeNodeVM(this.taskTree.Context)
                {
                    ID = Guid.NewGuid().ToString(),
                    Name = srcFileItem.FullPath,
                    SourceFullPath = srcFileItem.FullPath,
                    TargetFullPath = string.Format("{0}/{1}", targetDirectory, srcFileItem.Name),
                    SourceItemType = srcFileItem.Type,
                    Icon = srcFileItem.Icon
                };

                tasks.Add(taskVm);

                if (srcFileItem.Type == FsItemTypeEnum.Directory)
                {
                    taskVm.OpType = directoryTask;
                    List<TaskTreeNodeVM> subTasks = this.CreateTasksRecursively(srcFileItem.FullPath, taskVm.TargetFullPath, srcFsTransport, directoryTask, fileTask);
                    subTasks.ForEach(v => taskVm.Add(v));
                }
                else
                {
                    taskVm.OpType = fileTask;
                }
            }

            return tasks;
        }

        private List<AgentTask> CreateAgentTasks(AgentTask parentTask, IEnumerable<TaskTreeNodeVM> subTasks)
        {
            List<AgentTask> agentTasks = new List<AgentTask>();

            foreach (TaskTreeNodeVM taskVm in subTasks)
            {
                AgentTask agentTask = this.CreateAgentTask(taskVm);
                agentTask.Parent = parentTask;
                List<AgentTask> subAgentTasks = this.CreateAgentTasks(agentTask, taskVm.Children.Cast<TaskTreeNodeVM>());
                agentTask.SubTasks.AddRange(subAgentTasks);
                agentTasks.Add(agentTask);
            }

            return agentTasks;
        }

        #endregion

        private AgentTask CreateAgentTask(TaskTreeNodeVM taskVm)
        {
            return new AgentTask()
            {
                Id = taskVm.ID.ToString(),
                SourceFilePath = taskVm.SourceFullPath,
                TargetFilePath = taskVm.TargetFullPath,
                UserData = taskVm,
                Type = taskVm.OpType
            };
        }

        /// <summary>
        /// 调用ShellAPI打开文件
        /// </summary>
        /// <param name="fullPath">要打开的文件的完整路径</param>
        private void ShellOpenFile(string fullPath)
        {
            int rc = Shell32.ShellExecute(IntPtr.Zero, "open", fullPath, null, null, Shell32.SW_SHOW);
            if (rc > 32)
            {
                // 大于32表示执行成功
            }
            else
            {
                switch (rc)
                {
                    case Shell32.SE_ERR_PNF:
                    case Shell32.SE_ERR_FNF:
                        {
                            // 文件不存在
                            MTMessageBox.Info("打开失败, 文件不存在");
                            break;
                        }

                    case Shell32.SE_ERR_NOASSOC:
                        {
                            // 没有找到关联的应用程序
                            Shell32.OpenAs_RunDLL(IntPtr.Zero, IntPtr.Zero, fullPath, 0);
                            break;
                        }

                    default:
                        {
                            // 打开失败
                            MTMessageBox.Info("打开失败, 错误码:{0}", rc);
                            logger.ErrorFormat("打开文件失败, {0}, 错误码:{1}", fullPath, rc);
                            break;
                        }
                }
            }
        }

        #region 删除服务器目录和文件

        /// <summary>
        /// 删除服务器目录和文件
        /// </summary>
        /// <param name="fileItems">要删除的文件列表</param>
        private void DeleteServerItems(List<DeleteItem> fileItems)
        {
            TaskProgressWindowVM vm = new TaskProgressWindowVM();
            TaskProgressWindow progressWindow = new TaskProgressWindow();
            progressWindow.Owner = Application.Current.MainWindow;
            progressWindow.Title = "删除中...";
            progressWindow.DataContext = vm;
            progressWindow.Loaded += (sender, e) =>
            {
                // 等待窗口Loaded再进行删除
                // 如果删除速度太快会导致删除完了窗口才显示出来，这样窗口就永远不会关闭了

                Task.Factory.StartNew(() =>
                {
                    List<bool> results = new List<bool>();

                    foreach (DeleteItem fileItem in fileItems)
                    {
                        bool success = this.DeleteServerItemsRecursively(fileItem.FullPath, fileItem.Type, vm);

                        if (!success)
                        {
                            // 删除失败
                            logger.ErrorFormat("删除失败");
                        }
                        else
                        {
                            // 删除成功
                            logger.InfoFormat("删除成功");
                        }

                        results.Add(success);
                    }

                    // 是否所有的项目都被删除了
                    bool allSuccess = results.All(x => x);

                    progressWindow.Dispatcher.Invoke(progressWindow.Close);

                    this.LoadFileListAsync(this.serverFileList, this.serverFileList.CurrentDirectory);
                });
            };
            progressWindow.ShowDialog();
        }

        /// <summary>
        /// 递归删除服务器目录
        /// </summary>
        /// <param name="parentItem"></param>
        /// <returns></returns>
        private bool DeleteServerItemsRecursively(string fullPath, FsItemTypeEnum fsType, TaskProgressWindowVM vm)
        {
            if (fsType != FsItemTypeEnum.Directory)
            {
                vm.FilePath = fullPath;
                return this.serverFsTransport.DeleteFile(fullPath);
            }

            List<FsItemInfo> subFiles = this.serverFsTransport.ListItems(fullPath);

            foreach (FsItemInfo subFile in subFiles)
            {
                if (subFile.Type == FsItemTypeEnum.Directory)
                {
                    // 先删子目录
                    if (!this.DeleteServerItemsRecursively(subFile.FullPath, subFile.Type, vm))
                    {
                        return false;
                    }
                }
                else
                {
                    // 文件可以直接删除
                    vm.FilePath = fullPath;
                    if (!this.serverFsTransport.DeleteFile(subFile.FullPath))
                    {
                        return false;
                    }
                }
            }

            // 删除父目录
            vm.FilePath = fullPath;
            if (!this.serverFsTransport.DeleteDirectory(fullPath))
            {
                return false;
            }

            return true;
        }

        #endregion

        /// <summary>
        /// 初始化地址栏
        /// </summary>
        /// <param name="ftpRole"></param>
        /// <param name="directory">当前显示的目录</param>
        private void InitializeAddressbar(FtpRoleEnum ftpRole, string directory)
        {
            FileListVM fileList = ftpRole == FtpRoleEnum.Local ? this.localFileList : this.serverFileList;
            FileSystemTransport fsTransport = ftpRole == FtpRoleEnum.Local ? this.localFsTransport : this.serverFsTransport;
            AddressbarVM adrb = fileList.Addressbar;

            DirectoryVM rootDirectory = new DirectoryVM();
            rootDirectory.ID = VTBaseConsts.RootDirectoryChainId;
            rootDirectory.Name = ftpRole == FtpRoleEnum.Local ? "此电脑" : "远程电脑";
            rootDirectory.FullPath = string.Empty;
            adrb.DirectroyChain.Add(rootDirectory);

            List<FsItemInfo> chain = fsTransport.GetDirectoryChains(directory);

            foreach (FsItemInfo fsItem in chain)
            {
                DirectoryVM dirPart = new DirectoryVM()
                {
                    ID = fsItem.FullPath,
                    Name = fsItem.Name,
                    FullPath = fsItem.FullPath
                };
                adrb.DirectroyChain.Add(dirPart);
            }
        }

        private void LoadAddressbar(FileListVM fileList, FileItemVM dirItem, LoadAddressbarReasons actions)
        {
            AddressbarVM adrb = fileList.Addressbar;

            switch (actions)
            {
                case LoadAddressbarReasons.OpenParentDirectory:
                    {
                        adrb.DirectroyChain.RemoveAt(adrb.DirectroyChain.Count - 1);
                        break;
                    }

                case LoadAddressbarReasons.OpenSubDirectory:
                    {
                        DirectoryVM dir = new DirectoryVM()
                        {
                            ID = dirItem.FullPath,
                            Name = dirItem.Name,
                            FullPath = dirItem.FullPath
                        };
                        adrb.DirectroyChain.Add(dir);
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region 公开接口

        public void LoadFileListAsync(FileListVM fileList, string directory, Action callback = null)
        {
            Task.Factory.StartNew(() =>
            {
                bool localFs = false; // 加载的是不是本地文件列表
                FileSystemTransport transport = null;
                List<FsItemInfo> fsItems = null;

                try
                {
                    if (fileList == this.serverFileList)
                    {
                        transport = this.serverFsTransport;
                    }
                    else
                    {
                        transport = this.localFsTransport;
                        localFs = true;
                    }

                    fsItems = transport.ListItems(directory);
                    if (fsItems == null)
                    {
                        // 此时上层会触发连接断开事件
                        return;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("LoadFsTreeAsync异常", ex);
                    return;
                }

                fileList.CurrentDirectory = directory;

                List<FileItemVM> fileItems = new List<FileItemVM>();

                string parentDirectory = string.Empty;

                // 如果加载的是本地文件列表，那么把返回上级节点加进去
                if (localFs)
                {
                    DirectoryInfo directoryInfo = Directory.GetParent(directory);
                    if (directoryInfo == null)
                    {
                        // 说明没有上级目录了
                    }
                    else
                    {
                        parentDirectory = directoryInfo.FullName;
                    }
                }
                else
                {
                    // 加载的是服务器列表，服务器列表操作系统可能是Windows或者Linux
                    if (directory != "/")
                    {
                        int lastIndex = directory.LastIndexOf('/');
                        parentDirectory = directory.Substring(0, lastIndex);
                        if (string.IsNullOrEmpty(parentDirectory))
                        {
                            parentDirectory = "/";
                        }
                    }
                }

                // 创建“返回上级目录”节点
                if (!string.IsNullOrEmpty(parentDirectory))
                {
                    FileItemVM fileItem = new FileItemVM(fileList.Context);
                    fileItem.ID = Guid.Empty;
                    fileItem.Name = "..";
                    fileItem.Type = FsItemTypeEnum.ParentDirectory;
                    fileItem.FullPath = parentDirectory;
                    fileItem.Icon = Icons.Folder;
                    fileItems.Add(fileItem);
                }

                foreach (FsItemInfo fsItem in fsItems)
                {
                    FileItemVM fileItem = new FileItemVM(fileList.Context);
                    fileItem.ID = fsItem.FullPath;
                    fileItem.Name = fsItem.Name;
                    fileItem.FullPath = fsItem.FullPath;
                    fileItem.Size = fsItem.Size;
                    fileItem.LastUpdateTime = fsItem.LastUpdateTime;
                    fileItem.IsHidden = fsItem.IsHidden;
                    fileItem.IsVisible = !fileItem.IsHidden;
                    fileItem.Type = fsItem.Type;

                    if (fsItem.Type == FsItemTypeEnum.Directory)
                    {
                        fileItem.Icon = Icons.Folder;
                    }
                    else if (fsItem.Type == FsItemTypeEnum.ParentDirectory)
                    {
                        fileItem.Icon = Icons.Folder;
                    }
                    else
                    {
                        fileItem.Icon = Icons.GetFileIcon(fsItem.FullPath);
                    }

                    fileItems.Add(fileItem);
                }

                fileList.TotalHiddens = fileItems.Count(v => v.IsHidden);

                App.Current.Dispatcher.Invoke(() =>
                {
                    fileList.Clear();
                    fileList.Add(fileItems);

                    if (callback != null)
                    {
                        callback();
                    }
                });
            });
        }

        #endregion

        #region Internal

        /// <summary>
        /// 传输文件
        /// </summary>
        /// <param name="srcFileItems">要传输的原始文件列表</param>
        /// <param name="dstFileList">要传输到的目标文件夹，如果为空，则表示当前目录</param>
        internal void TransferFile(FileListVM srcFileList, FileListVM dstFileList, List<FileItemVM> srcFileItems, string targetDirectory)
        {
            FtpRoleEnum srcRole = srcFileList.Context.Role;
            FtpRoleEnum dstRole = dstFileList.Context.Role;

            if (srcRole == FtpRoleEnum.Local)
            {
                if (dstRole == FtpRoleEnum.Local)
                {
                    // 客户端 - 客户端
                    this.MoveClientFiles(srcFileItems, targetDirectory);
                }
                else
                {
                    // 客户端 - 服务器
                    this.TransferFiles(srcFileItems, targetDirectory, this.localFsTransport, true);
                }
            }
            else
            {
                if (dstRole == FtpRoleEnum.Local)
                {
                    // 服务器 - 客户端
                    this.TransferFiles(srcFileItems, targetDirectory, this.serverFsTransport, false);
                }
                else
                {
                    // 服务器 - 服务器
                }
            }
        }

        /// <summary>
        /// 当提交编辑的时候触发
        /// </summary>
        /// <param name="fileList"></param>
        /// <param name="fileItem"></param>
        internal bool OnCommitRename(FileListVM fileList, FileItemVM fileItem)
        {
            fileItem.State = FsItemStates.None;

            if (string.IsNullOrEmpty(fileItem.EditName))
            {
                return true;
            }

            FileSystemTransport fsTransport = null;
            string oldPath = fileItem.FullPath;
            string newPath = string.Format("{0}/{1}", fileList.CurrentDirectory, fileItem.EditName);

            if (fileList == this.localFileList)
            {
                fsTransport = this.localFsTransport;
            }
            else
            {
                fsTransport = this.serverFsTransport;
            }

            bool success = false;

            if (fileItem.Type == FsItemTypeEnum.Directory)
            {
                success = fsTransport.RenameDirectory(oldPath, newPath);
            }
            else
            {
                success = fsTransport.RenameFile(oldPath, newPath);
            }

            if (!success)
            {
                logger.ErrorFormat("修改文件夹/目录名字失败, {0}->{1}", fileItem.Name, fileItem.EditName);
                return false;
            }

            fileItem.Name = fileItem.EditName;

            return true;
        }

        /// <summary>
        /// 当双击文件或者目录的时候触发
        /// </summary>
        /// <param name="fileList"></param>
        /// <param name="fileItem"></param>
        internal void OnDoubleClickFileItem(FileListVM fileList, FileItemVM fileItem)
        {
            if (fileItem == null)
            {
                return;
            }

            switch (fileItem.Type)
            {
                case FsItemTypeEnum.ParentDirectory:
                    {
                        this.LoadFileListAsync(fileList, fileItem.FullPath);
                        this.LoadAddressbar(fileList, fileItem, LoadAddressbarReasons.OpenParentDirectory);
                        break;
                    }

                case FsItemTypeEnum.Directory:
                    {
                        this.LoadFileListAsync(fileList, fileItem.FullPath);
                        this.LoadAddressbar(fileList, fileItem, LoadAddressbarReasons.OpenSubDirectory);
                        break;
                    }

                default:
                    break;
            }
        }


        internal void FtpOpenClientItem()
        {
            FileItemVM fileItem = this.localFileList.SelectedItem as FileItemVM;
            if (fileItem == null)
            {
                return;
            }

            if (fileItem.Type == FsItemTypeEnum.Directory)
            {
                this.LoadFileListAsync(this.localFileList, fileItem.FullPath);
            }
            else if (fileItem.Type == FsItemTypeEnum.File)
            {
                this.ShellOpenFile(fileItem.FullPath);
            }
            else
            {
                logger.ErrorFormat("FtpOpenClientItem, 未处理的ItemType, {0}", fileItem.Type);
            }
        }

        internal void FtpDeleteItem(FtpRoleEnum ftpRole)
        {
            FileListVM fileList = null;
            FileSystemTransport fsTransport = null;

            if (ftpRole == FtpRoleEnum.Local)
            {
                fileList = this.localFileList;
                fsTransport = this.localFsTransport;
            }
            else
            {
                fileList = this.serverFileList;
                fsTransport = this.serverFsTransport;
            }

            FileItemVM fileItem = fileList.SelectedItem as FileItemVM;
            if (fileItem == null)
            {
                return;
            }

            if (!MTMessageBox.Confirm("是否确认删除{0}?", fileItem.Name))
            {
                return;
            }

            if (fileItem.Type == FsItemTypeEnum.Directory)
            {
                if (!fsTransport.DeleteDirectory(fileItem.FullPath))
                {
                    return;
                }
            }
            else
            {
                if (!fsTransport.DeleteFile(fileItem.FullPath))
                {
                    return;
                }
            }

            this.LoadFileListAsync(fileList, fileList.CurrentDirectory);
        }

        internal void FtpUploadClientItem()
        {
            List<FileItemVM> srcFsItems = this.localFileList.Context.SelectedItems.Cast<FileItemVM>().ToList();
            if (srcFsItems.Count == 0)
            {
                return;
            }

            string targetDirectory = this.serverFileList.CurrentDirectory;

            this.TransferFiles(srcFsItems, targetDirectory, this.localFsTransport, true);
        }

        internal void FtpRefreshItems(FtpRoleEnum ftpRole)
        {
            FileListVM fileList = null;
            if (ftpRole == FtpRoleEnum.Local)
            {
                fileList = this.localFileList;
            }
            else
            {
                fileList = this.serverFileList;
            }

            this.LoadFileListAsync(fileList, fileList.CurrentDirectory);
        }


        internal void FtpOpenServerItem()
        {
            FileItemVM fileItem = this.localFileList.SelectedItem as FileItemVM;
            if (fileItem == null)
            {
                return;
            }

            if (fileItem.Type == FsItemTypeEnum.Directory)
            {
                this.LoadFileListAsync(this.serverFileList, fileItem.FullPath);
            }
            else if (fileItem.Type == FsItemTypeEnum.File)
            {
            }
            else
            {
                logger.ErrorFormat("FtpOpenServerItem, 未处理的ItemType, {0}", fileItem.Type);
            }
        }

        internal void FtpDownloadServerItem()
        {
            List<FileItemVM> srcFsItems = this.serverFileList.Context.SelectedItems.Cast<FileItemVM>().ToList();
            if (srcFsItems.Count == 0)
            {
                return;
            }

            string targetDirectory = this.localFileList.CurrentDirectory;

            this.TransferFiles(srcFsItems, targetDirectory, this.serverFsTransport, false);
        }

        internal void FtpDeleteServerItem()
        {
            List<FileItemVM> srcFsItems = this.serverFileList.Context.SelectedItems.Cast<FileItemVM>().ToList();
            if (srcFsItems.Count == 0)
            {
                return;
            }

            if (!MTMessageBox.Confirm("确定要删除所选项目吗?"))
            {
                return;
            }

            List<DeleteItem> deleteItems = srcFsItems.Select(v => new DeleteItem(v.FullPath, v.Type)).ToList();
            this.DeleteServerItems(deleteItems);
        }

        #endregion

        #region 事件处理器

        private void Transport_StatusChanged(object arg1, SessionStatusEnum status)
        {
            logger.InfoFormat("Ftp连接状态发生改变, {0}", status);

            base.Status = status;

            switch (status)
            {
                case SessionStatusEnum.Connected:
                    {
                        logger.InfoFormat("Fs客户端已连接, 开始加载服务器文件列表");
                        this.LoadFileListAsync(this.serverFileList, this.serverFileList.CurrentDirectory);
                        break;
                    }

                case SessionStatusEnum.Disconnected:
                    {
                        break;
                    }

                case SessionStatusEnum.ConnectError:
                    {
                        break;
                    }

                case SessionStatusEnum.Connecting:
                    {
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        private void FtpAgent_ProcessStateChanged(FtpAgent ftpAgent, string taskId, double progress, string serverMessage, string speed, ProcessStates processStates, object userData)
        {
            TaskTreeNodeVM taskVm = userData as TaskTreeNodeVM;

            taskVm.State = processStates;
            taskVm.Message = serverMessage;

            switch (processStates)
            {
                case ProcessStates.ProgressChanged:
                    {
                        taskVm.Progress = progress;
                        taskVm.Speed = speed;
                        break;
                    }

                case ProcessStates.Failure:
                    {
                        break;
                    }

                case ProcessStates.Success:
                    {
                        taskVm.Progress = progress;
                        break;
                    }

                case ProcessStates.Starting:
                    {
                        taskVm.PrevTransferTime = DateTime.Now;
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion
    }
}
