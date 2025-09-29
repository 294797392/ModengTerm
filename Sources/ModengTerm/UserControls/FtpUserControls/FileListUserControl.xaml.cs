using log4net.Repository.Hierarchy;
using ModengTerm.Addon;
using ModengTerm.Base;
using ModengTerm.FileTrans.Clients;
using ModengTerm.FileTrans.DataModels;
using ModengTerm.FileTrans.Enumerations;
using ModengTerm.UserControls.FtpUserControls.FileListUserControls;
using ModengTerm.ViewModel;
using ModengTerm.ViewModel.Ftp;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFToolkit.DragDrop;
using WPFToolkit.Utils;
using Xceed.Wpf.Toolkit.Core.Utilities;

namespace ModengTerm.UserControls.FtpUserControls
{
    /// <summary>
    /// 文件列表的显示方式
    /// </summary>
    public enum FileListModes
    {
        /// <summary>
        /// 列表模式
        /// </summary>
        List,
    }

    /// <summary>
    /// FileListUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class FileListUserControl : UserControl
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("FileListUserControl");

        #region 公开事件

        #endregion

        #region 实例变量

        private ContextMenu fileItemContextMenu;
        private ContextMenu fileListContextMenu;
        private FileListModes mode;
        private Dictionary<FileListModes, FrameworkElement> fileListViews;
        private IFileListView currentView;
        private FtpSessionVM ftpSession;
        private FileListVM fileList;
        private FileSystemTransport fileSystemTransport;
        private FtpRoleEnum ftpRole;

        #endregion

        #region 属性

        /// <summary>
        /// 获取FtpSession
        /// </summary>
        public FtpSessionVM FtpSession
        {
            get { return this.ftpSession; }
            set
            {
                if (this.ftpSession != value)
                {
                    this.ftpSession = value;
                }
            }
        }

        /// <summary>
        /// 设置或获取文件列表右键菜单
        /// </summary>
        public ContextMenu FileItemContextMenu
        {
            get
            {
                return this.fileItemContextMenu;
            }
            set
            {
                if (this.fileItemContextMenu != value)
                {
                    this.fileItemContextMenu = value;
                    if (this.currentView != null)
                    {
                        this.ApplyContextMenu(this.currentView, value);
                    }
                }
            }
        }

        public ContextMenu FileListContextMenu
        {
            get { return this.fileListContextMenu; }
            set
            {
                if (this.fileListContextMenu != value)
                {
                    this.fileListContextMenu = value;
                    ContentControlFileList.ContextMenu = value;
                }
            }
        }

        /// <summary>
        /// 获取该控件绑定的FsTreeVM
        /// </summary>
        public FileListVM FileListVM
        {
            get { return this.fileList; }
            set
            {
                if (this.fileList != value)
                {
                    this.fileList = value;
                    this.ftpRole = value.Context.Role;
                }
            }
        }

        public FileSystemTransport FileSystemTransport
        {
            get { return this.fileSystemTransport; }
            set
            {
                if (this.fileSystemTransport != value)
                {
                    this.fileSystemTransport = value;
                }
            }
        }

        #endregion

        #region 构造方法

        public FileListUserControl()
        {
            InitializeComponent();

            this.InitializeUserControl();
        }

        #endregion

        #region 实例方法

        private void InitializeUserControl()
        {
            this.fileListViews = new Dictionary<FileListModes, FrameworkElement>();
        }

        private void ApplyContextMenu(IFileListView fileListView, ContextMenu contextMenu)
        {
            Style style = fileListView.ItemContainerStyle;
            Setter setter = style.Setters.OfType<Setter>().FirstOrDefault(x => x.Property == FrameworkElement.ContextMenuProperty);
            if (setter != null)
            {
                style.Setters.Remove(setter);
                setter.Value = contextMenu;
                style.Setters.Add(setter);
            }
            else
            {
                setter = new Setter(DataGridRow.ContextMenuProperty, contextMenu);
                style.Setters.Add(setter);
            }
        }

        #endregion

        #region 公开接口

        public void SwitchMode(FileListModes mode)
        {
            FrameworkElement frameworkElement;
            if (!this.fileListViews.TryGetValue(mode, out frameworkElement))
            {
                switch (mode)
                {
                    case FileListModes.List: frameworkElement = new ListModeUserControl(); break;
                    default:
                        throw new NotImplementedException();
                }

                this.fileListViews[mode] = frameworkElement;

                IFileListView fileListView = frameworkElement as IFileListView;
                fileListView.FtpSession = this.ftpSession;
                this.ApplyContextMenu(fileListView, this.fileItemContextMenu);
            }

            ContentControlFileList.Content = frameworkElement;
            this.currentView = frameworkElement as IFileListView;

            this.mode = mode;
        }

        /// <summary>
        /// 进入编辑模式
        /// </summary>
        /// <param name="fileItem"></param>
        public void BeginRename(FileItemVM fileItem)
        {
            this.currentView.BeginRename(fileItem);
        }

        public void EndRename(FileItemVM fileItem)
        {
            this.currentView.EndRename(fileItem);
        }

        #endregion

        #region 事件处理器

        private void CheckBoxShowHiddenItems_CheckedChanged(object sender, RoutedEventArgs e)
        {
            bool showHiddenItems = CheckBoxToggleHiddenItem.IsChecked.Value;

            this.FileListVM.ToggleHiddenItems(showHiddenItems);
        }

        /// <summary>
        /// 显示预览目录Popup列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonShowPreviewDirectories_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;
            DirectoryVM toPreview = frameworkElement.DataContext as DirectoryVM;
            AddressbarVM adrb = this.fileList.Addressbar;

            Task.Factory.StartNew(() =>
            {
                List<FsItemInfo> fsItems = null;
                try
                {
                    if (toPreview.ID == VTBaseConsts.RootDirectoryChainId)
                    {
                        fsItems = this.fileSystemTransport.ListRootItems();
                    }
                    else
                    {
                        fsItems = this.fileSystemTransport.ListItems(toPreview.FullPath);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("加载子目录列表异常", ex);
                    return;
                }

                List<DirectoryVM> previewDirs = new List<DirectoryVM>();

                foreach (FsItemInfo fsItem in fsItems)
                {
                    if (fsItem.Type != FsItemTypeEnum.Directory)
                    {
                        continue;
                    }

                    DirectoryVM previewDir = new DirectoryVM()
                    {
                        ID = fsItem.FullPath,
                        Name = fsItem.Name,
                        FullPath = fsItem.FullPath,
                        IsVisible = !fsItem.IsHidden
                    };
                    previewDirs.Add(previewDir);
                }

                base.Dispatcher.Invoke(() =>
                {
                    adrb.PreviewDirectories.Clear();
                    adrb.PreviewDirectories.AddRange(previewDirs);

                    if (ListBoxPreviewDirectory.ItemsSource != adrb.PreviewDirectories)
                    {
                        ListBoxPreviewDirectory.ItemsSource = adrb.PreviewDirectories;
                    }
                    if (PopupPreviewDirectory.PlacementTarget != frameworkElement)
                    {
                        PopupPreviewDirectory.PlacementTarget = frameworkElement;
                    }
                    PopupPreviewDirectory.IsOpen = true;
                    PopupPreviewDirectory.Tag = toPreview;
                });
            });
        }

        private void ButtonJumpDirectory_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;
            DirectoryVM toJump = frameworkElement.DataContext as DirectoryVM;
            AddressbarVM adrb = this.fileList.Addressbar;

            if (string.IsNullOrEmpty(toJump.FullPath))
            {
                return;
            }

            this.ftpSession.LoadFileListAsync(this.fileList, toJump.FullPath, () => 
            {
                DirectoryVM endRemove = PopupPreviewDirectory.Tag as DirectoryVM;
                while (adrb.DirectroyChain[adrb.DirectroyChain.Count - 1] != endRemove)
                {
                    adrb.DirectroyChain.RemoveAt(adrb.DirectroyChain.Count - 1);
                }
            });
        }

        private void ListBoxPreviewDirectory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 在预览列表里选中的要跳转到的目录
            DirectoryVM selectedDirectory = ListBoxPreviewDirectory.SelectedItem as DirectoryVM;
            if (selectedDirectory == null)
            {
                return;
            }

            this.ftpSession.LoadFileListAsync(this.fileList, selectedDirectory.FullPath, () => 
            {
                AddressbarVM adrb = this.fileList.Addressbar;

                DirectoryVM endRemove = PopupPreviewDirectory.Tag as DirectoryVM;
                while (adrb.DirectroyChain[adrb.DirectroyChain.Count - 1] != endRemove)
                {
                    adrb.DirectroyChain.RemoveAt(adrb.DirectroyChain.Count - 1);
                }

                adrb.DirectroyChain.Add(selectedDirectory);

                PopupPreviewDirectory.IsOpen = false;
            });
        }

        #endregion
    }
}

