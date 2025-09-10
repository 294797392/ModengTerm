using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Base.Enumerations
{
    /// <summary>
    /// 指定菜单显示的区域
    /// </summary>
    public enum MenuScopeEnum
    {
        /// <summary>
        /// 控制台
        /// </summary>
        Console,

        Ssh,

        SerialPort,

        Tcp,

        /// <summary>
        /// sftp本地文件列表
        /// </summary>
        FtpLocalFileList,

        /// <summary>
        /// sftp远程文件列表
        /// </summary>
        FtpRemoteFileList
    }
}
