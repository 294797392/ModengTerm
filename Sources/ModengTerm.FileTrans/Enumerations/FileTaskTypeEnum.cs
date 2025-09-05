using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.FileTrans.Enumerations
{
    public enum FileTaskTypeEnum
    {
        /// <summary>
        /// 上传文件任务
        /// </summary>
        UploadFile,

        /// <summary>
        /// 下载文件任务
        /// </summary>
        DownloadFile,

        /// <summary>
        /// 创建目录任务
        /// </summary>
        CreateDirectory
    }
}
