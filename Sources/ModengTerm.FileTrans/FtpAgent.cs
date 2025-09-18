using ModengTerm.Base;
using ModengTerm.Base.Enumerations;
using ModengTerm.FileTrans.Clients;
using ModengTerm.Ftp.Enumerations;
using Renci.SshNet.Common;

namespace ModengTerm.FileTrans
{
    /// <summary>
    /// 负责把文件上传到服务器，或者从服务器下载文件
    /// 多线程上传注意事项：
    /// 不要使用多个Sftp实例同时写入同一个文件的不同部分，因为服务器在多线程写入文件的时候可能会有出问题
    /// 如果服务器使用的是同一个FileStream，那么多线程同时写入同一个FileStream的时候，FileStream的当前位置指针会发成访问冲突
    /// </summary>
    public class FtpAgent
    {
        #region 公开事件

        /// <summary>
        /// 上传或者下载进度发生改变的时候触发
        /// string：taskId
        /// double：progress
        /// string：serverMessage
        /// string：speed
        /// ProcessStates：ProcessStates
        /// object：UserData
        /// </summary>
        public event Action<FtpAgent, string, double, string, string, ProcessStates, object> ProcessStateChanged;

        #endregion

        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("FtpAgent");

        #endregion

        #region 实例变量

        // 用来下载和上传的后台线程数量
        // 每个线程负责处理一个文件的上传和下载
        private List<Thread> threadList;
        private ManualResetEvent taskEvent;
        private List<AgentTask> taskList;

        private Queue<FsClientBase> clientQueue;
        private bool initOnce;

        private bool isRunning;

        #endregion

        #region 属性

        /// <summary>
        /// 客户端传输通道配置项
        /// </summary>
        public FsClientOptions ClientOptions { get; set; }

        /// <summary>
        /// 上传线程数量
        /// </summary>
        public int Threads { get; set; }

        /// <summary>
        /// 一次上传多大的数据
        /// 单位是字节
        /// </summary>
        public int UploadBufferSize { get; set; }

        /// <summary>
        /// 一次下载多大的数据
        /// 单位是字节
        /// </summary>
        public int DownloadBufferSize { get; set; }

        #endregion

        #region 构造方法

        public FtpAgent()
        {
        }

        #endregion

        #region 公开接口

        public void Initialize()
        {
            this.isRunning = true;
            this.taskList = new List<AgentTask>();
            this.clientQueue = new Queue<FsClientBase>();
            this.taskEvent = new ManualResetEvent(false);
            this.threadList = new List<Thread>();
        }

        /// <summary>
        /// 取消所有正在上传的任务
        /// </summary>
        public void Release()
        {
            this.isRunning = false;
            lock (this.taskList)
            {
                this.taskList.Clear();
                this.taskEvent.Set();
                this.taskEvent.Dispose();
            }
            this.threadList.ForEach(v => v.Join());
        }

        public void EnqueueTask(AgentTask task)
        {
            List<AgentTask> tasks = new List<AgentTask>();
            tasks.Add(task);

            this.EnqueueTask(tasks);
        }

        public void EnqueueTask(List<AgentTask> tasks)
        {
            if (!this.initOnce)
            {
                this.initOnce = true;
                for (int i = 0; i < this.Threads; i++)
                {
                    Thread thread = new Thread(this.WorkerThreadProc);
                    thread.IsBackground = true;
                    thread.Start();
                }
            }

            lock (this.taskList)
            {
                this.taskList.AddRange(tasks);
                this.taskEvent.Set();
            }
        }

        #endregion

        #region 实例方法

        /// <summary>
        /// 从任务队列中选择一个要上传的任务
        /// </summary>
        /// <returns></returns>
        private AgentTask DequeueTask()
        {
            AgentTask task = null;

            lock (this.taskList)
            {
                if (this.taskList.Count == 0)
                {
                    this.taskEvent.Reset();
                    return null;
                }

                task = this.taskList[0];

                this.taskList.RemoveAt(0);
            }

            return task;
        }

        private FsClientBase GetFreeFsClient()
        {
            FsClientBase client = null;

            lock (this.clientQueue)
            {
                if (this.clientQueue.Count > 0)
                {
                    client = this.clientQueue.Dequeue();
                }
            }

            if (client == null)
            {
                client = FsClientFactory.Create(this.ClientOptions);
                client.Options = this.ClientOptions;
                if (client.Open() != ResponseCode.SUCCESS)
                {
                    return null;
                }
            }

            return client;
        }

        private void ReuseFsClient(FsClientBase client)
        {
            lock (this.clientQueue)
            {
                this.clientQueue.Enqueue(client);
            }
        }

        private string CalculateSpeed(DateTime startTime, DateTime endTime, int bytesTransfered) 
        {
            TimeSpan ts = endTime - startTime;
            double bytesSpeed = bytesTransfered / ts.TotalSeconds;
            double toValue;
            SizeUnitEnum toUnit;
            VTBaseUtils.AutoFitSize(bytesSpeed, SizeUnitEnum.bytes, out toValue, out toUnit);
            return string.Format("{0} {1}/s", toValue, toUnit);
        }

        private bool UploadFile(AgentTask task, FsClientBase fsClient)
        {
            UploadFileTask uploadTask = task as UploadFileTask;

            FileStream stream = null;
            try
            {
                stream = new FileStream(uploadTask.SourceFilePath, FileMode.Open, FileAccess.Read);
            }
            catch (FileNotFoundException ex)
            {
                this.HandleFailureTask(uploadTask, "要上传的文件不存在");
                logger.ErrorFormat("打开要上传的文件异常, 文件不存在, {0}", uploadTask.SourceFilePath);
                return false;
            }
            catch (Exception ex)
            {
                this.HandleFailureTask(uploadTask, ex.Message);
                logger.ErrorFormat("打开要上传的文件异常, {0}, {1}", ex, uploadTask.SourceFilePath);
                return false;
            }

            bool success = true;

            fsClient.BeginUpload(uploadTask.TargetFilePath, this.UploadBufferSize);

            byte[] buffer = new byte[this.UploadBufferSize];

            this.NotifyProgressChanged(uploadTask, 0, string.Empty, string.Empty, ProcessStates.Starting);
            DateTime startTime = DateTime.Now;

            while (stream.Position != stream.Length)
            {
                try
                {
                    int len = stream.Read(buffer, 0, buffer.Length);
                    fsClient.Upload(buffer, 0, len);
                    string speed = this.CalculateSpeed(startTime, DateTime.Now, len);
                    double percent = Math.Round(((double)stream.Position / stream.Length) * 100, 2);
                    this.NotifyProgressChanged(uploadTask, percent, string.Empty, speed, ProcessStates.ProgressChanged);
                    //logger.InfoFormat("上传百分比:{0}", percent);
                }
                catch (SshException ex)
                {
                    success = false;
                    this.HandleFailureTask(uploadTask, ex.Message);
                    logger.Error("上传数据段异常", ex);
                    break;
                }
                catch (Exception ex)
                {
                    success = false;
                    this.HandleFailureTask(uploadTask, ex.Message);
                    logger.Error("上传数据段异常", ex);
                    break;
                }
            }

            logger.InfoFormat("上传完成");

            try
            {
                stream.Close();
            }
            finally
            { }

            fsClient.EndUpload();

            // 如果出现异常导致传输失败，那么在catch里触发事件
            if (success)
            {
                this.HandleCompletedTask(uploadTask);
            }

            return success;
        }

        private bool DeleteFile(AgentTask task, FsClientBase fsClient)
        {
            DeleteFileTask dfTask = task as DeleteFileTask;

            try
            {
                fsClient.DeleteFile(dfTask.FilePath);
                this.HandleCompletedTask(dfTask);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error("删除文件异常", ex);
                this.HandleFailureTask(dfTask, ex.Message);
                return false;
            }
        }

        private bool CreateDirectory(AgentTask task, FsClientBase fsClient)
        {
            CreateDirectoryTask cdTask = task as CreateDirectoryTask;

            try
            {
                fsClient.CreateDirectory(cdTask.DirectoryPath);
                this.HandleCompletedTask(task);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error("创建目录异常", ex);
                this.HandleFailureTask(task, ex.Message);
                return false;
            }
        }

        private bool DeleteDirectory(AgentTask task, FsClientBase fsClient)
        {
            DeleteDirectoryTask ddTask = task as DeleteDirectoryTask;

            try
            {
                fsClient.DeleteDirectory(ddTask.DirectoryPath);
                this.HandleCompletedTask(task);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error("删除目录异常", ex);
                this.HandleFailureTask(task, ex.Message);
                return false;
            }
        }

        private void NotifyProgressChanged(AgentTask task, double progress, string serverMessage, string speed, ProcessStates processStates)
        {
            this.ProcessStateChanged?.Invoke(this, task.Id, progress, serverMessage, speed, processStates, task.UserData);
        }

        private void HandleParentTaskState(AgentTask task) 
        {
            // 此时有父节点，触发父节点的进度事件
            AgentTask parentTask = task.Parent;

            while (parentTask != null)
            {
                int success = parentTask.SubTasks.Count(v => v.State == AgentTaskStates.Success);
                int failure = parentTask.SubTasks.Count(v => v.State == AgentTaskStates.Failure);
                int total = parentTask.SubTasks.Count;

                if (success + failure == total)
                {
                    // 只有等所有子任务全部传输完成，再通知状态改变
                    if (failure > 0)
                    {
                        lock (parentTask)
                        {
                            parentTask.State = AgentTaskStates.Failure;
                            this.ProcessStateChanged?.Invoke(this, parentTask.Id, 0, "子目录或文件上传失败", string.Empty, ProcessStates.Failure, parentTask.UserData);
                        }
                    }
                    else
                    {
                        lock (parentTask)
                        {
                            parentTask.State = AgentTaskStates.Success;
                            parentTask.Progress = 100;
                            this.ProcessStateChanged?.Invoke(this, parentTask.Id, 100, string.Empty, string.Empty, ProcessStates.Success, parentTask.UserData);
                        }
                    }
                }
                else
                {
                    // 如果有的任务还在传输，那么不通知父任务状态改变，只通知传输进度
                    lock (parentTask)
                    {
                        if (parentTask.State == AgentTaskStates.Processing)
                        {
                            double percent = Math.Round(((double)success / total) * 100, 2);
                            parentTask.Progress = percent;

                            double percent1 = Math.Round(parentTask.SubTasks.Sum(v => v.Progress) / (parentTask.SubTasks.Count * 100) * 100, 2);
                            this.ProcessStateChanged?.Invoke(this, parentTask.Id, percent1, string.Empty, string.Empty, ProcessStates.ProgressChanged, parentTask.UserData);
                        }
                    }
                }

                parentTask = parentTask.Parent;
            }
        }

        /// <summary>
        /// 当上传失败之后调用
        /// </summary>
        /// <param name="task">上传失败的任务</param>
        /// <param name="message">失败消息</param>
        private void HandleFailureTask(AgentTask task, string message)
        {
            task.State = AgentTaskStates.Failure;

            if (task.SubTasks.Count == 0)
            {
                this.ProcessStateChanged?.Invoke(this, task.Id, 0, message, string.Empty, ProcessStates.Failure, task.UserData);
            }

            // 如果该节点有父节点，那么需要通知所有父节点失败
            AgentTask parentTask = task.Parent;

            while (parentTask != null)
            {
                int success = parentTask.SubTasks.Count(v => v.State == AgentTaskStates.Success);
                int failure = parentTask.SubTasks.Count(v => v.State == AgentTaskStates.Failure);
                int total = parentTask.SubTasks.Count;

                if (success + failure == total)
                {
                    // 只有等所有子任务全部传输完成，再通知状态改变
                    lock (parentTask)
                    {
                        if (parentTask.State == AgentTaskStates.Failure)
                        {
                            this.ProcessStateChanged?.Invoke(this, parentTask.Id, 0, message, string.Empty, ProcessStates.Failure, parentTask.UserData);
                        }
                    }
                }
                else
                {
                    // 如果有的任务还在传输，那么不通知父任务状态改变，只通知传输进度
                    // 因为传输失败，所以此时传输进度不变，所以不需要通知
                }

                parentTask = parentTask.Parent;
            }
        }

        /// <summary>
        /// 当上传成功之后调用
        /// </summary>
        /// <param name="task">上传成功的任务</param>
        private void HandleCompletedTask(AgentTask task)
        {
            switch (task.Type)
            {
                case FsOperationTypeEnum.UploadFile:
                    {
                        task.State = AgentTaskStates.Success;
                        task.Progress = 100;
                        this.ProcessStateChanged?.Invoke(this, task.Id, 100, string.Empty, string.Empty, ProcessStates.Success, task.UserData);
                        this.HandleParentTaskState(task);
                        break;
                    }

                case FsOperationTypeEnum.CreateDirectory:
                    {
                        if (task.SubTasks.Count == 0)
                        {
                            task.State = AgentTaskStates.Success;
                            task.Progress = 100;
                            this.ProcessStateChanged?.Invoke(this, task.Id, 100, string.Empty, string.Empty, ProcessStates.Success, task.UserData);
                            this.HandleParentTaskState(task);
                        }
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

        }

        #endregion

        #region 事件处理器

        private void WorkerThreadProc()
        {
            FsClientBase fsClient = null;

            while (this.isRunning)
            {
                this.taskEvent.WaitOne();

                if (!this.isRunning) 
                {
                    break;
                }

                AgentTask task = this.DequeueTask();
                if (task == null)
                {
                    continue;
                }

                if (fsClient == null)
                {
                    fsClient = this.GetFreeFsClient();

                    if (fsClient == null)
                    {
                        // 这个线程连接服务器失败, 把任务重新入队, 等待下次某个线程继续上传
                        lock (this.taskList)
                        {
                            this.taskList.Insert(0, task);
                            this.taskEvent.Set();
                        }
                        this.HandleFailureTask(task, "连接服务器失败");
                        continue;
                    }
                }

                bool success = false;

                switch (task.Type)
                {
                    case FsOperationTypeEnum.CreateDirectory:
                        {
                            success = this.CreateDirectory(task, fsClient);
                            break;
                        }

                    case FsOperationTypeEnum.DeleteDirectory:
                        {
                            success = this.DeleteDirectory(task, fsClient);
                            break;
                        }

                    case FsOperationTypeEnum.UploadFile:
                        {
                            success = this.UploadFile(task, fsClient);
                            break;
                        }

                    case FsOperationTypeEnum.DeleteFile:
                        {
                            success = this.DeleteFile(task, fsClient);
                            break;
                        }

                    default:
                        throw new NotImplementedException();
                }

                // 子任务必须在父任务成功运行结束之后再运行
                if (success)
                {
                    // 此时父任务成功运行结束，让子任务入队
                    if (task.SubTasks.Count > 0)
                    {
                        lock (this.taskList)
                        {
                            this.taskList.AddRange(task.SubTasks);
                            this.taskEvent.Set();
                        }
                    }
                }
            }

            if (fsClient != null) 
            {
                fsClient.Close();
            }
        }

        #endregion
    }
}