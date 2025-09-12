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
        /// 上传操作
        /// </summary>
        UploadFile,

        /// <summary>
        /// 下载操作
        /// </summary>
        Download,

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
