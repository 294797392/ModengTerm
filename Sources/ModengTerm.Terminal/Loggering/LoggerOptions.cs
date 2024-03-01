using ModengTerm.Document.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Loggering
{
    public class LoggerOptions
    {
        /// <summary>
        /// 日志文件名字
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 要保存的日志格式
        /// </summary>
        public LogFileTypeEnum FileType { get; set; }

        /// <summary>
        /// 日志过滤器类型
        /// </summary>
        public FilterTypeEnum FilterType { get; set; }

        /// <summary>
        /// 过滤内容
        /// </summary>
        public string FilterText { get; set; }

        /// <summary>
        /// true就是胡烈大小写
        /// false就是不胡忽略大小写
        /// </summary>
        public bool IgnoreCase { get; set; }
    }
}
