using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.ViewModels.SFTP
{
    public enum TransferTypeEnum
    {
        /// <summary>
        /// 向SFTP服务器上传文件
        /// </summary>
        Upload,

        /// <summary>
        /// 从SFTP服务器下载文件
        /// </summary>
        Download
    }
}
