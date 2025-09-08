using ModengTerm.FileTrans.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.FileTrans
{
    /// <summary>
    /// 表示一个要上传的项目
    /// </summary>
    public class FileTask
    {
        /// <summary>
        /// 要上传的项目的Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 要上传的项目原始路径
        /// </summary>
        public string SourceFilePath { get; set; }

        /// <summary>
        /// 要上传到的路径
        /// </summary>
        public string TargetFilePath { get; set; }

        /// <summary>
        /// 任务类型
        /// </summary>
        public FileTaskTypeEnum Type { get; set; }

        /// <summary>
        /// 要上传的文件流
        /// </summary>
        public FileStream Stream { get; set; }

        /// <summary>
        /// 要写入的文件的总长度
        /// </summary>
        public ulong TotalLength { get; set; }

        /// <summary>
        /// 当前已经写入了多少字节的数据
        /// </summary>
        public ulong Offset { get; set; }

        /// <summary>
        /// 还剩余多少字节数据要写入
        /// </summary>
        public ulong Remain { get; set; }
    }
}
