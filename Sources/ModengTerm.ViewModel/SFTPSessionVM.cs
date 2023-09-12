using DotNEToolkit;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPFToolkit.MVVM;
using XTerminal.Base;
using XTerminal.Base.DataModels;
using XTerminal.Base.Enumerations;
using XTerminal.Base.SftpTrasmit;
using XTerminal.ViewModels.SFTP;

namespace XTerminal.ViewModels
{
    /// <summary>
    /// 维护一个打开的Sftp会话
    /// </summary>
    public class SFTPSessionVM : OpenedSessionVM
    {
        #region 类变量

        /// <summary>
        /// 定义所有的右键菜单列表和对应的命令执行器
        /// </summary>
        private static readonly List<ContextMenuItemVM> MenuList = new List<ContextMenuItemVM>()
        {
            new ContextMenuItemVM("传输选定的项", UploadFile, DownloadFile),
            new ContextMenuItemVM("打开", OpenSelectedItem),
            new ContextMenuItemVM("移动", MoveSelectedItem),
            new ContextMenuItemVM("删除", DeleteSelectedItem),
            new ContextMenuItemVM("重命名", RenameSelectedItem),
            new ContextMenuItemVM("属性", LocalShowProperty, SftpShowProperty),
            new ContextMenuItemVM("新建文件夹", CreateDirectory),
            new ContextMenuItemVM("新建文件", CreateFile),
            new ContextMenuItemVM("刷新", RefreshFileSystemTree),
        };

        private static log4net.ILog logger = log4net.LogManager.GetLogger("SFTPSessionVM");

        #endregion

        #region 实例变量

        private SftpClient sftpClient;
        private string serverAddress;
        private int serverPort;
        private SftpTransmitter transmitter;

        #endregion

        #region 属性

        /// <summary>
        /// SFTP服务地址
        /// </summary>
        public string ServerAddress
        {
            get { return this.serverAddress; }
            set
            {
                if (this.serverAddress != value)
                {
                    this.serverAddress = value;
                    this.NotifyPropertyChanged("ServerAddress");
                }
            }
        }

        /// <summary>
        /// SFTP服务端口号
        /// </summary>
        public int ServerPort
        {
            get { return this.serverPort; }
            set
            {
                if (this.serverPort != value)
                {
                    this.serverPort = value;
                    this.NotifyPropertyChanged("ServerPort");
                }
            }
        }

        /// <summary>
        /// 服务器的文件系统树形列表
        /// </summary>
        public FileSystemTreeVM ServerTreeVM { get; private set; }

        /// <summary>
        /// 本地文件系统树形列表
        /// </summary>
        public FileSystemTreeVM ClientTreeVM { get; private set; }

        /// <summary>
        /// 当前正在上传的文件列表
        /// </summary>
        public BindableCollection<TransferFileVM> TransferFileList { get; private set; }

        #endregion

        #region 构造方法

        public SFTPSessionVM()
        {
        }

        #endregion

        #region 公开接口

        protected override int OnOpen(XTermSession session)
        {
            this.TransferFileList = new BindableCollection<TransferFileVM>();

            #region 连接SFTP服务器

            //SFTPAuthTypeEnum authType = session.GetOption<SFTPAuthTypeEnum>(OptionKeyEnum.SFTP_AUTH_TYPE);
            //string serverAddress = session.GetOption<string>(OptionKeyEnum.SFTP_SERVER_ADDRESS);
            //int serverPort = session.GetOption<int>(OptionKeyEnum.SFTP_SERVER_PORT);
            //string userName = session.GetOption<string>(OptionKeyEnum.SSH_SERVER_USER_NAME);
            //string password = session.GetOption<string>(OptionKeyEnum.SSH_SERVER_PASSWORD);

            //AuthenticationMethod authentication = null;
            //switch (authType)
            //{
            //    case SFTPAuthTypeEnum.None:
            //        {
            //            authentication = new NoneAuthenticationMethod(userName);
            //            break;
            //        }

            //    case SFTPAuthTypeEnum.Password:
            //        {
            //            authentication = new PasswordAuthenticationMethod(userName, password);
            //            break;
            //        }

            //    case SFTPAuthTypeEnum.PrivateKey:
            //        {
            //            //byte[] privateKey = File.ReadAllBytes(privateKeyFile);
            //            //using (MemoryStream ms = new MemoryStream(privateKey))
            //            //{
            //            //    var keyFile = new PrivateKeyFile(ms, passphrase);
            //            //    authentication = new PrivateKeyAuthenticationMethod(userName, keyFile);
            //            //}
            //            //break;
            //            throw new NotImplementedException();
            //        }

            //    default:
            //        throw new NotImplementedException();
            //}

            //ConnectionInfo connectionInfo = new ConnectionInfo(serverAddress, serverPort, userName, authentication);
            //this.sftpClient = new SftpClient(connectionInfo);
            //this.sftpClient.Connect();

            #endregion

            //this.ServerTreeVM = new SftpFileSystemTreeVM(this.sftpClient);
            //this.ServerTreeVM.InitialDirectory = session.GetOption<string>(OptionKeyEnum.SFTP_SERVER_INITIAL_DIRECTORY);
            //this.ServerTreeVM.Initialize();

            this.ClientTreeVM = new LocalFileSystemTreeVM();
            this.ClientTreeVM.SftpClient = this.sftpClient;
            this.ClientTreeVM.ContextMenus.AddRange(MenuList);
            this.ClientTreeVM.InitialDirectory = session.GetOption<string>(OptionKeyEnum.SFTP_CLIENT_INITIAL_DIRECTORY);
            this.ClientTreeVM.Initialize();

            this.transmitter = new SftpTransmitter();
            this.transmitter.Threads = 1;
            this.transmitter.Client = this.sftpClient;
            this.transmitter.StatusChanged += this.Transmitter_StatusChanged;
            this.transmitter.Initialize();

            return ResponseCode.SUCCESS;
        }

        protected override void OnClose()
        {
            this.transmitter.StatusChanged -= this.Transmitter_StatusChanged;
            this.transmitter.Release();

            this.ServerTreeVM.Release();
            this.ClientTreeVM.Release();

            this.sftpClient.Disconnect();
            this.sftpClient.Dispose();
        }

        #endregion

        #region 实例方法

        #endregion

        #region 事件处理器

        private void Transmitter_StatusChanged(SftpTransmit transmit)
        {
        }

        #endregion

        #region 命令响应

        /// <summary>
        /// 从本地上传文件到Sftp服务器
        /// </summary>
        /// <param name="fileSystemTree"></param>
        private static void UploadFile(FileSystemTreeVM fileSystemTree)
        {
            IEnumerable<FileSystemTreeNodeVM> uploadList = fileSystemTree.Context.SelectedItems.Cast<FileSystemTreeNodeVM>();


        }

        /// <summary>
        /// 从Sftp服务器下载文件到本地
        /// </summary>
        /// <param name="fileSystemTree"></param>
        private static void DownloadFile(FileSystemTreeVM fileSystemTree)
        {
            IEnumerable<FileSystemTreeNodeVM> downloadList = fileSystemTree.Context.SelectedItems.Cast<FileSystemTreeNodeVM>();
        }


        private static void OpenSelectedItem(FileSystemTreeVM fileSystemTree)
        {
            
        }

        private static void MoveSelectedItem(FileSystemTreeVM fileSystemTree)
        { }

        private static void DeleteSelectedItem(FileSystemTreeVM fileSystemTree)
        { }

        private static void RenameSelectedItem(FileSystemTreeVM fileSystemTree)
        { }

        private static void LocalShowProperty(FileSystemTreeVM fileSystemTree)
        {
            FileSystemTreeNodeVM selectedItem = fileSystemTree.Context.SelectedItem as FileSystemTreeNodeVM;
            if(selectedItem == null)
            {
                return;
            }

            Win32APIHelper.ShowFileProperties(selectedItem.FullPath);
        }

        private static void SftpShowProperty(FileSystemTreeVM fileSystemTree)
        {
        }

        private static void CreateDirectory(FileSystemTreeVM fileSystemTree)
        { }

        private static void CreateFile(FileSystemTreeVM fileSystemTree)
        {
            
        }

        private static void RefreshFileSystemTree(FileSystemTreeVM fileSystemTree)
        {
            fileSystemTree.EnterDirectory(fileSystemTree.CurrentDirectory);
        }

        #endregion
    }
}
