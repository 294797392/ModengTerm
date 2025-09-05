using ModengTerm.Base;
using ModengTerm.FileTrans.Clients;
using ModengTerm.FileTrans.Clients.Channels;
using ModengTerm.FileTrans.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// </summary>
        public event Action<FileAgent, string, double> ProgressChanged;

        #endregion

        #region 实例变量

        private FsClientTransport clientTransport;

        // 用来下载和上传的后台线程数量
        // 每个线程负责处理一个文件的上传和下载
        private List<Thread> threadList;
        private ManualResetEvent taskEvent;
        private List<FileTask> taskList;

        private Queue<FsUploadChannel> uploadChannelQueue;

        #endregion

        #region 属性

        /// <summary>
        /// 客户端传输通道
        /// </summary>
        public FsClientTransport Transport
        {
            get { return this.clientTransport; }
            set
            {
                if (this.clientTransport != value)
                {
                    this.clientTransport = value;
                }
            }
        }

        /// <summary>
        /// 上传线程数量
        /// </summary>
        public int Threads { get; set; }

        #endregion

        #region 构造方法

        public FileAgent()
        {
        }

        #endregion

        #region 公开接口

        public void Initialize()
        {
            this.uploadChannelQueue = new Queue<FsUploadChannel>();
        }

        /// <summary>
        /// 取消所有正在上传的任务
        /// </summary>
        public void Release()
        {

        }

        public void EnqueueTask(List<FileTask> tasks)
        {
            if (this.threadList == null)
            {
                this.taskEvent = new ManualResetEvent(false);
                this.threadList = new List<Thread>();
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

        private void NotifyProgressChanged(string taskId, double progress)
        {
            this.ProgressChanged?.Invoke(this, taskId, progress);
        }

        private FsUploadChannel GetFreeUploadChannel() 
        {
            FsUploadChannel channel = null;

            lock (this.uploadChannelQueue)
            {
                if (this.uploadChannelQueue.Count > 0) 
                {
                    channel = this.uploadChannelQueue.Dequeue();
                }
            }

            if (channel == null) 
            {
                channel = this.clientTransport.CreateUploadChannel();
            }

            return channel;
        }

        private void ReuseUploadChannel(FsUploadChannel channel) 
        {
            lock (this.uploadChannelQueue)
            {
                this.uploadChannelQueue.Enqueue(channel);
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

                switch (task.Type)
                {
                    case FileTaskTypeEnum.CreateDirectory:
                        {
                            if (!this.clientTransport.CreateDirectory(task.TargetFilePath))
                            {
                                this.NotifyProgressChanged(task.Id, ResponseCode.FAILED);
                            }
                            else
                            {
                                this.NotifyProgressChanged(task.Id, 100);
                            }
                            break;
                        }

                    case FileTaskTypeEnum.UploadFile:
                        {
                            FsUploadChannel uploadChannel = this.GetFreeUploadChannel();
                            break;
                        }

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        #endregion
    }
}