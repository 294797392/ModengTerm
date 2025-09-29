using ModengTerm.Base.Enumerations;
using ModengTerm.FileTrans.DataModels;
using ModengTerm.FileTrans.Enumerations;
using ModengTerm.Ftp.Enumerations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.FileTrans.Clients
{
    public class FileSystemTransport
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("FileSystemTransport");

        #region 公开事件

        /// <summary>
        /// 当Session状态发生改变的时候触发
        /// </summary>
        public event Action<object, SessionStatusEnum> StatusChanged;

        /// <summary>
        /// 当上传状态改变的时候触发
        /// </summary>
        public event Action<AgentTask, double> UploadProgress;

        #endregion

        #region 实例变量

        private FileSystem fileSystem;
        private SessionStatusEnum status;
        private FileSystemOptions options;

        #endregion

        #region 属性

        /// <summary>
        /// 获取文件系统类型
        /// </summary>
        public FileSystemTypeEnum FileSystemType { get { return this.options.Type; } }

        #endregion

        #region 公开接口

        public void OpenAsync(FileSystemOptions options)
        {
            this.options = options;

            this.fileSystem = FileSystemFactory.Create(options);
            this.fileSystem.Options = options;

            this.NotifyStatusChanged(SessionStatusEnum.Connecting);

            Task.Factory.StartNew(() =>
            {
                try
                {
                    this.fileSystem.Open();
                    this.fileSystem.ChangeDirectory(options.InitialDirectory);

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
            FileSystem client = this.fileSystem;

            if (client == null)
            {
                return;
            }

            client.Close();

            this.fileSystem = null;

            this.NotifyStatusChanged(SessionStatusEnum.Disconnected);
        }

        public List<FsItemInfo> ListItems(string directory)
        {
            if (!this.CheckStatus())
            {
                return null;
            }

            try
            {
                return this.fileSystem.ListItems(directory);
            }
            catch (Exception ex)
            {
                logger.Error("ListFiles异常", ex);
                return null;
            }
        }

        public List<FsItemInfo> GetDirectoryChains(string directory)
        {
            try
            {
                return this.fileSystem.GetDirectoryChains(directory);
            }
            catch (Exception ex)
            {
                logger.Error("GetDirectoryChains异常", ex);
                return null;
            }
        }

        public List<FsItemInfo> ListRootItems()
        {
            if (!this.CheckStatus())
            {
                return null;
            }

            try
            {
                return this.fileSystem.ListRootItems();
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
                this.fileSystem.ChangeDirectory(directory);
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
                this.fileSystem.CreateDirectory(dir);
                return true;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("CreateDirectory异常, {0}, {1}", dir, ex);
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
                this.fileSystem.DeleteFile(filePath);
                return true;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("删除文件异常, {0}, {1}", filePath, ex);
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
                this.fileSystem.DeleteDirectory(directoryPath);
                return true;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("删除目录异常, {0}, {1}", directoryPath, ex);
                return false;
            }
        }

        public bool RenameDirectory(string oldPath, string newPath)
        {
            if (!this.CheckStatus())
            {
                return false;
            }

            try
            {
                this.fileSystem.RenameDirectory(oldPath, newPath);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error("重命名目录异常", ex);
                return false;
            }
        }

        public bool RenameFile(string oldPath, string newPath)
        {
            if (!this.CheckStatus())
            {
                return false;
            }

            try
            {
                this.fileSystem.RenameFile(oldPath, newPath);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error("重命名文件异常", ex);
                return false;
            }
        }

        #endregion

        #region 实例方法

        private bool CheckStatus()
        {
            FileSystem client = this.fileSystem;

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
