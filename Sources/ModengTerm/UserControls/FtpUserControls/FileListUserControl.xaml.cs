using log4net.Repository.Hierarchy;
using ModengTerm.FileTrans.Enumerations;
using ModengTerm.ViewModel.FileTrans;
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
using WPFToolkit.DragDrop;

namespace ModengTerm.UserControls.FileTransUserControls
{
    /// <summary>
    /// FileSystemTreeUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class FileSystemTreeUserControl : UserControl, IDropHandler
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("FileSystemTreeUserControl");

        #region 公开事件

        public event Action<FsTreeVM, FsTreeVM, List<FsItemVM>, string> TransferFile;
        public event Action<FsTreeVM, FsItemVM> EnterDirectory;

        #endregion

        #region 构造方法

        public FileSystemTreeUserControl()
        {
            InitializeComponent();
        }

        #endregion

        #region 事件处理器

        private void DataGridFsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FsItemVM fsItemVM = DataGridFsList.SelectedItem as FsItemVM;
            if (fsItemVM == null)
            {
                return;
            }

            switch (fsItemVM.Type)
            {
                case FsItemTypeEnum.Directory:
                    {
                        this.EnterDirectory?.Invoke(DataGridFsList.DataContext as FsTreeVM, fsItemVM);
                        break;
                    }

                default:
                    break;
            }
        }

        private void CheckBoxShowHiddenItems_CheckedChanged(object sender, RoutedEventArgs e)
        {
            FsTreeVM fsTree = DataGridFsList.DataContext as FsTreeVM;

            bool showHidden = CheckBoxToggleHiddenItem.IsChecked.Value;

            fsTree.ToggleHiddenItems(showHidden);
        }

        #endregion

        #region 实例方法

        #endregion

        #region IDropHandler

        public void OnDragOver(DropInfo dropInfo)
        {
            FsTreeVM sourceFsTree = (dropInfo.DragInfo.VisualSource as DataGrid).DataContext as FsTreeVM;
            FsTreeVM targetFsTree = DataGridFsList.DataContext as FsTreeVM;

            // 拖放到哪个节点
            FsItemVM dropItem = dropInfo.TargetItem as FsItemVM;
            if (dropItem == null)
            {
                dropInfo.Effects = DragDropEffects.Copy;
                return;
            }

            if (sourceFsTree == targetFsTree)
            {
                if (dropItem.Type != FsItemTypeEnum.Directory)
                {
                    dropInfo.Effects = DragDropEffects.None;
                    return;
                }
            }
            else
            {
            }

            if (dropInfo.Data is List<FsItemVM>)
            {
                List<FsItemVM> dragItems = dropInfo.Data as List<FsItemVM>;
                if (dropInfo == null || dragItems.Contains(dropItem))
                {
                    dropInfo.Effects = DragDropEffects.None;
                    return;
                }
            }
            else if (dropInfo.Data is FsItemVM)
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
            FsTreeVM sourceFsTree = (dropInfo.DragInfo.VisualSource as DataGrid).DataContext as FsTreeVM;
            FsTreeVM targetFsTree = DataGridFsList.DataContext as FsTreeVM;

            List<FsItemVM> dragItems = dropInfo.Data as List<FsItemVM>;
            if (dragItems == null)
            {
                dragItems = new List<FsItemVM>();
                FsItemVM fsItemVm = dropInfo.Data as FsItemVM;
                dragItems.Add(fsItemVm);
            }

            string dstDir = string.Empty;

            FsItemVM dropItem = dropInfo.TargetItem as FsItemVM;
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
