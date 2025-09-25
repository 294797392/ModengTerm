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

        private enum TransferTypeEnum
        {
            /// <summary>
            /// 传输类型是上传文件
            /// </summary>
            Upload,

            /// <summary>
            /// 传输类型是下载文件
            /// </summary>
            Download
        }

        #endregion

        #region 实例变量

        private FileListVM serverFileList; // 服务器文件列表
        private FileListVM localFileList; // 客户端文件列表
        private FsClientTransport serverFsTransport; // 访问服务器文件系统的类
        private FsClientTransport localFsTransport; // 访问客户端文件系统的类

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

            FsClientOptions options = this.CreateOptions();
            FsClientTransport transport = new FsClientTransport();
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

        private FsClientOptions CreateOptions()
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

        private FsClientTransport CreateLocalFsTransport()
        {
            LocalFsClientOptions localFsOptions = new LocalFsClientOptions();
            localFsOptions.InitialDirectory = this.session.GetOption<string>(PredefinedOptions.FS_GENERAL_CLIENT_INITIAL_DIR);
            FsClientTransport clientFsTransport = new FsClientTransport();
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

        #region 文件传输

        /// <summary>
        /// 传输文件
        /// </summary>
        /// <param name="srcFileItems">要传输的原始文件列表</param>
        /// <param name="targetDirectory">要传输到的目录</param>
        private void TransferFiles(List<FileItemVM> srcFileItems, string targetDirectory, FsClientTransport srcClientTransport, TransferTypeEnum transferType)
        {
            List<TaskTreeNodeVM> taskVms = this.CreateTransferTasks(srcFileItems, targetDirectory, srcClientTransport, transferType);
            List<AgentTask> agentTasks = this.CreateAgentTasks(null, taskVms);
            taskVms.ForEach(v => taskTree.AddRootNode(v));
            this.ftpAgent.EnqueueTask(agentTasks);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcDirectory"></param>
        /// <param name="dstDirectory">要上传到哪个目录里</param>
        /// <returns></returns>
        private List<TaskTreeNodeVM> CreateTransferTasksRecursively(string srcDirectory, string dstDirectory, FsClientTransport srcClientTransport, TransferTypeEnum transferType)
        {
            List<TaskTreeNodeVM> tasks = new List<TaskTreeNodeVM>();

            List<FsItemInfo> fsItems = srcClientTransport.ListFiles(srcDirectory);

            foreach (FsItemInfo fsInfo in fsItems)
            {
                TaskTreeNodeVM taskVm = new TaskTreeNodeVM(taskTree.Context)
                {
                    ID = Guid.NewGuid().ToString(),
                    Name = fsInfo.FullPath,
                    SourceFullPath = fsInfo.FullPath,
                    TargetFullPath = string.Format("{0}/{1}", dstDirectory, fsInfo.Name),
                    SourceItemType = fsInfo.Type
                };

                if (fsInfo is DirectoryInfo)
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
                    switch (transferType)
                    {
                        case TransferTypeEnum.Upload: taskVm.OpType = TaskTypeEnum.CreateDirectory; break;
                        case TransferTypeEnum.Download: taskVm.OpType = TaskTypeEnum.CreateLocalDirectory; break;
                        default: throw new NotImplementedException();
                    }
                    List<TaskTreeNodeVM> subTasks = this.CreateTransferTasksRecursively(fsInfo.FullPath, taskVm.TargetFullPath, srcClientTransport, transferType);
                    subTasks.ForEach(v => taskVm.Add(v));
                }
                else
                {
                    switch (transferType)
                    {
                        case TransferTypeEnum.Upload: taskVm.OpType = TaskTypeEnum.UploadFile; break;
                        case TransferTypeEnum.Download: taskVm.OpType = TaskTypeEnum.DownloadFile; break;
                        default: throw new NotImplementedException();
                    }
                }
            }

            return tasks;
        }

        private List<TaskTreeNodeVM> CreateTransferTasks(List<FileItemVM> srcFileItems, string targetDirectory, FsClientTransport srcClientTransport, TransferTypeEnum transferType)
        {
            List<TaskTreeNodeVM> tasks = new List<TaskTreeNodeVM>();

            foreach (FileItemVM srcFileItem in srcFileItems)
            {
                // 不可以上传“返回上级目录”节点
                if (srcFileItem.Type == FsItemTypeEnum.ParentDirectory)
                {
                    continue;
                }

                TaskTreeNodeVM taskVm = new TaskTreeNodeVM(taskTree.Context)
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
                    switch (transferType)
                    {
                        case TransferTypeEnum.Upload: taskVm.OpType = TaskTypeEnum.CreateDirectory; break;
                        case TransferTypeEnum.Download: taskVm.OpType = TaskTypeEnum.CreateLocalDirectory; break;
                        default: throw new NotImplementedException();
                    }
                    List<TaskTreeNodeVM> subTasks = this.CreateTransferTasksRecursively(srcFileItem.FullPath, taskVm.TargetFullPath, srcClientTransport, transferType);
                    subTasks.ForEach(v => taskVm.Add(v));
                }
                else
                {
                    switch (transferType)
                    {
                        case TransferTypeEnum.Upload: taskVm.OpType = TaskTypeEnum.UploadFile; break;
                        case TransferTypeEnum.Download: taskVm.OpType = TaskTypeEnum.DownloadFile; break;
                        default: throw new NotImplementedException();
                    }
                }
            }

            return tasks;
        }

        #endregion

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

        private void ForegroundDownloadFile(string srcFullPath, string dstFullPath)
        {

        }

        #endregion

        #region 公开接口

        public void LoadFileListAsync(FileListVM fsTree, string directory)
        {
            Task.Factory.StartNew(() =>
            {
                bool clientFs = false; // 加载的是不是本地文件列表
                FsClientTransport transport = null;
                List<FsItemInfo> fsItems = null;

                try
                {
                    if (fsTree == this.serverFileList)
                    {
                        transport = this.serverFsTransport;
                    }
                    else
                    {
                        transport = this.localFsTransport;
                        clientFs = true;
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

                fsTree.CurrentDirectory = directory;

                List<FileItemVM> fsItemVms = new List<FileItemVM>();

                // 如果加载的是本地文件列表，那么把返回上级节点加进去
                if (clientFs)
                {
                    // 如果路径后有反斜杠，Directory.GetParent会直接把反斜杠去掉之后就返回，相当于是返回了同一个目录
                    // 如果路径是磁盘根目录（比如C:\），那么就不去掉最后的反斜杠，如果去掉了Directory.GetParent会继续返回软件当前目录
                    if (!directory.EndsWith(":\\"))
                    {
                        if (directory.EndsWith('\\'))
                        {
                            directory = directory.TrimEnd('\\');
                        }
                    }

                    DirectoryInfo directoryInfo = Directory.GetParent(directory);
                    if (directoryInfo == null)
                    {
                        // 说明没有上级目录了
                    }
                    else
                    {
                        FileItemVM fsItemVM = new FileItemVM(fsTree.Context);
                        fsItemVM.ID = directoryInfo.FullName;
                        fsItemVM.Name = "..";
                        fsItemVM.Type = FsItemTypeEnum.ParentDirectory;
                        fsItemVM.FullPath = directoryInfo.FullName;
                        fsItemVM.Icon = IconUtils.GetFolderIcon();
                        fsItemVms.Add(fsItemVM);
                    }
                }

                foreach (FsItemInfo fsItem in fsItems)
                {
                    FileItemVM fsItemVm = new FileItemVM(fsTree.Context);
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

                fsTree.TotalHiddens = fsItemVms.Count(v => v.IsHidden);

                App.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        fsTree.ClearNodes();

                        foreach (FileItemVM fsItemVm in fsItemVms)
                        {
                            fsTree.AddRootNode(fsItemVm);
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
                    this.TransferFiles(srcFileItems, targetDirectory, this.localFsTransport, TransferTypeEnum.Upload);
                }
            }
            else
            {
                if (dstRole == FtpRoleEnum.Client)
                {
                    // 服务器 - 客户端
                    this.TransferFiles(srcFileItems, targetDirectory, this.serverFsTransport, TransferTypeEnum.Download);
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

            FsClientTransport fsTransport = null;
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
            FileListVM fsTree = null;
            FsClientTransport fsTransport = null;

            if (ftpRole == FtpRoleEnum.Client)
            {
                fsTree = this.localFileList;
                fsTransport = this.localFsTransport;
            }
            else
            {
                fsTree = this.serverFileList;
                fsTransport = this.serverFsTransport;
            }

            FileItemVM fsItem = fsTree.SelectedItem as FileItemVM;
            if (fsItem == null)
            {
                return;
            }

            if (!MTMessageBox.Confirm("是否确认删除{0}?", fsItem.Name))
            {
                return;
            }

            if (fsItem.Type == FsItemTypeEnum.Directory)
            {
                if (!fsTransport.DeleteDirectory(fsItem.FullPath))
                {
                    return;
                }
            }
            else
            {
                if (!fsTransport.DeleteFile(fsItem.FullPath))
                {
                    return;
                }
            }

            this.LoadFileListAsync(fsTree, fsTree.CurrentDirectory);
        }

        internal void FtpUploadClientItem()
        {
            List<FileItemVM> srcFsItems = this.localFileList.Context.SelectedItems.Cast<FileItemVM>().ToList();
            if (srcFsItems.Count == 0)
            {
                return;
            }

            string dstDir = this.serverFileList.CurrentDirectory;

            this.TransferFiles(srcFsItems, dstDir, this.localFsTransport, TransferTypeEnum.Upload);
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

