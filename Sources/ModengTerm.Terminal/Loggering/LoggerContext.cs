using ModengTerm.Terminal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Loggering
{
    internal class LoggerContext
    {
        public StringBuilder Builder { get; set; }

        /// <summary>
        /// 过滤参数
        /// </summary>
        public LoggerOptions Options { get; set; }

        /// <summary>
        /// 过滤器
        /// </summary>
        public LoggerFilter Filter { get; set; }

        /// <summary>
        /// 终端
        /// </summary>
        public VideoTerminal VideoTerminal { get; set; }

        /// <summary>
        /// 文件格式
        /// </summary>
        public LogFileTypeEnum FileType
        {
            get { return this.Options.FileType; }
        }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath
        {
            get { return this.Options.FilePath; }
        }

        /// <summary>
        /// 下一次要记录的行的索引
        /// </summary>
        public int NextLine { get; set; }

        public bool IsPaused { get; set; }

        /// <summary>
        /// 释放Logger占用的资源
        /// </summary>
        public void Dispose()
        {
            this.Builder.Clear();
        }
    }
}
