using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Base.SftpTrasmit
{
    public enum SftpTransmitTypeEnum
    {
        /// <summary>
        /// 从Sftp服务器下载文件到本地
        /// </summary>
        Download,

        /// <summary>
        /// 从本地上传文件到Sftp服务器
        /// </summary>
        Upload
    }
}
