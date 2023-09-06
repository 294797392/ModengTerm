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
        Action,

        RawRead
    }

    /// <summary>
    /// 提供一些记录日志的工具函数
    /// </summary>
    public class VTDebug : SingletonObject<VTDebug>
    {
        /// <summary>
        /// 存储日志分类的上下文信息
        /// </summary>
        public class LogCategory
        {
            private bool enabled;

            public string Name { get; set; }

            /// <summary>
            /// 日志分类
            /// </summary>
            public VTDebugCategoryEnum Category { get; private set; }

            /// <summary>
            /// 日志文件的完整路径
            /// </summary>
            public string FilePath { get; set; }

            /// <summary>
            /// 是否记录该类型的日志
            /// </summary>
            public bool Enabled
            {
                get { return this.enabled; }
                set
                {
                    if (this.enabled != value)
                    {
                        this.enabled = value;

                        if (value)
                        {
                            this.OnStart();
                        }
                        else
                        {
                            this.OnStop();
                        }
                    }
                }
            }

            public LogCategory(VTDebugCategoryEnum category)
            {
                this.Category = category;
                this.Name = this.Category.ToString();
            }

            public void OnStart()
            {
                if (File.Exists(this.FilePath))
                {
                    try
                    {
                        File.Delete(this.FilePath);
                    }
                    catch (Exception ex)
                    {
                        logger.Error("删除日志文件异常", ex);
                    }
                }
            }

            public void OnStop()
            { }
        }

        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTDebug");

        private LogCategory actionCategory;
        private LogCategory rawReadCategory;
        private Dictionary<VTDebugCategoryEnum, LogCategory> categoryMap;
        public List<LogCategory> Categories { get; private set; }

        public VTDebug()
        {
            this.Categories = new List<LogCategory>();
            this.categoryMap = new Dictionary<VTDebugCategoryEnum, LogCategory>();

            this.actionCategory = this.CreateCategory(VTDebugCategoryEnum.Action);
            this.rawReadCategory = this.CreateCategory(VTDebugCategoryEnum.RawRead);
        }

        private LogCategory CreateCategory(VTDebugCategoryEnum categoryEnum)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("log_{0}.txt", categoryEnum.ToString()));

            LogCategory category = new LogCategory(categoryEnum)
            {
                FilePath = filePath
            };

            this.categoryMap[categoryEnum] = category;
            this.Categories.Add(category);

            return category;
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

        public void WriteRawRead(byte[] bytes, int size)
        {
            if (!this.rawReadCategory.Enabled)
            {
                return;
            }

            string message = bytes.Take(size).Select(v => ((int)v).ToString()).Join(",");
            File.AppendAllText(this.rawReadCategory.FilePath, message + ",");
        }
    }
}