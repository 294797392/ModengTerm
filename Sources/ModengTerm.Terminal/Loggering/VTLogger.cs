using ModengTerm.Document.Enumerations;
using ModengTerm.Document.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Loggering
{
    public class VTLogger
    {
        public CreateLineDelegate CreateLine { get; set; }

        public StringBuilder Builder { get; set; }

        /// <summary>
        /// 过滤器
        /// </summary>
        public LoggerFilter Filter { get; set; }

        /// <summary>
        /// 文件格式
        /// </summary>
        public ParagraphFormatEnum FileType { get; set; }

        /// <summary>
        /// 日志文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 标识该日志是否暂停记录
        /// </summary>
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
