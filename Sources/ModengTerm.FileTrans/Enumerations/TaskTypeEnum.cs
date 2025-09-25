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
    public enum TaskTypeEnum
    {
        /// <summary>
        /// 从本地上传文件到服务器
        /// </summary>
        UploadFile,

        /// <summary>
        /// 从服务器下载文件到本地
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

        /// <summary>
        /// 创建目录
        /// </summary>
        CreateDirectory,
        CreateLocalDirectory
    }
}
