using DotNEToolkit;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XTerminal.Base.SftpTrasmit
{
    /// <summary>
    /// SFTP传输代理
    /// </summary>
    public class SftpTransmitter
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("SftpTransmitter");

        #endregion

        #region 公开事件

        /// <summary>
        /// 当Sftp传输状态改变的时候触发
        /// </summary>
        public event Action<SftpTransmit> StatusChanged;

        #endregion

        #region 实例变量

        /// <summary>
        /// 传输队列
        /// </summary>
        private BufferQueue<SftpTransmit> transmitQueue;
        private List<Thread> transmitThreads;
        private bool isRunning;

        #endregion

        #region 属性

        /// <summary>
        /// 线程数量
        /// </summary>
        public int Threads { get; set; }

        /// <summary>
        /// 已连接的Sftp客户端
        /// </summary>
        public SftpClient Client { get; set; }

        #endregion

        #region 公开接口

        public int Initialize()
        {
            this.transmitQueue = new BufferQueue<SftpTransmit>(8192);
            this.transmitThreads = new List<Thread>();
            for (int i = 0; i < this.Threads; i++)
            {
                Thread thread = new Thread(this.TransmitThreadProc);
                thread.IsBackground = true;
                thread.Start();
                this.transmitThreads.Add(thread);
            }

            return ResponseCode.SUCCESS;
        }

        public void Release()
        {
            this.isRunning = false;
            this.transmitQueue.Dispose();
            this.transmitThreads.ForEach(v => v.Join());
        }

        /// <summary>
        /// 执行一个传输任务
        /// </summary>
        /// <param name="transmit"></param>
        public void Transmit(SftpTransmit transmit)
        {
            this.transmitQueue.Enqueue(transmit);
            transmit.Status = TransmitStatusEnum.Queued;
            this.NotifyStatus(transmit);
        }

        #endregion

        #region 实例方法

        private void NotifyStatus(SftpTransmit transmit)
        {
            if (this.StatusChanged != null)
            {
                this.StatusChanged(transmit);
            }
        }

        #endregion

        #region 事件处理器

        private void TransmitThreadProc(object state)
        {
            logger.InfoFormat("Transmit线程已启动...");

            while (this.isRunning)
            {
                SftpTransmit transmit = this.transmitQueue.Dequeue();

                transmit.Status = TransmitStatusEnum.Transmitting;
                this.NotifyStatus(transmit);

                switch (transmit.Type)
                {
                    case SftpTransmitTypeEnum.Download:
                        {
                            try
                            {
                                using (FileStream fs = new FileStream(transmit.LocalPath, FileMode.Create, FileAccess.ReadWrite))
                                {
                                    this.Client.DownloadFile(transmit.RemotePath, fs);
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.Error("下载文件异常", ex);
                                transmit.Status = TransmitStatusEnum.Error;
                                this.NotifyStatus(transmit);
                                continue;
                            }
                            break;
                        }

                    case SftpTransmitTypeEnum.Upload:
                        {
                            try
                            {
                                using (FileStream fs = new FileStream(transmit.LocalPath, FileMode.Open, FileAccess.ReadWrite))
                                {
                                    this.Client.UploadFile(fs, transmit.RemotePath);
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.Error("上传文件异常", ex);
                                transmit.Status = TransmitStatusEnum.Error;
                                this.NotifyStatus(transmit);
                                continue;
                            }
                            break;
                        }

                    default:
                        {
                            throw new NotImplementedException();
                        }
                }

                transmit.Status = TransmitStatusEnum.Success;
                this.NotifyStatus(transmit);
            }
        }

        #endregion
    }
}
