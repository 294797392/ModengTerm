using ModengTerm.FileTrans.Clients;
using ModengTerm.Styles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel.Ftp
{
    public class FileListContext : TreeViewModelContext
    {
        /// <summary>
        /// 获取是服务器树形列表还是客户端树形列表
        /// </summary>
        public FtpRoleEnum Role { get; set; }
    }

    public class FileListVM : TreeViewModel<FileListContext>
    {
        #region 实例变量

        private string currentDirectory;
        private int totalHiddens;

        #endregion

        #region 属性

        /// <summary>
        /// 当前显示的目录
        /// </summary>
        public string CurrentDirectory
        {
            get { return this.currentDirectory; }
            set
            {
                if (this.currentDirectory != value)
                {
                    this.currentDirectory = value;
                    this.NotifyPropertyChanged("CurrentDirectory");
                }
            }
        }

        /// <summary>
        /// 一共有多少个隐藏文件
        /// </summary>
        public int TotalHiddens
        {
            get { return this.totalHiddens; }
            set
            {
                if (this.totalHiddens != value)
                {
                    this.totalHiddens = value;
                    this.NotifyPropertyChanged("TotalHiddens");
                }
            }
        }

        /// <summary>
        /// 树形列表的右键菜单
        /// </summary>
        public BindableCollection<MenuItemVM> ContextMenus { get; private set; }

        public BindableCollection<MenuItemVM> FileListContextMenus { get; private set; }

        /// <summary>
        /// 地址栏ViewModel
        /// </summary>
        public AddressbarVM Addressbar { get; private set; }

        #endregion

        #region 构造方法

        public FileListVM() 
        {
            this.ContextMenus = new BindableCollection<MenuItemVM>();
            this.FileListContextMenus = new BindableCollection<MenuItemVM>();
            this.Addressbar = new AddressbarVM();
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 切换隐藏目录的显示状态
        /// </summary>
        /// <param name="isShow">是否显示隐藏目录</param>
        public void ToggleHiddenItems(bool isShow)
        {
            IEnumerable<FileItemVM> hiddenItems = this.GetAllNodes<FileItemVM>().Where(v => v.IsHidden);

            foreach (FileItemVM hiddenItem in hiddenItems)
            {
                hiddenItem.IsVisible = isShow;
            }
        }

        #endregion
    }
}
