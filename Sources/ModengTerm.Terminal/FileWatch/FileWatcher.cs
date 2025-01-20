using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Terminal.Session;
using Renci.SshNet;
using System.IO;

namespace ModengTerm.Terminal.FileWatch
{
    public abstract class FileWatcher
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("FileWatcher");

        #endregion

        #region 公开事件

        public event Action<FileWatcher, string> NewLine;

        #endregion

        #region 实例变量

        private bool isWatching;

        #endregion

        #region 属性

        public XTermSession Session { get; set; }

        public SessionDriver SessionDriver { get; set; }

        /// <summary>
        /// 判断是否有数据可读
        /// </summary>
        public abstract int Avaliable { get; }

        public string FilePath { get; set; }

        #endregion

        #region 公开接口

        public int Initialize()
        {
            // 初始化

            return this.OnInitialize();
        }

        public void Release()
        {
            this.OnRelease();

            // 释放资源
        }

        /// <summary>
        /// 非阻塞的方式读取一行或多行数据
        /// </summary>
        /// <returns></returns>
        public List<string> ReadLine()
        {
            throw new NotImplementedException();
        }

        //public int Start(string filePath)
        //{
        //    try
        //    {
        //        int code = this.Initialize(filePath);
        //        if (code != ResponseCode.SUCCESS)
        //        {
        //            return code;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("初始化文件监控异常, {0}", ex);
        //        return ResponseCode.FAILED;
        //    }

        //    this.isWatching = true;

        //    Task.Factory.StartNew(() =>
        //    {
        //        while (this.isWatching)
        //        {
        //            try
        //            {
        //                string line = this.ReadLine();

        //                this.NewLine?.Invoke(this, line);
        //            }
        //            catch (Exception ex)
        //            {
        //                logger.Error("监控文件异常", ex);
        //                break;
        //            }
        //        }

        //        this.isWatching = false;
        //    });

        //    return ResponseCode.SUCCESS;
        //}

        //public void Stop()
        //{
        //    this.isWatching = false;

        //    this.Release();
        //}

        #endregion

        #region 抽象方法

        protected abstract int OnInitialize();

        protected abstract void OnRelease();

        #endregion
    }

    public class SshFileWatcher : FileWatcher
    {
        #region 实例变量

        private SshNetSession sshNetDrv;
        private SshClient sshClient;
        private SshCommand sshCommand;

        #endregion

        #region FileWatcher

        public override int Avaliable => throw new NotImplementedException();

        protected override int OnInitialize()
        {
            return ResponseCode.SUCCESS;
        }

        protected override void OnRelease()
        {
        }

        //protected override int OnStart(string filePath)
        //{
        //    this.sshNetDrv = this.SessionDriver as SshNetSession;
        //    this.sshClient = this.sshNetDrv.SshClient;

        //    string tail = string.Format("tail -f {0}", filePath);
        //    this.sshCommand = this.sshClient.CreateCommand(tail);
        //    this.sshCommand.Execute

        //    StreamReader streamReader = new StreamReader(this.sshCommand.OutputStream);

        //    while (true)
        //    {
        //        this.sshCommand.

        //        string line = streamReader.ReadLine();
        //    }

        //    return ResponseCode.SUCCESS;
        //}

        #endregion
    }
}
