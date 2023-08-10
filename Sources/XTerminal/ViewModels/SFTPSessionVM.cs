using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.ViewModels.SFTP;

namespace XTerminal.ViewModels
{
    /// <summary>
    /// 维护一个打开的Sftp会话
    /// </summary>
    public class SFTPSessionVM : OpenedSessionVM
    {
        #region 实例变量

        private Renci.SshNet.ScpClient ScpClient;

        #endregion

        /// <summary>
        /// 服务器的文件系统树形列表
        /// </summary>
        public SftpTreeVM RemoteTreeVM { get; private set; }

        /// <summary>
        /// 本地文件系统树形列表
        /// </summary>
        public SftpTreeVM LocalTreeVM { get; private set; }

        public SFTPSessionVM()
        {
            this.RemoteTreeVM = new SftpTreeVM();
            this.LocalTreeVM = new SftpTreeVM();
        }
    }
}
