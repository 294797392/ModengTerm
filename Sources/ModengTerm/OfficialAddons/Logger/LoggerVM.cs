using ModengTerm.Addon.Interactive;
using ModengTerm.Document;
using System.Collections.Generic;
using WPFToolkit.MVVM;

namespace ModengTerm.OfficialAddons.Logger
{
    public enum LoggerStatus
    {
        /// <summary>
        /// 日志记录是停止状态
        /// </summary>
        Stop,

        /// <summary>
        /// 正在记录日志中
        /// </summary>
        Start,
    }

    public class LoggerVM : ViewModelBase
    {
        private LoggerStatus status;
        private string filePath;

        //public CreateLineDelegate CreateLine { get; set; }

        //public StringBuilder Builder { get; set; }

        ///// <summary>
        ///// 过滤器
        ///// </summary>
        //public LoggerFilter Filter { get; set; }

        /// <summary>
        /// 文件格式
        /// </summary>
        //public ParagraphFormatEnum FileType { get; set; }

        /// <summary>
        /// 记录日志的状态
        /// </summary>
        public LoggerStatus Status 
        {
            get { return this.status; }
            set
            {
                if (this.status != value)
                {
                    this.status = value;
                    this.NotifyPropertyChanged("Status");
                }
            }
        }

        /// <summary>
        /// 日志文件路径
        /// </summary>
        public string FilePath 
        {
            get { return this.filePath; }
            set
            {
                if (this.filePath != value)
                {
                    this.filePath = value;
                    this.NotifyPropertyChanged("FilePath");
                }
            }
        }

        /// <summary>
        /// 待写入的行
        /// </summary>
        public List<VTHistoryLine> PendingLines { get; private set; }

        /// <summary>
        /// 最后一个待写入的行
        /// </summary>
        public VTHistoryLine PendingLastLine { get; set; }

        /// <summary>
        /// 记录的是哪个Tab页
        /// </summary>
        public IClientTab ClientTab { get; private set; }

        public LoggerVM(IClientTab clientTab)
        {
            this.ClientTab = clientTab;
            this.PendingLines = new List<VTHistoryLine>();
        }

        ///// <summary>
        ///// 释放Logger占用的资源
        ///// </summary>
        //public void Dispose()
        //{
        //    this.Builder.Clear();
        //}
    }
}
