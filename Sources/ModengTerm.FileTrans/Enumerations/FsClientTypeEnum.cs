using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.FileTrans.Enumerations
{
    /// <summary>
    /// 文件系统客户端类型
    /// 文件系统客户端用来增删改查文件系统
    /// </summary>
    public enum FsClientTypeEnum
    {
        /// <summary>
        /// Sftp客户端
        /// </summary>
        Sftp,

        /// <summary>
        /// 本地文件系统
        /// </summary>
        LocalFs
    }
}
