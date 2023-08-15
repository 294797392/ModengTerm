using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base.SftpTrasmit;

namespace XTerminal.Base.SftpTrasmit
{
    /// <summary>
    /// 描述一个Sftp传输信息
    /// </summary>
    public class SftpTransmit
    {
        /// <summary>
        /// 文件传输类型
        /// </summary>
        public SftpTransmitTypeEnum Type { get; set; }

        /// <summary>
        /// 要操作的文件的本地路径
        /// </summary>
        public string LocalPath { get; set; }

        /// <summary>
        /// 要操作的文件的远程路径
        /// </summary>
        public string RemotePath { get; set; }

        /// <summary>
        /// 传输进度
        /// </summary>
        public double Progress { get; set; }

        /// <summary>
        /// 传输状态
        /// </summary>
        public TransmitStatusEnum Status { get; set; }
    }
}