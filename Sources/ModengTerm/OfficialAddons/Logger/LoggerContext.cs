using ModengTerm.Addon.Interactive;
using ModengTerm.Document;
using System.Collections.Generic;

namespace ModengTerm.OfficialAddons.Logger
{
    public enum LoggerStates
    {
        Start,
        Stop
    }

    public class LoggerContext
    {
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
        public LoggerStates Status { get; set; }

        /// <summary>
        /// 日志文件路径
        /// </summary>
        public string FilePath { get; set; }

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
        public IClientTab ClientTab { get; set; }

        public LoggerContext() 
        {
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
