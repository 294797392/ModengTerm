using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base;
using XTerminal.Base.DataModels;
using XTerminal.Base.Enumerations;
using XTerminal.ViewModels.SFTP;

namespace XTerminal.ViewModels
{
    /// <summary>
    /// 维护一个打开的Sftp会话
    /// </summary>
    public class SFTPSessionVM : OpenedSessionVM
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("SFTPSessionVM");

        #endregion

        #region 实例变量

        private SftpClient sftpClient;

        #endregion

        #region 属性

        /// <summary>
        /// 服务器的文件系统树形列表
        /// </summary>
        public FileSystemTreeVM ServerTreeVM { get; private set; }

        /// <summary>
        /// 本地文件系统树形列表
        /// </summary>
        public FileSystemTreeVM ClientTreeVM { get; private set; }

        #endregion

        #region 构造方法

        public SFTPSessionVM()
        {
        }

        #endregion

        #region 公开接口

        public override int Open(XTermSession session)
        {
            this.sftpClient = new SftpClient("ubuntu-dev", "oheiheiheiheihei", "18612538605");
            this.sftpClient.Connect();

            this.ServerTreeVM = new SftpFileSystemTreeVM(this.sftpClient);
            this.ServerTreeVM.InitialDirectory = session.GetOption<string>(OptionKeyEnum.SFTP_SERVER_INITIAL_DIRECTORY);
            this.ServerTreeVM.Initialize();

            this.ClientTreeVM = new LocalFileSystemTreeVM();
            this.ClientTreeVM.InitialDirectory = session.GetOption<string>(OptionKeyEnum.SFTP_CLIENT_INITIAL_DIRECTORY);
            this.ClientTreeVM.Initialize();

            return ResponseCode.SUCCESS;
        }

        public override void Close()
        {
            this.ServerTreeVM.Release();
            this.ClientTreeVM.Release();

            this.sftpClient.Disconnect();
            this.sftpClient.Dispose();
        }

        #endregion
    }
}
