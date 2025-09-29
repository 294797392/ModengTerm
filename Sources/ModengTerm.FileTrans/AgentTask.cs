using ModengTerm.Ftp.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Ftp
{
    public enum AgentTaskStates
    {
        /// <summary>
        /// 正在处理中
        /// </summary>
        Processing,

        /// <summary>
        /// 处理成功
        /// </summary>
        Success,

        /// <summary>
        /// 处理失败
        /// </summary>
        Failure,
    }

    /// <summary>
    /// 表示一个要上传的项目
    /// </summary>
    public class AgentTask
    {
        /// <summary>
        /// 要上传的项目的Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 任务类型
        /// </summary>
        public TaskTypeEnum Type { get; set; }

        /// <summary>
        /// 父任务
        /// </summary>
        public AgentTask Parent { get; set; }

        /// <summary>
        /// 用户数据
        /// </summary>
        public object UserData { get; set; }

        /// <summary>
        /// 原始文件完整路径
        /// </summary>
        public string SourceFilePath { get; set; }

        /// <summary>
        /// 目标文件完整路径
        /// 删除，新建路径
        /// </summary>
        public string TargetFilePath { get; set; }

        /// <summary>
        /// 该任务的操作状态
        /// </summary>
        internal AgentTaskStates State { get; set; }

        /// <summary>
        /// 上传进度
        /// </summary>
        internal double Progress { get; set; }

        /// <summary>
        /// 子任务
        /// 子任务必须在父任务运行结束之后再运行
        /// </summary>
        public List<AgentTask> SubTasks { get; private set; }

        public AgentTask()
        {
            this.SubTasks = new List<AgentTask>();
        }
    }
}
