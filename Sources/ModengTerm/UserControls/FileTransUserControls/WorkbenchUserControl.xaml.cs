using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.FileTrans.Clients;
using ModengTerm.ViewModel;
using ModengTerm.ViewModel.FileTrans;
using System;
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

namespace ModengTerm.UserControls.FileTransUserControls
{
    /// <summary>
    /// SftpWorkbenchUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class WorkbenchUserControl : UserControl, ISessionContent
    {
        #region 实例变量

        private XTermSession sesison;
        private FsSessionVM fsSession;

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

        public WorkbenchUserControl()
        {
            InitializeComponent();
        }

        #endregion

        #region ISessionContent

        public int Open(OpenedSessionVM sessionVM)
        {
            this.fsSession = sessionVM as FsSessionVM;
            this.fsSession.Open();

            base.DataContext = this.fsSession;

            return ResponseCode.SUCCESS;
        }

        public void Close()
        {
            this.fsSession.Close();
            this.fsSession.Release();
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

        #region 事件处理器

        private void FileSystemTreeUserControl_EnterDirectory(FsTreeVM fsTreeVm, FsItemVM fsItemVm)
        {
            this.fsSession.LoadFsTreeAsync(fsTreeVm, fsItemVm.FullPath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcFsTree">从哪颗树上开始拖动</param>
        /// <param name="dstFsTree">要拖动到哪颗树上</param>
        /// <param name="srcFsItems">拖拽的文件列表</param>
        /// <param name="dstFsItem">要拖到哪个目录里，如果是空，则表示当前目录</param>
        private void FileSystemTreeUserControl_StartTransfer(FsTreeVM srcFsTree, FsTreeVM dstFsTree, List<FsItemVM> srcFsItems, FsItemVM dstFsItem)
        {
            this.fsSession.TransferFile(srcFsTree, dstFsTree, srcFsItems, dstFsItem);
        }

        #endregion
    }
}
