using log4net.Repository.Hierarchy;
using ModengTerm.Addon;
using ModengTerm.Base;
using ModengTerm.FileTrans.Enumerations;
using ModengTerm.UserControls.FtpUserControls.FileListUserControls;
using ModengTerm.ViewModel;
using ModengTerm.ViewModel.Ftp;
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

        private ContextMenu itemContextMenu;
        private FileListModes mode;
        private Dictionary<FileListModes, FrameworkElement> fileListViews;
        private IFileListView currentView;

        #endregion

        #region 属性

        /// <summary>
        /// 获取FtpSession
        /// </summary>
        public FtpSessionVM FtpSession { get; set; }

        /// <summary>
        /// 设置或获取文件列表右键菜单
        /// </summary>
        public ContextMenu ItemContextMenu
        {
            get
            {
                return this.itemContextMenu;
            }
            set
            {
                if (this.itemContextMenu != value)
                {
                    this.itemContextMenu = value;
                    if (this.currentView != null)
                    {
                        this.ApplyContextMenu(this.currentView, value);
                    }
                }
            }
        }

        /// <summary>
        /// 获取该控件绑定的FsTreeVM
        /// </summary>
        public FileListVM FileListVM { get { return base.DataContext as FileListVM; } }

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
                fileListView.FtpSession = this.FtpSession;
                this.ApplyContextMenu(fileListView, this.itemContextMenu);
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

        #endregion
    }
}

