using DotNEToolkit.Packaging;
using ModengTerm.Ftp.Enumerations;
using ModengTerm.ViewModel;
using ModengTerm.ViewModel.Ftp;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
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

namespace ModengTerm.UserControls.FtpUserControls.FileListUserControls
{
    /// <summary>
    /// ListModeUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class ListModeUserControl : UserControl, IFileListView
    {
        #region 公开事件

        #endregion

        #region 实例变量

        #endregion

        #region 属性

        public FtpSessionVM FtpSession { get; set; }

        public Style ItemContainerStyle { get { return DataGridFsList.RowStyle; } }

        private FileListVM FileListVM { get { return base.DataContext as FileListVM; } }

        #endregion

        #region 构造方法

        public ListModeUserControl()
        {
            InitializeComponent();
        }

        #endregion

        #region 实例方法

        private TextBox GetEditTextBox(FileItemVM fileItem)
        {
            DataGridRow dataGridRow = DataGridFsList.ItemContainerGenerator.ContainerFromItem(fileItem) as DataGridRow;
            FrameworkElement frameworkElement = DataGridTemplateColumnName.GetCellContent(dataGridRow);
            ContentPresenter contentPresenter = frameworkElement as ContentPresenter;
            return contentPresenter.ContentTemplate.FindName("TextBoxEditName", contentPresenter) as TextBox;
        }

        #endregion

        #region 事件处理器

        private void DataGridFsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = VisualTreeExtensions.GetVisualAncestor<TextBox>(e.OriginalSource as DependencyObject);
            if (textBox != null)
            {
                // 说明此时双击的是TextBox
                return;
            }

            this.FtpSession.OnDoubleClickFileItem(this.FileListVM, DataGridFsList.SelectedItem as FileItemVM);
        }

        /// <summary>
        /// 当TextBox失去焦点的时候隐藏编辑并且结束编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            FileItemVM fileItem = textBox.DataContext as FileItemVM;
            this.EndRename(fileItem);
            // 提交重命名
            this.FtpSession.OnCommitRename(this.FileListVM, fileItem);
        }

        /// <summary>
        /// 阻止输入非法文件名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();

            foreach (char c in e.Text)
            {
                if (invalidChars.Contains(c))
                {
                    e.Handled = true;
                    break;
                }
            }
        }

        #endregion

        #region IFileListView

        public void BeginRename(FileItemVM fileItem)
        {
            TextBox textBox = this.GetEditTextBox(fileItem);
            textBox.Visibility = Visibility.Visible;
            textBox.LostFocus += this.TextBox_LostFocus;
            textBox.PreviewTextInput += this.TextBox_PreviewTextInput;
            textBox.Focus();
            textBox.SelectAll();
        }

        public void EndRename(FileItemVM fileItem)
        {
            TextBox textBox = this.GetEditTextBox(fileItem);
            textBox.LostFocus -= this.TextBox_LostFocus;
            textBox.PreviewTextInput -= this.TextBox_PreviewTextInput;
            textBox.Visibility = Visibility.Collapsed;
            fileItem.State = FsItemStates.None;
        }

        #endregion

        #region IDragHandler

        public void StartDrag(DragInfo dragInfo)
        {
            FileListVM sourceFileList = (dragInfo.VisualSource as FrameworkElement).DataContext as FileListVM;

            // 如果拖拽的项里包含“上级目录”，那么不允许拖拽
            IEnumerable<FileItemVM> dragItems = dragInfo.SourceItems.Cast<FileItemVM>();
            if (dragItems.FirstOrDefault(v => v.Type == FsItemTypeEnum.ParentDirectory) != null)
            {
                dragInfo.Effects = DragDropEffects.None;
                return;
            }

            dragInfo.Data = dragItems;
            dragInfo.Effects = DragDropEffects.Copy;
        }

        #endregion

        #region IDropHandler

        public void OnDragOver(DropInfo dropInfo)
        {
            FileListVM sourceFileList = (dropInfo.DragInfo.VisualSource as FrameworkElement).DataContext as FileListVM;
            FileListVM targetFileList = (dropInfo.VisualTarget as FrameworkElement).DataContext as FileListVM;

            // 拖放到哪个节点
            FileItemVM dropItem = dropInfo.TargetItem as FileItemVM;

            if (sourceFileList == targetFileList)
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

            IEnumerable<FileItemVM> dragItems = dropInfo.Data as IEnumerable<FileItemVM>;
            if (dropInfo == null || dragItems.Contains(dropItem))
            {
                dropInfo.Effects = DragDropEffects.None;
                return;
            }

            dropInfo.Effects = DragDropEffects.Copy;
        }

        public void OnDrop(DropInfo dropInfo)
        {
            FileListVM sourceFileList = (dropInfo.DragInfo.VisualSource as FrameworkElement).DataContext as FileListVM;
            FileListVM targetFileList = (dropInfo.VisualTarget as FrameworkElement).DataContext as FileListVM;

            string targetDirectory = string.Empty;

            FileItemVM dropItem = dropInfo.TargetItem as FileItemVM;
            if (dropItem == null)
            {
                // 拖到了DataGrid的空白处，说明是要传输到当前显示的目录里
                // 判断要拖到服务器还是客户端
                if (sourceFileList == targetFileList)
                {
                    return;
                }

                targetDirectory = targetFileList.CurrentDirectory;
            }
            else
            {
                // 拖到了项目上
                switch (dropItem.Type)
                {
                    case FsItemTypeEnum.File:
                        {
                            // 拖到了文件上，说明要上传到当前目录
                            targetDirectory = targetFileList.CurrentDirectory;
                            break;
                        }

                    default:
                        {
                            // 拖到了文件夹上，说明要上传到文件夹里
                            targetDirectory = dropItem.FullPath;
                            break;
                        }
                }
            }

            IEnumerable<FileItemVM> dragItems = dropInfo.Data as IEnumerable<FileItemVM>;
            this.FtpSession.TransferFile(sourceFileList, targetFileList, dragItems.ToList(), targetDirectory);
        }

        #endregion
    }
}
