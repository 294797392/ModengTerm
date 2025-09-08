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

        public event Action<FsTreeVM, FsTreeVM, List<FsItemVM>, FsItemVM> TransferFile;
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

        #endregion

        #region 实例方法

        #endregion

        #region IDropHandler

        public void OnDragOver(DropInfo dropInfo)
        {
            FsItemVM targetItem = dropInfo.TargetItem as FsItemVM;
            if (targetItem == null)
            {
                dropInfo.Effects = DragDropEffects.Copy;
                return;
            }

            if (targetItem.Type != FsItemTypeEnum.Directory)
            {
                dropInfo.Effects = DragDropEffects.None;
                return;
            }

            if (dropInfo.Data is List<FsItemVM>)
            {
                List<FsItemVM> dragItems = dropInfo.Data as List<FsItemVM>;
                if (dropInfo == null || dragItems.Contains(targetItem))
                {
                    dropInfo.Effects = DragDropEffects.None;
                    return;
                }
            }
            else if (dropInfo.Data is FsItemVM)
            {
                if (targetItem == dropInfo.Data)
                {
                    dropInfo.Effects = DragDropEffects.None;
                    return;
                }
            }

            dropInfo.Effects = DragDropEffects.Copy;
        }

        public void OnDrop(DropInfo dropInfo)
        {
            List<FsItemVM> dragItems = dropInfo.Data as List<FsItemVM>;
            if (dragItems == null)
            {
                dragItems = new List<FsItemVM>();
                FsItemVM fsItemVm = dropInfo.Data as FsItemVM;
                dragItems.Add(fsItemVm);
            }

            FsItemVM dropItem = dropInfo.TargetItem as FsItemVM;
            if (dropItem == null)
            {
                // 拖到了DataGrid的空白处，说明是要传输到当前显示的目录里
                // 判断要拖到服务器还是客户端
            }
            else
            {
                // 拖到了目录上，说明要传输到dropItem目录里
            }

            FsTreeVM sourceFsTree = (dropInfo.DragInfo.VisualSource as DataGrid).DataContext as FsTreeVM;
            FsTreeVM targetFsTree = DataGridFsList.DataContext as FsTreeVM;
            this.TransferFile?.Invoke(sourceFsTree, targetFsTree, dragItems, dropItem);
        }

        #endregion
    }
}
