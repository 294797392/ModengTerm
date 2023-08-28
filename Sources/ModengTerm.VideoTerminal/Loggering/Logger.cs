using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Document;

namespace ModengTerm.Terminal
{
    /// <summary>
    /// 实现一个终端历史记录的日志记录器
    /// </summary>
    public class Logger
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("Logger");

        #endregion

        #region 实例变量

        #endregion

        #region 属性

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

        public void Write(VTHistoryLine historyLine)
        {

        }

        #endregion
    }
}