using DotNEToolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Parser;

namespace ModengTerm.Terminal
{
    public enum VTDebugCategoryEnum
    {
        /// <summary>
        /// 执行动作的日志
        /// </summary>
        Action
    }

    /// <summary>
    /// 提供一些记录日志的工具函数
    /// </summary>
    public class VTDebug : SingletonObject<VTDebug>
    {
        /// <summary>
        /// 存储日志分类的上下文信息
        /// </summary>
        private class LogCategory
        {
            /// <summary>
            /// 日志分类
            /// </summary>
            public VTDebugCategoryEnum Category { get; set; }

            /// <summary>
            /// 日志文件的完整路径
            /// </summary>
            public string FilePath { get; set; }

            /// <summary>
            /// 是否记录该类型的日志
            /// </summary>
            public bool Enabled { get; set; }

            public LogCategory()
            {
            }
        }

        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTDebug");

        private LogCategory actionCategory;
        private Dictionary<VTDebugCategoryEnum, LogCategory> categoryMap = new Dictionary<VTDebugCategoryEnum, LogCategory>();

        public VTDebug()
        {
            this.actionCategory = this.CreateCategory(VTDebugCategoryEnum.Action);
        }

        private LogCategory CreateCategory(VTDebugCategoryEnum categoryEnum)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("log_{0}.txt", categoryEnum.ToString()));

            LogCategory category = new LogCategory()
            {
                Category = categoryEnum,
                FilePath = filePath
            };

            this.categoryMap[categoryEnum] = category;

            return category;
        }

        public void StartLogger(VTDebugCategoryEnum categoryEnum)
        {
            LogCategory category = this.categoryMap[categoryEnum];
            category.Enabled = true;
            if (File.Exists(category.FilePath))
            {
                File.Delete(category.FilePath);
            }
        }

        public void StopLogger(VTDebugCategoryEnum categoryEnum)
        {
            LogCategory category = this.categoryMap[categoryEnum];
            category.Enabled = false;
        }

        public void WriteAction(VTActions action, string format, params object[] param)
        {
            if (!this.actionCategory.Enabled)
            {
                return;
            }

            string message = string.Format(format, param);
            string log = string.Format("[{0},{1}]", action, message);
            File.AppendAllText(this.actionCategory.FilePath, log);
            File.AppendAllText(this.actionCategory.FilePath, "\r\n");
        }
    }
}