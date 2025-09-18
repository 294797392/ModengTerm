using DotNEToolkit;
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
using System.Windows.Forms;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel.Ftp
{
    /// <summary>
    /// 文件传输会话ViewModel
    /// </summary>
    public class FtpSessionVM : OpenedSessionVM, IClientFtpTab
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("FsSessionVM");

        #endregion

        #region 实例变量

        private FsTreeVM serverFsTree;
        private FsTreeVM clientFsTree;
        private FsClientTransport serverFsTransport;
        private FsClientTransport clientFsTransport;

        private TaskTreeVM taskTree;

        private FtpAgent ftpAgent;

        #endregion

        #region 属性

        /// <summary>
        /// 本地文件树形列表
        /// </summary>
        public FsTreeVM ClientFsTree { get { return this.clientFsTree; } }

        /// <summary>
        /// 服务器文件树形列表
        /// </summary>
        public FsTreeVM ServerFsTree { get { return this.serverFsTree; } }

        /// <summary>
        /// 当前所有在队列里的文件状态
        /// </summary>
        public TaskTreeVM TaskTree { get { return this.taskTree; } }

        #endregion

        #region 构造方法

        static FtpSessionVM()
        {
            Client.RegisterCommand(FtpCommandKeys.CLIENT_OPEN_ITEM, OnFtpOpenClientItem);
            Client.RegisterCommand(FtpCommandKeys.CLIENT_DELETE_ITEM, OnFtpDeleteClientItem);
        }

        public FtpSessionVM(XTermSession session) :
            base(session)
        {
        }

        #endregion

        #region OpenedSessionVM

        protected override void OnInitialize()
        {
        }

        protected override void OnRelease()
        {
        }

        protected override int OnOpen()
        {
            this.clientFsTree = new FsTreeVM();
            this.clientFsTree.Context.Type = FtpRoleEnum.Client;
            this.serverFsTree = new FsTreeVM();
            this.serverFsTree.Context.Type = FtpRoleEnum.Server;
            this.taskTree = new TaskTreeVM();

            this.serverFsTree.CurrentDirectory = this.session.GetOption<string>(PredefinedOptions.FS_GENERAL_SERVER_INITIAL_DIR);
            this.clientFsTree.CurrentDirectory = this.session.GetOption<string>(PredefinedOptions.FS_GENERAL_CLIENT_INITIAL_DIR);

            // 加载树形列表右键菜单
            this.serverFsTree.ContextMenus.AddRange(VMUtils.CreateDefaultMenuItems(VTBaseConsts.FtpServerFileListMenus));
            this.clientFsTree.ContextMenus.AddRange(VMUtils.CreateDefaultMenuItems(VTBaseConsts.FtpClientFileListMenus));

            FsClientOptions options = this.CreateOptions();
            FsClientTransport transport = new FsClientTransport();
            transport.StatusChanged += this.Transport_StatusChanged;
            transport.OpenAsync(options);

            this.serverFsTransport = transport;
            this.clientFsTransport = this.CreateClientFsTransport();

            this.ftpAgent = new FtpAgent();
            this.ftpAgent.ClientOptions = options;
            this.ftpAgent.Threads = this.session.GetOption<int>(PredefinedOptions.FS_TRANS_THREADS);
            this.ftpAgent.UploadBufferSize = this.session.GetOption<int>(PredefinedOptions.FS_TRANS_UPLOAD_BUFFER_SIZE) * 1024;
            this.ftpAgent.DownloadBufferSize = this.session.GetOption<int>(PredefinedOptions.FS_TRANS_DOWNLOAD_BUFFER_SIZE) * 1024;
            this.ftpAgent.ProcessStateChanged += this.FtpAgent_ProcessStateChanged;
            this.ftpAgent.Initialize();

            this.LoadFsTreeAsync(this.clientFsTree, this.clientFsTree.CurrentDirectory);

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

        private FsClientTransport CreateClientFsTransport()
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
        private void MoveFiles(List<FsItemVM> srcFsItems, string dstDir)
        {
            foreach (FsItemVM fsItem in srcFsItems)
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

                    this.clientFsTree.Roots.Remove(fsItem);
                }
                catch (Exception ex)
                {
                    logger.Error("MoveFiles异常", ex);
                }
            }
        }

        #endregion

        #region 上传文件

        private void UploadFiles(List<FsItemVM> localFsItems, string serverDir)
        {
            List<TaskTreeNodeVM> taskVms = this.CreateUploadTasks(localFsItems, serverDir);
            List<AgentTask> agentTasks = this.CreateAgentTasks(null, taskVms);
            taskVms.ForEach(v => taskTree.AddRootNode(v));
            this.ftpAgent.EnqueueTask(agentTasks);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localDir"></param>
        /// <param name="serverDir">要上传到哪个目录里</param>
        /// <returns></returns>
        private List<TaskTreeNodeVM> CreateUploadTasksRecursively(string localDir, string serverDir)
        {
            List<TaskTreeNodeVM> tasks = new List<TaskTreeNodeVM>();
            DirectoryInfo directoryInfo = new DirectoryInfo(localDir);
            FileSystemInfo[] fileSystemInfos = directoryInfo.GetFileSystemInfos();

            foreach (FileSystemInfo fsInfo in fileSystemInfos)
            {
                TaskTreeNodeVM taskVm = new TaskTreeNodeVM(taskTree.Context)
                {
                    ID = Guid.NewGuid().ToString(),
                    Name = fsInfo.FullName,
                    SourceFullPath = fsInfo.FullName,
                    TargetFullPath = string.Format("{0}/{1}", serverDir, fsInfo.Name),
                    SourceItemType = fsInfo is DirectoryInfo ? FsItemTypeEnum.Directory : FsItemTypeEnum.File
                };

                if (fsInfo is DirectoryInfo)
                {
                    taskVm.Icon = IconUtils.GetFolderIcon();
                }
                else
                {
                    taskVm.Icon = IconUtils.GetIcon(fsInfo.FullName);
                }

                tasks.Add(taskVm);

                if (taskVm.SourceItemType == FsItemTypeEnum.Directory)
                {
                    taskVm.OpType = FsOperationTypeEnum.CreateDirectory;
                    List<TaskTreeNodeVM> subTasks = this.CreateUploadTasksRecursively(fsInfo.FullName, taskVm.TargetFullPath);
                    subTasks.ForEach(v => taskVm.Add(v));
                }
                else
                {
                    taskVm.OpType = FsOperationTypeEnum.UploadFile;
                }
            }

            return tasks;
        }

        private List<TaskTreeNodeVM> CreateUploadTasks(List<FsItemVM> localFsItems, string serverDir)
        {
            List<TaskTreeNodeVM> tasks = new List<TaskTreeNodeVM>();

            foreach (FsItemVM fsItem in localFsItems)
            {
                // 不可以上传“返回上级目录”节点
                if (fsItem.Type == FsItemTypeEnum.ParentDirectory)
                {
                    continue;
                }

                TaskTreeNodeVM taskVm = new TaskTreeNodeVM(taskTree.Context)
                {
                    ID = Guid.NewGuid().ToString(),
                    Name = fsItem.FullPath,
                    SourceFullPath = fsItem.FullPath,
                    TargetFullPath = string.Format("{0}/{1}", serverDir, fsItem.Name),
                    SourceItemType = fsItem.Type,
                    Icon = fsItem.Icon
                };

                tasks.Add(taskVm);

                if (fsItem.Type == FsItemTypeEnum.Directory)
                {
                    taskVm.OpType = FsOperationTypeEnum.CreateDirectory;
                    List<TaskTreeNodeVM> subTasks = this.CreateUploadTasksRecursively(fsItem.FullPath, taskVm.TargetFullPath);
                    subTasks.ForEach(v => taskVm.Add(v));
                }
                else
                {
                    taskVm.OpType = FsOperationTypeEnum.UploadFile;
                }
            }

            return tasks;
        }

        #endregion

        private List<AgentTask> CreateAgentTasks(AgentTask parentTask, IEnumerable<TaskTreeNodeVM> tasks)
        {
            List<AgentTask> agentTasks = new List<AgentTask>();

            foreach (TaskTreeNodeVM taskVm in tasks)
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
            switch (taskVm.OpType)
            {
                case FsOperationTypeEnum.UploadFile:
                    {
                        return new UploadFileTask()
                        {
                            Id = taskVm.ID.ToString(),
                            SourceFilePath = taskVm.SourceFullPath,
                            TargetFilePath = taskVm.TargetFullPath,
                            UserData = taskVm
                        };
                    }

                case FsOperationTypeEnum.CreateDirectory:
                    {
                        return new CreateDirectoryTask()
                        {
                            Id = taskVm.ID.ToString(),
                            DirectoryPath = taskVm.TargetFullPath,
                            UserData = taskVm
                        };
                    }

                case FsOperationTypeEnum.DeleteDirectory:
                    {
                        return new DeleteDirectoryTask()
                        {
                            Id = taskVm.ID.ToString(),
                            DirectoryPath = taskVm.TargetFullPath,
                            UserData = taskVm
                        };
                    }

                case FsOperationTypeEnum.DeleteFile:
                    {
                        return new DeleteFileTask()
                        {
                            Id = taskVm.ID.ToString(),
                            FilePath = taskVm.SourceFullPath,
                            UserData = taskVm
                        };
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region 公开接口

        public void LoadFsTreeAsync(FsTreeVM fsTree, string directory)
        {
            Task.Factory.StartNew(() =>
            {
                bool clientFs = false; // 加载的是不是本地文件列表
                FsClientTransport transport = null;
                List<FsItemInfo> fsItems = null;

                try
                {
                    if (fsTree == this.serverFsTree)
                    {
                        transport = this.serverFsTransport;
                    }
                    else
                    {
                        transport = this.clientFsTransport;
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

                List<FsItemVM> fsItemVms = new List<FsItemVM>();

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
                        FsItemVM fsItemVM = new FsItemVM(fsTree.Context);
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
                    FsItemVM fsItemVm = new FsItemVM(fsTree.Context);
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

                        foreach (FsItemVM fsItemVm in fsItemVms)
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

        /// <summary>
        /// 传输文件
        /// </summary>
        /// <param name="srcFsItems">要传输的原始文件列表</param>
        /// <param name="dstFsItem">要传输到的目标文件夹，如果为空，则表示当前目录</param>
        public void TransferFile(FsTreeVM srcFsTree, FsTreeVM dstFsTree, List<FsItemVM> srcFsItems, string dstDir)
        {
            FtpRoleEnum srcTreeType = srcFsTree.Context.Type;
            FtpRoleEnum dstTreeType = dstFsTree.Context.Type;

            if (srcTreeType == FtpRoleEnum.Client)
            {
                if (dstTreeType == FtpRoleEnum.Client)
                {
                    // 客户端 - 客户端
                    this.MoveFiles(srcFsItems, dstDir);
                }
                else
                {
                    // 客户端 - 服务器
                    this.UploadFiles(srcFsItems, dstDir);
                }
            }
            else
            {
                if (dstTreeType == FtpRoleEnum.Client)
                {
                    // 服务器 - 客户端
                }
                else
                {
                    // 服务器 - 服务器
                }
            }
        }

        #endregion

        #region 事件处理器

        private void Transport_StatusChanged(object arg1, SessionStatusEnum status)
        {
            base.Status = status;

            switch (status)
            {
                case SessionStatusEnum.Connected:
                    {
                        logger.InfoFormat("Fs客户端已连接, 开始加载服务器文件列表");
                        this.LoadFsTreeAsync(this.serverFsTree, this.serverFsTree.CurrentDirectory);
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

        private void FtpOpenClientItem()
        {
            FsItemVM selectedItem = this.clientFsTree.SelectedItem as FsItemVM;
            if (selectedItem == null)
            {
                return;
            }

            if (selectedItem.Type == FsItemTypeEnum.Directory)
            {
                this.LoadFsTreeAsync(this.clientFsTree, selectedItem.FullPath);
            }
            else if (selectedItem.Type == FsItemTypeEnum.File)
            {
                int rc = Shell32.ShellExecute(IntPtr.Zero, "open", selectedItem.FullPath, null, null, Shell32.SW_SHOW);
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
                                Shell32.OpenAs_RunDLL(IntPtr.Zero, IntPtr.Zero, selectedItem.FullPath, 0);
                                break;
                            }

                        default:
                            {
                                // 打开失败
                                MTMessageBox.Info("打开失败, 错误码:{0}", rc);
                                logger.ErrorFormat("打开文件失败, {0}, 错误码:{1}", selectedItem.FullPath, rc);
                                break;
                            }
                    }
                }
            }
            else
            {
                logger.ErrorFormat("FtpClientOpenItem, 未处理的ItemType, {0}", selectedItem.Type);
            }
        }

        private void FtpDeleteClientItem()
        {
            FsItemVM selectedItem = this.clientFsTree.SelectedItem as FsItemVM;
            if (selectedItem == null)
            {
                return;
            }

            if (!MTMessageBox.Confirm("是否确认删除{0}?", selectedItem.Name))
            {
                return;
            }

            FsOperationTypeEnum opType = FsOperationTypeEnum.DeleteFile;

            if (selectedItem.Type == FsItemTypeEnum.Directory)
            {
                opType = FsOperationTypeEnum.DeleteDirectory;
            }



            //TaskTreeNodeVM taskNode = new TaskTreeNodeVM(this.taskTree.Context)
            //{
            //    ID = Guid.NewGuid().ToString(),
            //    SourceFullPath = selectedItem.FullPath,
            //    State = ProcessStates.Queued,
            //    Name = selectedItem.Name,
            //    OpType = opType,
            //    SourceItemType = selectedItem.Type,
            //};
            //this.taskTree.AddRootNode(taskNode);
            //AgentTask agentTask = this.CreateAgentTask(taskNode);
            //this.ftpAgent.EnqueueTask(agentTask);
        }

        #endregion

        #region 默认右键菜单处理器

        private static void OnFtpOpenClientItem(CommandArgs e) 
        {
            (e.ActiveTab as FtpSessionVM).FtpOpenClientItem();
        }

        private static void OnFtpDeleteClientItem(CommandArgs e)
        {
            (e.ActiveTab as FtpSessionVM).FtpDeleteClientItem();
        }

        #endregion
    }
}