using ModengTerm.Addon;
using ModengTerm.Addon.ClientBridges;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.FileTrans.Clients;
using ModengTerm.UserControls.FtpUserControls;
using ModengTerm.ViewModel;
using ModengTerm.ViewModel.Ftp;
using ModengTerm.ViewModel.Terminal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModengTerm.UserControls.FtpUserControls
{
    /// <summary>
    /// SftpWorkbenchUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class FtpSessionUserControl : UserControl, ISessionContent
    {
        #region 实例变量

        private XTermSession sesison;
        private FtpSessionVM ftpSession;
        private FileListVM clientFsTree;
        private FileListVM serverFsTree;

        #endregion

        #region 属性

        public XTermSession Session
        {
            get { return this.sesison; }
            set
            {
                if (this.sesison != value)
                {
                    this.sesison = value;
                }
            }
        }

        #endregion

        #region 构造方法

        static FtpSessionUserControl() 
        {
            // 注册文件列表右键菜单事件处理器
            Client.RegisterCommand(FtpCommandKeys.CLIENT_OPEN_ITEM, OnFtpOpenClientItem);
            Client.RegisterCommand(FtpCommandKeys.CLIENT_UPLOAD_ITEM, OnFtpUploadClientItem);
            Client.RegisterCommand(FtpCommandKeys.CLIENT_DELETE_ITEM, OnFtpDeleteItem, FtpRoleEnum.Client);
            Client.RegisterCommand(FtpCommandKeys.CLIENT_RENAME_ITEM, OnFtpRenameItem, FtpRoleEnum.Client);
        }

        public FtpSessionUserControl()
        {
            InitializeComponent();
        }

        #endregion

        #region ISessionContent

        public int Open(OpenedSessionVM sessionVM)
        {
            FtpSessionVM ftpSession = sessionVM as FtpSessionVM;
            ftpSession.Open();

            this.clientFsTree = ftpSession.ClientFsTree;
            this.serverFsTree = ftpSession.ServerFsTree;

            FileListUserControlClient.FtpSession = ftpSession;
            FileListUserControlClient.FileListContextMenu = this.CreateFileListContextMenu(clientFsTree.ContextMenus);
            FileListUserControlClient.FileListContextMenu.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(this.FileListMenuItem_Click));
            FileListUserControlServer.FtpSession = ftpSession;
            FileListUserControlServer.FileListContextMenu = this.CreateFileListContextMenu(serverFsTree.ContextMenus);
            FileListUserControlServer.FileListContextMenu.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(this.FileListMenuItem_Click));

            base.DataContext = ftpSession;

            this.ftpSession = ftpSession;

            return ResponseCode.SUCCESS;
        }

        public void Close()
        {
            FtpSessionVM ftpSession = this.ftpSession;

            ftpSession.Close();
            ftpSession.Release();

            FileListUserControlClient.FtpSession = null;
            FileListUserControlClient.FileListContextMenu.RemoveHandler(MenuItem.ClickEvent, this.FileListMenuItem_Click);
            FileListUserControlServer.FtpSession = null;
            FileListUserControlServer.FileListContextMenu.RemoveHandler(MenuItem.ClickEvent, this.FileListMenuItem_Click);

            base.DataContext = null;

            this.ftpSession = null;
        }

        public bool HasInputFocus()
        {
            return true;
        }

        public bool SetInputFocus()
        {
            return true;
        }

        #endregion

        #region 实例方法

        private ContextMenu CreateFileListContextMenu(IEnumerable itemsSource) 
        {
            ContextMenu ctxMenu = new ContextMenu()
            {
                ItemsSource = itemsSource,
                ItemTemplate = this.FindResource("HierarchicalDataTemplateFileList") as HierarchicalDataTemplate,
                Style = App.Current.FindResource("StyleContextMenu") as Style
            };

            return ctxMenu;
        }

        #endregion

        #region 事件处理器

        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcFsTree">从哪颗树上开始拖动</param>
        /// <param name="dstFsTree">要拖动到哪颗树上</param>
        /// <param name="srcFsItems">拖拽的文件列表</param>
        /// <param name="dstDir">要传输到的目录</param>
        private void FileSystemTreeUserControl_StartTransfer(FileListVM srcFsTree, FileListVM dstFsTree, List<FileItemVM> srcFsItems, string dstDir)
        {
            this.ftpSession.TransferFile(srcFsTree, dstFsTree, srcFsItems, dstDir);
        }

        private void FileListMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItemVM menuItemVm = (e.OriginalSource as FrameworkElement).DataContext as MenuItemVM;
            ClientUtils.DispatchCommand(menuItemVm);
        }

        #endregion

        #region 默认右键菜单处理器

        private static void OnFtpOpenClientItem(CommandArgs e)
        {
            (e.ActiveTab as FtpSessionVM).FtpOpenClientItem();
        }

        private static void OnFtpDeleteItem(CommandArgs e)
        {
            (e.ActiveTab as FtpSessionVM).FtpDeleteItem((FtpRoleEnum)e.UserData);
        }

        private static void OnFtpUploadClientItem(CommandArgs e)
        {
            (e.ActiveTab as FtpSessionVM).FtpUploadClientItem();
        }

        private static void OnFtpRenameItem(CommandArgs e)
        {
            FtpRoleEnum ftpRole = (FtpRoleEnum)e.UserData;
            FtpSessionVM ftpSession = e.ActiveTab as FtpSessionVM;
            FtpSessionUserControl ftpSessionUserControl = ftpSession.Content as FtpSessionUserControl;
            FileListUserControl fileListUserControl = null;

            FileListVM fsTree = null;

            if (ftpRole == FtpRoleEnum.Client)
            {
                fsTree = ftpSession.ClientFsTree;
                fileListUserControl = ftpSessionUserControl.FileListUserControlClient;
            }
            else
            {
                fsTree = ftpSession.ServerFsTree;
                fileListUserControl = ftpSessionUserControl.FileListUserControlServer;
            }

            FileItemVM fsItem = fsTree.SelectedItem as FileItemVM;
            if (fsItem == null)
            {
                return;
            }

            fsItem.EditName = fsItem.Name;
            fsItem.State = FsItemStates.EditName;
            fileListUserControl.RenameItem(fsItem);
        }

        #endregion
    }
}
