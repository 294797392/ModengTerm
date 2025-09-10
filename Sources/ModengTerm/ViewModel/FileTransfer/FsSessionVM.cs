using log4net.Repository.Hierarchy;
using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.DataModels.Ssh;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.Enumerations.Ssh;
using ModengTerm.FileTrans;
using ModengTerm.FileTrans.Clients;
using ModengTerm.FileTrans.DataModels;
using ModengTerm.FileTrans.Enumerations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel.FileTrans
{
    public class FsSessionVM : OpenedSessionVM, IClientFileTransTab
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("FsSessionVM");

        #endregion

        #region 实例变量

        private FsTreeVM serverFsTree;
        private FsTreeVM clientFsTree;
        private FsClientTransport serverFsTransport;
        private FsClientTransport clientFsTransport;

        private BindableCollection<FileStatusVM> fileStatus;

        private FileAgent fileAgent;

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
        public BindableCollection<FileStatusVM> FileStatus { get { return this.fileStatus; } }

        #endregion

        #region 构造方法

        public FsSessionVM(XTermSession session) :
            base(session)
        {
        }

        #endregion

        #region OpenedSessionVM

        protected override void OnInitialize()
        {
            this.clientFsTree = new FsTreeVM();
            this.clientFsTree.Context.Type = FsTreeTypeEnum.ClientTree;
            this.serverFsTree = new FsTreeVM();
            this.serverFsTree.Context.Type = FsTreeTypeEnum.ServerTree;
            this.fileStatus = new BindableCollection<FileStatusVM>();
        }

        protected override void OnRelease()
        {
        }

        protected override int OnOpen()
        {
            this.serverFsTree.CurrentDirectory = this.session.GetOption<string>(PredefinedOptions.FS_GENERAL_SERVER_INITIAL_DIR);
            this.clientFsTree.CurrentDirectory = this.session.GetOption<string>(PredefinedOptions.FS_GENERAL_CLIENT_INITIAL_DIR);

            FsClientOptions options = this.CreateOptions();
            FsClientTransport transport = new FsClientTransport();
            transport.StatusChanged += this.Transport_StatusChanged;
            transport.OpenAsync(options);

            this.serverFsTransport = transport;
            this.clientFsTransport = this.CreateClientFsTransport();
            this.fileAgent = new FileAgent();
            this.fileAgent.ClientOptions = options;
            this.fileAgent.Threads = this.session.GetOption<int>(PredefinedOptions.FS_TRANS_THREADS);
            this.fileAgent.UploadBufferSize = this.session.GetOption<int>(PredefinedOptions.FS_TRANS_UPLOAD_BUFFER_SIZE) * 1024;
            this.fileAgent.DownloadBufferSize = this.session.GetOption<int>(PredefinedOptions.FS_TRANS_DOWNLOAD_BUFFER_SIZE) * 1024;
            this.fileAgent.ProgressChanged += this.FileAgent_ProgressChanged;
            this.fileAgent.Initialize();

            this.LoadFsTreeAsync(this.clientFsTree, this.clientFsTree.CurrentDirectory);

            return ResponseCode.SUCCESS;
        }

        protected override void OnClose()
        {
            this.fileAgent.ProgressChanged -= this.FileAgent_ProgressChanged;
            this.fileAgent.Release();

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

        private void UploadFiles(List<FsItemVM> localFsItems, string serverDir)
        {
            List<FileStatusVM> fileStatusVM = this.CreateFileStatus(localFsItems, serverDir, FsOperationTypeEnum.Upload);
            List<FileTask> uploadItems = this.CreateUploadTasks(fileStatusVM);

            this.fileStatus.AddRange(fileStatusVM);
            this.fileAgent.EnqueueTask(uploadItems);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localDir"></param>
        /// <param name="serverDir">要上传到哪个目录里</param>
        /// <returns></returns>
        private List<FileStatusVM> CreateFileStatusRecursively(string localDir, string serverDir, FsOperationTypeEnum opType)
        {
            List<FileStatusVM> fileStatus = new List<FileStatusVM>();
            DirectoryInfo directoryInfo = new DirectoryInfo(localDir);
            FileSystemInfo[] fileSystemInfos = directoryInfo.GetFileSystemInfos();

            foreach (FileSystemInfo fsInfo in fileSystemInfos)
            {
                FileStatusVM fstat = new FileStatusVM()
                {
                    ID = Guid.NewGuid().ToString(),
                    Name = fsInfo.FullName,
                    SourceFullPath = fsInfo.FullName,
                    TargetFullPath = string.Format("{0}/{1}", serverDir, fsInfo.Name),
                    Type = fsInfo is DirectoryInfo ? FsItemTypeEnum.Directory : FsItemTypeEnum.File,
                    OpType = opType
                };
                fileStatus.Add(fstat);

                if (fstat.Type == FsItemTypeEnum.Directory)
                {
                    fileStatus.AddRange(this.CreateFileStatusRecursively(fsInfo.FullName, fstat.TargetFullPath, opType));
                }
            }

            return fileStatus;
        }

        private List<FileStatusVM> CreateFileStatus(List<FsItemVM> localFsItems, string serverDir, FsOperationTypeEnum opType)
        {
            List<FileStatusVM> fileStatusVMs = new List<FileStatusVM>();

            foreach (FsItemVM fsItem in localFsItems)
            {
                FileStatusVM fstat = new FileStatusVM()
                {
                    ID = Guid.NewGuid().ToString(),
                    Name = fsItem.FullPath,
                    SourceFullPath = fsItem.FullPath,
                    TargetFullPath = string.Format("{0}/{1}", serverDir, fsItem.Name),
                    Type = fsItem.Type,
                    OpType = opType
                };

                fileStatusVMs.Add(fstat);

                if (fsItem.Type == FsItemTypeEnum.Directory)
                {
                    fileStatusVMs.AddRange(this.CreateFileStatusRecursively(fsItem.FullPath, fstat.TargetFullPath, opType));
                }
            }

            return fileStatusVMs;
        }

        private List<FileTask> CreateUploadTasks(List<FileStatusVM> fileStatusVM)
        {
            List<FileTask> uploadTasks = new List<FileTask>();

            foreach (FileStatusVM fileStatus in fileStatusVM)
            {
                FileTask uploadTask = new FileTask()
                {
                    Id = fileStatus.ID.ToString(),
                    SourceFilePath = fileStatus.SourceFullPath,
                    TargetFilePath = fileStatus.TargetFullPath,
                };

                if (fileStatus.Type == FsItemTypeEnum.Directory)
                {
                    uploadTask.Type = FileTaskTypeEnum.CreateDirectory;
                }
                else
                {
                    uploadTask.Type = FileTaskTypeEnum.UploadFile;
                }

                uploadTasks.Add(uploadTask);
            }

            return uploadTasks;
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
                        fsItemVM.Type = FsItemTypeEnum.Directory;
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
                    fsItemVm.Type = fsItem.Type;
                    fsItemVm.IsHidden = fsItem.IsHidden;
                    fsItemVm.IsVisible = !fsItemVm.IsHidden;

                    if (fsItem.Type == FsItemTypeEnum.Directory)
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
            FsTreeTypeEnum srcTreeType = srcFsTree.Context.Type;
            FsTreeTypeEnum dstTreeType = dstFsTree.Context.Type;

            if (srcTreeType == FsTreeTypeEnum.ClientTree)
            {
                if (dstTreeType == FsTreeTypeEnum.ClientTree)
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
                if (dstTreeType == FsTreeTypeEnum.ClientTree)
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

        private void FileAgent_ProgressChanged(FileAgent fileAgent, string taskId, double progress, string serverMessage, int bytesTransfer, ProcessStates processStates)
        {
            FileStatusVM fileStatus = this.fileStatus.FirstOrDefault(v => v.ID == taskId);
            if (fileStatus == null)
            {
                logger.ErrorFormat("查找文件状态失败, {0}, {1}, {2}", taskId, progress, serverMessage);
                return;
            }

            fileStatus.State = processStates;
            fileStatus.Message = serverMessage;

            switch (processStates)
            {
                case ProcessStates.BytesTransfered:
                    {
                        TimeSpan ts = DateTime.Now - fileStatus.PrevTransferTime;
                        double speed = bytesTransfer / ts.TotalSeconds;
                        double toValue;
                        SizeUnitEnum toUnit;
                        VTBaseUtils.AutoFitSize(speed, SizeUnitEnum.bytes, out toValue, out toUnit);
                        fileStatus.Speed = string.Format("{0} {1}/s", toValue, toUnit);
                        fileStatus.Progress = progress;
                        fileStatus.PrevTransferTime = DateTime.Now;
                        break;
                    }

                case ProcessStates.Failure:
                    {
                        break;
                    }

                case ProcessStates.Completed:
                    {
                        fileStatus.Progress = progress;
                        break;
                    }

                case ProcessStates.StartTransfer:
                    {
                        fileStatus.PrevTransferTime = DateTime.Now;
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion
    }
}