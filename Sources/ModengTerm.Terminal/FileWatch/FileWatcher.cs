using ModengTerm.Base.DataModels;
using ModengTerm.Terminal.Engines;

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

        public ChannelBase SessionDriver { get; set; }

        /// <summary>
        /// 判断是否有数据可读
        /// </summary>
        public abstract long Avaliable { get; }

        public string FilePath { get; private set; }

        #endregion

        #region 公开接口

        public int Initialize(string filePath)
        {
            // 初始化
            this.FilePath = filePath;

            return this.OnInitialize();
        }

        public void Release()
        {
            this.OnRelease();

            // 释放资源
        }

        /// <summary>
        /// 阻塞的方式读取缓冲区数据
        /// </summary>
        /// <returns></returns>
        public abstract int Read(byte[] buffer, int offset, int size);

        #endregion

        #region 抽象方法

        protected abstract int OnInitialize();

        protected abstract void OnRelease();

        #endregion
    }
}
