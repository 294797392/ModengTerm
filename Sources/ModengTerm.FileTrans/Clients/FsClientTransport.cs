using ModengTerm.Base.Enumerations;
using ModengTerm.FileTrans.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.FileTrans.Clients
{
    public class FsClientTransport
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("ClientTransport");

        #region 公开事件

        /// <summary>
        /// 当Session状态发生改变的时候触发
        /// </summary>
        public event Action<object, SessionStatusEnum> StatusChanged;

        /// <summary>
        /// 当上传状态改变的时候触发
        /// </summary>
        public event Action<AbstractTask, double> UploadProgress;

        #endregion

        #region 实例变量

        private FsClientBase client;
        private SessionStatusEnum status;

        #endregion

        #region 公开接口

        public void OpenAsync(FsClientOptions options)
        {
            this.client = FsClientFactory.Create(options);
            this.client.Options = options;

            this.NotifyStatusChanged(SessionStatusEnum.Connecting);

            Task.Factory.StartNew(() =>
            {
                try
                {
                    this.client.Open();
                    this.client.ChangeDirectory(options.InitialDirectory);

                    this.NotifyStatusChanged(SessionStatusEnum.Connected);
                }
                catch (Exception ex)
                {
                    logger.Error("FsClient连接异常", ex);
                    this.NotifyStatusChanged(SessionStatusEnum.ConnectError);
                }
            });
        }

        public void Close()
        {
            FsClientBase client = this.client;

            if (client == null)
            {
                return;
            }

            client.Close();

            this.client = null;

            this.NotifyStatusChanged(SessionStatusEnum.Disconnected);
        }

        public List<FsItemInfo> ListFiles(string directory)
        {
            if (!this.CheckStatus())
            {
                return null;
            }

            try
            {
                return this.client.ListFiles(directory);
            }
            catch (Exception ex)
            {
                logger.Error("ListFiles异常", ex);
                return null;
            }
        }

        public bool ChangeDirectory(string directory)
        {
            if (!this.CheckStatus())
            {
                return false;
            }

            try
            {
                this.client.ChangeDirectory(directory);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error("ChangeDirectory异常", ex);
                return false;
            }
        }

        /// <summary>
        /// 在服务器上创建多个文件
        /// </summary>
        /// <param name="directories"></param>
        /// <returns></returns>
        public bool CreateDirectory(string dir)
        {
            if (!this.CheckStatus())
            {
                return false;
            }

            try
            {
                this.client.CreateDirectory(dir);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error("CreateDirectory异常", ex);
                return false;
            }
        }

        public bool DeleteFile(string filePath) 
        {
            if (!this.CheckStatus()) 
            {
                return false;
            }

            try 
            {
                this.client.DeleteFile(filePath);
                return true;
            }
            catch (Exception ex) 
            {
                logger.Error("删除文件异常", ex);
                return false;
            }
        }

        public bool DeleteDirectory(string directoryPath) 
        {
            if (!this.CheckStatus()) 
            {
                return false;
            }

            try
            {
                this.client.DeleteDirectory(directoryPath);
                return true;
            }
            catch (Exception ex) 
            {
                logger.Error("删除目录异常", ex);
                return false;
            }
        }


        #endregion

        #region 实例方法

        private bool CheckStatus()
        {
            FsClientBase client = this.client;

            if (client == null)
            {
                return false;
            }

            if (this.status != SessionStatusEnum.Connected)
            {
                return false;
            }

            return true;
        }

        private void NotifyStatusChanged(SessionStatusEnum status)
        {
            if (this.status == status)
            {
                return;
            }

            this.status = status;

            if (this.StatusChanged != null)
            {
                this.StatusChanged(this, status);
            }
        }

        #endregion
    }
}
