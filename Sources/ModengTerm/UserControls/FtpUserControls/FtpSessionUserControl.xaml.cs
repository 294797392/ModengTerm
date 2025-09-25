using DotNEToolkit.Packaging;
using log4net.Repository.Hierarchy;
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
using System.Runtime.InteropServices;
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
using static DotNEToolkit.Shell32;

namespace ModengTerm.UserControls.FtpUserControls
{
    /// <summary>
    /// FtpSessionUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class FtpSessionUserControl : UserControl, ISessionContent
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("FtpSessionUserControl");

        #endregion

        #region 实例变量

        private XTermSession sesison;
        private FtpSessionVM ftpSession;
        private FileListVM clientFileList;
        private FileListVM serverFileList;

        private FileListVM editFileListVM;
        private FileItemVM editFileItem; // 当前正在编辑的项
        private FileListUserControl editFileList; // 当前正在编辑的文件列表

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
            Client.RegisterCommand(FtpCommandKeys.CLIENT_SHOW_ITEM_PROPERTY, OnFtpShowItemProperty, FtpRoleEnum.Client);
            Client.RegisterCommand(FtpCommandKeys.CLIENT_REFRESH_ITEMS, OnFtpRefreshItems, FtpRoleEnum.Client);

            Client.RegisterCommand(FtpCommandKeys.SERVER_OPEN_ITEM, OnFtpOpenServerItem);
            Client.RegisterCommand(FtpCommandKeys.SERVER_REFRESH_ITEMS, OnFtpRefreshItems, FtpRoleEnum.Server);
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

            this.clientFileList = ftpSession.ClientFsTree;
            this.serverFileList = ftpSession.ServerFsTree;

            FileListUserControlClient.FtpSession = ftpSession;
            FileListUserControlClient.ItemContextMenu = this.CreateFileItemContextMenu(clientFileList.ContextMenus);
            FileListUserControlClient.ItemContextMenu.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(this.FileListMenuItem_Click));
            FileListUserControlClient.SwitchMode(FileListModes.List);
            FileListUserControlServer.FtpSession = ftpSession;
            FileListUserControlServer.ItemContextMenu = this.CreateFileItemContextMenu(serverFileList.ContextMenus);
            FileListUserControlServer.ItemContextMenu.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(this.FileListMenuItem_Click));
            FileListUserControlServer.SwitchMode(FileListModes.List);

            base.DataContext = ftpSession;

            this.ftpSession = ftpSession;

            return ResponseCode.SUCCESS;
        }

        public void Close()
        {
            FileListUserControlClient.FtpSession = null;
            FileListUserControlClient.ItemContextMenu.RemoveHandler(MenuItem.ClickEvent, this.FileListMenuItem_Click);
            FileListUserControlServer.FtpSession = null;
            FileListUserControlServer.ItemContextMenu.RemoveHandler(MenuItem.ClickEvent, this.FileListMenuItem_Click);

            base.DataContext = null;

            this.ftpSession.Close();
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

        private ContextMenu CreateFileItemContextMenu(IEnumerable itemsSource)
        {
            ContextMenu ctxMenu = new ContextMenu()
            {
                ItemsSource = itemsSource,
                ItemTemplate = this.FindResource("HierarchicalDataTemplateFileList") as HierarchicalDataTemplate,
                Style = App.Current.FindResource("StyleContextMenu") as Style
            };

            return ctxMenu;
        }

        /// <summary>
        /// 当前选中的项目进入编辑模式
        /// </summary>
        /// <param name="ftpRole"></param>
        private void BeginRename(FtpRoleEnum ftpRole)
        {
            FileListUserControl fileListUserControl = null;
            FileListVM fileListVM = null;

            if (ftpRole == FtpRoleEnum.Client)
            {
                fileListVM = ftpSession.ClientFsTree;
                fileListUserControl = FileListUserControlClient;
            }
            else
            {
                fileListVM = ftpSession.ServerFsTree;
                fileListUserControl = FileListUserControlServer;
            }

            FileItemVM fileItem = fileListVM.SelectedItem as FileItemVM;
            if (fileItem == null)
            {
                return;
            }

            fileItem.EditName = fileItem.Name;
            fileItem.State = FsItemStates.EditName;
            fileListUserControl.BeginRename(fileItem);
            this.editFileItem = fileItem;
            this.editFileList = fileListUserControl;
            this.editFileListVM = fileListVM;
        }

        #endregion

        #region 事件处理器

        private void FileListMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItemVM menuItemVm = (e.OriginalSource as FrameworkElement).DataContext as MenuItemVM;
            ClientUtils.DispatchCommand(menuItemVm);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (this.editFileItem != null)
            {
                switch (e.Key)
                {
                    default:
                        {
                            return;
                        }

                    case Key.Enter:
                        {
                            // 提交编辑
                            this.ftpSession.OnCommitRename(this.editFileListVM, this.editFileItem);
                            this.editFileList.EndRename(this.editFileItem);
                            break;
                        }

                    case Key.Escape:
                        {
                            // 取消编辑
                            this.editFileList.EndRename(this.editFileItem);
                            break;
                        }
                }

                this.editFileItem = null;
                this.editFileList = null;
                this.editFileListVM = null;
                e.Handled = true;
            }
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
            ftpSessionUserControl.BeginRename(ftpRole);
        }

        private static void OnFtpShowItemProperty(CommandArgs e)
        {
            FtpRoleEnum ftpRole = (FtpRoleEnum)e.UserData;
            FtpSessionVM ftpSession = e.ActiveTab as FtpSessionVM;

            FileItemVM fileItem = ftpSession.ClientFsTree.SelectedItem as FileItemVM;
            if (fileItem == null)
            {
                return;
            }

            SHELLEXECUTEINFO info = new SHELLEXECUTEINFO();
            info.cbSize = Marshal.SizeOf(info);
            info.lpVerb = "properties"; // 关键：指定动作为 "properties"
            info.lpFile = fileItem.FullPath;
            info.nShow = SW_SHOW;
            info.fMask = SEE_MASK_INVOKEIDLIST;

            bool result = ShellExecuteEx(ref info);
            if (!result)
            {
                int error = Marshal.GetLastWin32Error();
                MTMessageBox.Info("打开属性对话框失败, {0}", error);
                logger.ErrorFormat("打开属性对话框失败, {0}, {1}", error, fileItem.FullPath);
            }
        }

        private static void OnFtpRefreshItems(CommandArgs e)
        {
            FtpRoleEnum ftpRole = (FtpRoleEnum)e.UserData;
            FtpSessionVM ftpSession = e.ActiveTab as FtpSessionVM;
            ftpSession.FtpRefreshItems(ftpRole);
        }


        private static void OnFtpOpenServerItem(CommandArgs e)
        {
            (e.ActiveTab as FtpSessionVM).FtpOpenServerItem();
        }

        #endregion
    }
}
