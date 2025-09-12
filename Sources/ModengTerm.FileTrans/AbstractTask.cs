using ModengTerm.FileTrans.Enumerations;
using ModengTerm.Ftp.Enumerations;
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
    public abstract class AbstractTask
    {
        /// <summary>
        /// 要上传的项目的Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 任务类型
        /// </summary>
        public abstract FsOperationTypeEnum Type { get; }

        /// <summary>
        /// 子任务
        /// 子任务必须在父任务运行结束之后再运行
        /// </summary>
        public List<AbstractTask> SubTasks { get; private set; }

        public AbstractTask() 
        {
            this.SubTasks = new List<AbstractTask>();
        }
    }

    public class UploadFileTask : AbstractTask
    {
        public override FsOperationTypeEnum Type => FsOperationTypeEnum.UploadFile;

        /// <summary>
        /// 要上传的项目原始路径
        /// </summary>
        public string SourceFilePath { get; set; }

        /// <summary>
        /// 要上传到的路径
        /// </summary>
        public string TargetFilePath { get; set; }
    }

    public class CreateDirectoryTask : AbstractTask
    {
        public override FsOperationTypeEnum Type => FsOperationTypeEnum.CreateDirectory;

        /// <summary>
        /// 要创建的目录完整路径
        /// </summary>
        public string DirectoryPath { get; set; }
    }

    public class DeleteDirectoryTask : AbstractTask
    {
        public override FsOperationTypeEnum Type => FsOperationTypeEnum.DeleteDirectory;

        /// <summary>
        /// 要删除的目录的完整路径
        /// </summary>
        public string DirectoryPath { get; set; }
    }

    public class DeleteFileTask : AbstractTask
    {
        public override FsOperationTypeEnum Type => FsOperationTypeEnum.DeleteFile;

        /// <summary>
        /// 要删除的文件完整路径
        /// </summary>
        public string FilePath { get; set; }
    }
}
