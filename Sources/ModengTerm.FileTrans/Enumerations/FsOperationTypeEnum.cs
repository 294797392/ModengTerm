using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Ftp.Enumerations
{
    /// <summary>
    /// 定义操作类型
    /// </summary>
    public enum FsOperationTypeEnum
    {
        /// <summary>
        /// 从本地上传文件到服务器
        /// </summary>
        UploadFile,

        /// <summary>
        /// 下载操作
        /// </summary>
        DownloadFile,

        /// <summary>
        /// 删除目录
        /// </summary>
        DeleteDirectory,

        /// <summary>
        /// 删除文件
        /// </summary>
        DeleteFile,

        CreateDirectory,
    }
}
