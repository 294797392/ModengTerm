using log4net.Repository.Hierarchy;
using ModengTerm.Base;
using ModengTerm.Document;
using ModengTerm.FileTrans.Clients;
using ModengTerm.FileTrans.Enumerations;
using Renci.SshNet.Common;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ModengTerm.FileTrans
{
    /// <summary>
    /// 负责把文件上传到服务器，或者从服务器下载文件
    /// </summary>
    public class FileAgent
    {
        #region 公开事件

        /// <summary>
        /// 上传或者下载进度发生改变的时候触发
        /// string：taskId
        /// double：progress
        /// string：serverMessage
        /// int：bytesTransferd，本次传输字节数
        /// ProcessStates：ProcessStates
        /// </summary>
        public event Action<FileAgent, string, double, string, int, ProcessStates> ProgressChanged;

        #endregion

        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("FileAgent");

        #endregion

        #region 实例变量

        private FsClientTransport clientTransport;

        // 用来下载和上传的后台线程数量
        // 每个线程负责处理一个文件的上传和下载
        private List<Thread> threadList;
        private ManualResetEvent taskEvent;
        private List<FileTask> taskList;

        private Queue<FsClientBase> clientQueue;
        private bool initOnce;

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

        public FileAgent()
        {
        }

        #endregion

        #region 公开接口

        public void Initialize()
        {
            this.taskList = new List<FileTask>();
            this.clientQueue = new Queue<FsClientBase>();
            this.taskEvent = new ManualResetEvent(false);
            this.threadList = new List<Thread>();
        }

        /// <summary>
        /// 取消所有正在上传的任务
        /// </summary>
        public void Release()
        {
            this.clientTransport.Close();
        }

        public void EnqueueTask(List<FileTask> tasks)
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
        private FileTask SelectTask()
        {
            FileTask task = null;

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

        private void NotifyProgressChanged(string taskId, double progress, string serverMessage, int bytesTransfer, ProcessStates processStates)
        {
            this.ProgressChanged?.Invoke(this, taskId, progress, serverMessage, bytesTransfer, processStates);
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

        private bool UploadFile(FileTask task, FsClientBase fsClient)
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(task.SourceFilePath, FileMode.Open, FileAccess.Read);
            }
            catch (FileNotFoundException ex)
            {
                this.NotifyProgressChanged(task.SourceFilePath, 0, "要上传的文件不存在", 0, ProcessStates.Failure);
                logger.ErrorFormat("打开要上传的文件异常, 文件不存在, {0}", task.SourceFilePath);
                return false;
            }
            catch (Exception ex)
            {
                this.NotifyProgressChanged(task.Id, 0, ex.Message, 0, ProcessStates.Failure);
                logger.ErrorFormat("打开要上传的文件异常, {0}, {1}", ex, task.SourceFilePath);
                return false;
            }

            bool success = true;

            fsClient.BeginUpload(task.TargetFilePath, this.UploadBufferSize);

            byte[] buffer = new byte[this.UploadBufferSize];

            this.NotifyProgressChanged(task.Id, 0, string.Empty, 0, ProcessStates.StartTransfer);

            while (stream.Position != stream.Length)
            {
                try
                {
                    int len = stream.Read(buffer, 0, buffer.Length);
                    fsClient.Upload(buffer, 0, len);

                    double percent = Math.Round(((double)stream.Position / stream.Length) * 100, 2);
                    this.NotifyProgressChanged(task.Id, percent, string.Empty, len, ProcessStates.BytesTransfered);
                    //logger.InfoFormat("上传百分比:{0}", percent);
                }
                catch (SshException ex)
                {
                    success = false;
                    this.NotifyProgressChanged(task.Id, 0, ex.Message, 0, ProcessStates.Failure);
                    logger.Error("上传数据段异常", ex);
                    break;
                }
                catch (Exception ex)
                {
                    success = false;
                    this.NotifyProgressChanged(task.Id, 0, ex.Message, 0, ProcessStates.Failure);
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
                this.NotifyProgressChanged(task.Id, 100, string.Empty, 0, ProcessStates.Completed);
            }

            return success;
        }

        private bool CreateDirectory(FileTask task, FsClientBase fsClient)
        {
            try
            {
                fsClient.CreateDirectory(task.TargetFilePath);
                this.NotifyProgressChanged(task.Id, 100, string.Empty, 0, ProcessStates.Completed);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error("创建目录异常", ex);
                this.NotifyProgressChanged(task.Id, 0, ex.Message, 0, ProcessStates.Failure);
                return false;
            }
        }

        #endregion

        #region 事件处理器

        private void WorkerThreadProc()
        {
            while (true)
            {
                this.taskEvent.WaitOne();

                FileTask task = this.SelectTask();
                if (task == null)
                {
                    continue;
                }

                FsClientBase fsClient = this.GetFreeFsClient();
                if (fsClient == null)
                {
                    this.NotifyProgressChanged(task.Id, 0, "连接服务器失败", 0, ProcessStates.Failure);
                    continue;
                }

                switch (task.Type)
                {
                    case FileTaskTypeEnum.CreateDirectory:
                        {
                            this.CreateDirectory(task, fsClient);
                            break;
                        }

                    case FileTaskTypeEnum.UploadFile:
                        {
                            this.UploadFile(task, fsClient);
                            break;
                        }

                    default:
                        throw new NotImplementedException();
                }

                this.ReuseFsClient(fsClient);
            }
        }

        #endregion
    }
}