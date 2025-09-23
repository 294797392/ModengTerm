using log4net.Repository.Hierarchy;
using ModengTerm.Addon;
using ModengTerm.Base;
using ModengTerm.FileTrans.Enumerations;
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
using System.Text;
using System.Threading;
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
    public partial class FileListUserControl : UserControl, IDropHandler
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("FileListUserControl");

        #region 公开事件

        public event Action<FileListVM, FileListVM, List<FileItemVM>, string> TransferFile;

        #endregion

        #region 实例变量

        private ContextMenu fileListContextMenu;
        private FileListModes mode;

        #endregion

        #region 属性

        /// <summary>
        /// 获取FtpSession
        /// </summary>
        public FtpSessionVM FtpSession { get; set; }

        /// <summary>
        /// 设置或获取文件列表右键菜单
        /// </summary>
        public ContextMenu FileListContextMenu 
        {
            get
            {
                return this.fileListContextMenu;
            }
            set
            {
                if (this.fileListContextMenu != value)
                {
                    this.fileListContextMenu = value;
                    Setter setter = new Setter(DataGridRow.ContextMenuProperty, value);
                    Style style = DataGridFsList.RowStyle as Style;
                    style.Setters.Add(setter);
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
        }

        private void OnFileListDoubleClick()
        {
            FileListVM fsTree = base.DataContext as FileListVM;
            FileItemVM fsItem = fsTree.SelectedItem as FileItemVM;
            if (fsItem == null)
            {
                return;
            }

            switch (fsItem.Type)
            {
                case FsItemTypeEnum.ParentDirectory:
                case FsItemTypeEnum.Directory:
                    {
                        this.FtpSession.LoadFsTreeAsync(fsTree, fsItem.FullPath);
                        break;
                    }

                default:
                    break;
            }
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 进入编辑模式
        /// </summary>
        /// <param name="fileItem"></param>
        public void RenameItem(FileItemVM fileItem) 
        {
            switch (this.mode) 
            {

            }
        }

        #endregion

        #region 事件处理器

        #region DataGrid事件

        private void DataGridFsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = VisualTreeExtensions.GetVisualAncestor<TextBox>(e.OriginalSource as DependencyObject);
            if (textBox != null)
            {
                // 说明此时双击的是TextBox
                return;
            }

            this.OnFileListDoubleClick();
        }

        private void DataGridFsList_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            MouseButtonEventArgs mouseButtonEventArgs = e.EditingEventArgs as MouseButtonEventArgs;
            if (mouseButtonEventArgs.ClickCount == 2)
            {
                e.Cancel = true;
                return;
            }

            // 不能编辑“返回上级目录”项
            FileItemVM fsItem = e.Row.DataContext as FileItemVM;
            if (fsItem.Type == FsItemTypeEnum.ParentDirectory)
            {
                e.Cancel = true;
                return;
            }
        }

        private void DataGridFsList_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            ContentPresenter contentPresenter = e.EditingElement as ContentPresenter;
            TextBox textBox = contentPresenter.ContentTemplate.FindName("TextBoxEditName", contentPresenter) as TextBox;
            textBox.Focus();
            string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(textBox.Text);
            textBox.Select(0, fileNameWithoutExtension.Length);
        }

        private void DataGridFsList_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            switch (e.EditAction)
            {
                case DataGridEditAction.Commit:
                    {
                        FileItemVM fsItem = e.EditingElement.DataContext as FileItemVM;
                        string editName = fsItem.EditName;

                        if (string.IsNullOrEmpty(editName))
                        {
                            MTMessageBox.Info("请输入正确的文件名");
                            e.Cancel = true;
                            return;
                        }

                        char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
                        bool containsAnyInvalidChar = invalidChars.Any(x => editName.Contains(x));
                        if (containsAnyInvalidChar)
                        {
                            MTMessageBox.Info("请输入正确的文件名");
                            e.Cancel = true;
                            return;
                        }

                        FileListVM fsTree = base.DataContext as FileListVM;
                        this.FtpSession.CompleteRenameItem(fsTree, fsItem);

                        e.Cancel = false;

                        break;
                    }

                case DataGridEditAction.Cancel:
                    {
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        private void CheckBoxShowHiddenItems_CheckedChanged(object sender, RoutedEventArgs e)
        {
            FileListVM fsTree = DataGridFsList.DataContext as FileListVM;

            bool showHidden = CheckBoxToggleHiddenItem.IsChecked.Value;

            fsTree.ToggleHiddenItems(showHidden);
        }

        #endregion

        #region IDropHandler

        public void OnDragOver(DropInfo dropInfo)
        {
            FileListVM sourceFsTree = (dropInfo.DragInfo.VisualSource as DataGrid).DataContext as FileListVM;
            FileListVM targetFsTree = DataGridFsList.DataContext as FileListVM;

            // 拖放到哪个节点
            FileItemVM dropItem = dropInfo.TargetItem as FileItemVM;

            if (sourceFsTree == targetFsTree)
            {
                if (dropItem == null)
                {
                    dropInfo.Effects = DragDropEffects.None;
                    return;
                }

                if (dropItem.Type != FsItemTypeEnum.Directory)
                {
                    dropInfo.Effects = DragDropEffects.None;
                    return;
                }
            }
            else
            {
                if (dropItem == null)
                {
                    dropInfo.Effects = DragDropEffects.Copy;
                    return;
                }
            }

            if (dropInfo.Data is List<FileItemVM>)
            {
                List<FileItemVM> dragItems = dropInfo.Data as List<FileItemVM>;
                if (dropInfo == null || dragItems.Contains(dropItem))
                {
                    dropInfo.Effects = DragDropEffects.None;
                    return;
                }
            }
            else if (dropInfo.Data is FileItemVM)
            {
                if (dropItem == dropInfo.Data)
                {
                    dropInfo.Effects = DragDropEffects.None;
                    return;
                }
            }

            dropInfo.Effects = DragDropEffects.Copy;
        }

        public void OnDrop(DropInfo dropInfo)
        {
            FileListVM sourceFsTree = (dropInfo.DragInfo.VisualSource as DataGrid).DataContext as FileListVM;
            FileListVM targetFsTree = DataGridFsList.DataContext as FileListVM;

            List<FileItemVM> dragItems = dropInfo.Data as List<FileItemVM>;
            if (dragItems == null)
            {
                dragItems = new List<FileItemVM>();
                FileItemVM fsItemVm = dropInfo.Data as FileItemVM;
                dragItems.Add(fsItemVm);
            }

            string dstDir = string.Empty;

            FileItemVM dropItem = dropInfo.TargetItem as FileItemVM;
            if (dropItem == null)
            {
                // 拖到了DataGrid的空白处，说明是要传输到当前显示的目录里
                // 判断要拖到服务器还是客户端
                if (sourceFsTree == targetFsTree)
                {
                    return;
                }

                dstDir = targetFsTree.CurrentDirectory;
            }
            else
            {
                // 拖到了目录上，说明要传输到dropItem目录里
                dstDir = dropItem.FullPath;
            }

            this.TransferFile?.Invoke(sourceFsTree, targetFsTree, dragItems, dstDir);
        }

        #endregion
    }
}

