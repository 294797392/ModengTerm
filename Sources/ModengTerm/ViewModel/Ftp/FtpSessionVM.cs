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
using ModengTerm.FileTrans;
using ModengTerm.FileTrans.Clients;
using ModengTerm.FileTrans.DataModels;
using ModengTerm.FileTrans.Enumerations;
using ModengTerm.Ftp.Enumerations;
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

        /// <summary>
        /// 服务器文件树形列表
        /// </summary>
        public FileListVM ServerFsTree { get { return this.serverFileList; } }

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
            this.localFileList.Context.Type = FtpRoleEnum.Client;
            this.serverFileList = new FileListVM();
            this.serverFileList.Context.Type = FtpRoleEnum.Server;
            this.taskTree = new TaskTreeVM();

            this.serverFileList.CurrentDirectory = this.session.GetOption<string>(PredefinedOptions.FS_GENERAL_SERVER_INITIAL_DIR);
            this.localFileList.CurrentDirectory = this.session.GetOption<string>(PredefinedOptions.FS_GENERAL_CLIENT_INITIAL_DIR);

            // 加载树形列表右键菜单
            this.serverFileList.ContextMenus.AddRange(VMUtils.CreateDefaultMenuItems(VTBaseConsts.FtpServerFileListMenus));
            this.localFileList.ContextMenus.AddRange(VMUtils.CreateDefaultMenuItems(VTBaseConsts.FtpClientFileListMenus));

            FileSystemOptions options = this.CreateOptions();
            FileSystemTransport transport = new FileSystemTransport();
            transport.StatusChanged += this.Transport_StatusChanged;
            transport.OpenAsync(options);

            this.serverFsTransport = transport;
            this.localFsTransport = this.CreateLocalFsTransport();

            this.ftpAgent = new FtpAgent();
            this.ftpAgent.ClientOptions = options;
            this.ftpAgent.Threads = this.session.GetOption<int>(PredefinedOptions.FS_TRANS_THREADS);
            this.ftpAgent.UploadBufferSize = this.session.GetOption<int>(PredefinedOptions.FS_TRANS_UPLOAD_BUFFER_SIZE) * 1024;
            this.ftpAgent.DownloadBufferSize = this.session.GetOption<int>(PredefinedOptions.FS_TRANS_DOWNLOAD_BUFFER_SIZE) * 1024;
            this.ftpAgent.ProcessStateChanged += this.FtpAgent_ProcessStateChanged;
            this.ftpAgent.Initialize();

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
                        return new SftpClientOptions()
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
            LocalFsClientOptions localFsOptions = new LocalFsClientOptions();
            localFsOptions.InitialDirectory = this.session.GetOption<string>(PredefinedOptions.FS_GENERAL_CLIENT_INITIAL_DIR);
            FileSystemTransport clientFsTransport = new FileSystemTransport();
            clientFsTransport.OpenAsync(localFsOptions);
            return clientFsTransport;
        }

        #region 本地文件操作

        /// <summary>
        /// 移动本地文件
        /// </summary>
        /// <param name="srcFsItems"></param>
        /// <param name="dstDir"></param>
        private void MoveClientFiles(List<FileItemVM> srcFsItems, string dstDir)
        {
            foreach (FileItemVM fsItem in srcFsItems)
            {
                try
                {
                    if (fsItem.Type == FsItemTypeEnum.Directory)
                    {
                        Directory.Move(fsItem.FullPath, dstDir);
                    }
                    else
                    {
                        string dstFilePath = Path.Combine(dstDir, fsItem.Name);
                        File.Move(fsItem.FullPath, dstFilePath);
                    }

                    this.localFileList.Roots.Remove(fsItem);
                }
                catch (Exception ex)
                {
                    logger.Error("MoveFiles异常", ex);
                }
            }
        }

        #endregion

        /// <summary>
        /// 传输文件
        /// </summary>
        /// <param name="srcFileItems">要传输的原始文件列表</param>
        /// <param name="targetDirectory">要传输到的目录</param>
        private void TransferFiles(List<FileItemVM> srcFileItems, string targetDirectory, FileSystemTransport srcFsTransport, bool upload)
        {
            TaskTypeEnum directoryTask = upload ? TaskTypeEnum.CreateDirectory : TaskTypeEnum.CreateLocalDirectory;
            TaskTypeEnum fileTask = upload ? TaskTypeEnum.UploadFile : TaskTypeEnum.DownloadFile;

            List<TaskTreeNodeVM> taskVms = this.CreateTasks(srcFileItems, targetDirectory, srcFsTransport, directoryTask, fileTask, this.taskTree.Context);
            List<AgentTask> agentTasks = this.CreateAgentTasks(null, taskVms);
            taskVms.ForEach(v => taskTree.AddRootNode(v));
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
        private List<TaskTreeNodeVM> CreateTasksRecursively(string srcDirectory, string targetDirectory, FileSystemTransport srcFsTransport, TaskTypeEnum directoryTask, TaskTypeEnum fileTask, TreeViewModelContext tvctx)
        {
            List<TaskTreeNodeVM> tasks = new List<TaskTreeNodeVM>();

            List<FsItemInfo> fsItems = srcFsTransport.ListFiles(srcDirectory);

            foreach (FsItemInfo fsInfo in fsItems)
            {
                TaskTreeNodeVM taskVm = new TaskTreeNodeVM(tvctx)
                {
                    ID = Guid.NewGuid().ToString(),
                    Name = fsInfo.FullPath,
                    SourceFullPath = fsInfo.FullPath,
                    TargetFullPath = string.Format("{0}/{1}", targetDirectory, fsInfo.Name),
                    SourceItemType = fsInfo.Type
                };

                if (fsInfo.Type == FsItemTypeEnum.Directory)
                {
                    taskVm.Icon = IconUtils.GetFolderIcon();
                }
                else
                {
                    taskVm.Icon = IconUtils.GetIcon(fsInfo.FullPath);
                }

                tasks.Add(taskVm);

                if (taskVm.SourceItemType == FsItemTypeEnum.Directory)
                {
                    taskVm.OpType = directoryTask;
                    List<TaskTreeNodeVM> subTasks = this.CreateTasksRecursively(fsInfo.FullPath, taskVm.TargetFullPath, srcFsTransport, directoryTask, fileTask, tvctx);
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
        private List<TaskTreeNodeVM> CreateTasks(List<FileItemVM> srcFileItems, string targetDirectory, FileSystemTransport srcFsTransport, TaskTypeEnum directoryTask, TaskTypeEnum fileTask, TreeViewModelContext tvctx)
        {
            List<TaskTreeNodeVM> tasks = new List<TaskTreeNodeVM>();

            foreach (FileItemVM srcFileItem in srcFileItems)
            {
                // 不可以上传“返回上级目录”节点
                if (srcFileItem.Type == FsItemTypeEnum.ParentDirectory)
                {
                    continue;
                }

                TaskTreeNodeVM taskVm = new TaskTreeNodeVM(tvctx)
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
                    List<TaskTreeNodeVM> subTasks = this.CreateTasksRecursively(srcFileItem.FullPath, taskVm.TargetFullPath, srcFsTransport, directoryTask, fileTask, tvctx);
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

        /// <summary>
        /// 递归删除服务器目录
        /// </summary>
        /// <param name="parentItem"></param>
        /// <returns></returns>
        private bool DeleteServerItemsRecursively(string fullPath, FsItemTypeEnum fsType)
        {
            if (fsType != FsItemTypeEnum.Directory)
            {
                return this.serverFsTransport.DeleteFile(fullPath);
            }

            List<FsItemInfo> subFiles = this.serverFsTransport.ListFiles(fullPath);

            foreach (FsItemInfo subFile in subFiles)
            {
                if (subFile.Type == FsItemTypeEnum.Directory)
                {
                    // 先删子目录
                    if (!this.DeleteServerItemsRecursively(subFile.FullPath, subFile.Type))
                    {
                        return false;
                    }
                }
                else
                {
                    // 文件可以直接删除
                    if (!this.serverFsTransport.DeleteFile(subFile.FullPath))
                    {
                        return false;
                    }
                }
            }

            // 删除父目录
            if (!this.serverFsTransport.DeleteDirectory(fullPath))
            {
                return false;
            }

            return true;
        }

        #endregion

        #region 公开接口

        public void LoadFileListAsync(FileListVM fileList, string directory)
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

                    fsItems = transport.ListFiles(directory);
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

                List<FileItemVM> fsItemVms = new List<FileItemVM>();

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

                if (!string.IsNullOrEmpty(parentDirectory))
                {
                    FileItemVM fsItemVM = new FileItemVM(fileList.Context);
                    fsItemVM.ID = Guid.Empty;
                    fsItemVM.Name = "..";
                    fsItemVM.Type = FsItemTypeEnum.ParentDirectory;
                    fsItemVM.FullPath = parentDirectory;
                    fsItemVM.Icon = IconUtils.GetFolderIcon();
                    fsItemVms.Add(fsItemVM);
                }

                foreach (FsItemInfo fsItem in fsItems)
                {
                    FileItemVM fsItemVm = new FileItemVM(fileList.Context);
                    fsItemVm.ID = fsItem.FullPath;
                    fsItemVm.Name = fsItem.Name;
                    fsItemVm.FullPath = fsItem.FullPath;
                    fsItemVm.Size = fsItem.Size;
                    fsItemVm.LastUpdateTime = fsItem.LastUpdateTime;
                    fsItemVm.IsHidden = fsItem.IsHidden;
                    fsItemVm.IsVisible = !fsItemVm.IsHidden;
                    if (fsItem.Name == "..")
                    {
                        fsItemVm.Type = FsItemTypeEnum.ParentDirectory;
                    }
                    else
                    {
                        fsItemVm.Type = fsItem.Type;
                    }

                    if (fsItem.Type == FsItemTypeEnum.Directory)
                    {
                        fsItemVm.Icon = IconUtils.GetFolderIcon();
                    }
                    else if (fsItem.Type == FsItemTypeEnum.ParentDirectory)
                    {
                        fsItemVm.Icon = IconUtils.GetFolderIcon();
                    }
                    else
                    {
                        fsItemVm.Icon = IconUtils.GetIcon(fsItem.FullPath);
                    }

                    fsItemVms.Add(fsItemVm);
                }

                fileList.TotalHiddens = fsItemVms.Count(v => v.IsHidden);

                App.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        fileList.ClearNodes();

                        foreach (FileItemVM fsItemVm in fsItemVms)
                        {
                            fileList.AddRootNode(fsItemVm);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error("加载文件列表异常", ex);
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
            FtpRoleEnum srcRole = srcFileList.Context.Type;
            FtpRoleEnum dstRole = dstFileList.Context.Type;

            if (srcRole == FtpRoleEnum.Client)
            {
                if (dstRole == FtpRoleEnum.Client)
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
                if (dstRole == FtpRoleEnum.Client)
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
        /// <param name="fsTree"></param>
        /// <param name="fileItem"></param>
        internal bool OnCommitRename(FileListVM fsTree, FileItemVM fileItem)
        {
            fileItem.State = FsItemStates.None;

            if (string.IsNullOrEmpty(fileItem.EditName))
            {
                return true;
            }

            FileSystemTransport fsTransport = null;
            string oldPath = fileItem.FullPath;
            string newPath = Path.Combine(fsTree.CurrentDirectory, fileItem.EditName);

            if (fsTree == this.localFileList)
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
        /// 当双击的时候触发
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
                case FsItemTypeEnum.Directory:
                    {
                        this.LoadFileListAsync(fileList, fileItem.FullPath);
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

            if (ftpRole == FtpRoleEnum.Client)
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
            if (ftpRole == FtpRoleEnum.Client)
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

            TaskProgressWindowVM vm = new TaskProgressWindowVM();
            TaskProgressWindow progressWindow = new TaskProgressWindow();
            progressWindow.Owner = Application.Current.MainWindow;
            progressWindow.DataContext = vm;
            progressWindow.Loaded += (sender, e) =>
            {
                // 等待窗口Loaded再进行删除
                // 如果删除速度太快会导致删除完了窗口才显示出来，这样窗口就永远不会关闭了

                Task.Factory.StartNew(() =>
                {
                    List<bool> results = new List<bool>();

                    foreach (FileItemVM fileItem in srcFsItems)
                    {
                        bool success = this.DeleteServerItemsRecursively(fileItem.FullPath, fileItem.Type);

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
                });
            };
            progressWindow.ShowDialog();
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
