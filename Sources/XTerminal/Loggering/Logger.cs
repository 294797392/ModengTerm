using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Loggering
{
    /// <summary>
    /// 实现一个终端历史记录的日志记录器
    /// </summary>
    public class Logger
    {
        #region 实例变量

        #endregion

        #region 属性

        /// <summary>
        /// 所记录的终端
        /// </summary>
        public VideoTerminal videoTerminal { get; set; }

        /// <summary>
        /// 日志文件名字
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 要保存的日志格式
        /// </summary>
        public LogFileTypeEnum FileType { get; set; }

        #endregion

        #region 公开接口

        public void Initialize()
        {

        }

        /// <summary>
        /// 处理当前的日志记录
        /// </summary>
        public void Process()
        {
            
        }

        #endregion
    }
}