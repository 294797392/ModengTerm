using ModengTerm.Ftp.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel.Ftp
{
    public enum FsItemStates
    {
        /// <summary>
        /// 无状态
        /// </summary>
        None,

        /// <summary>
        /// 编辑节点名字状态
        /// </summary>
        EditName
    }

    public class FileItemVM : TreeNodeViewModel
    {
        private string fullPath;
        private FsItemTypeEnum type;
        private DateTime lastUpdateTime;
        private long size;
        private BitmapSource icon;
        private bool isHidden;
        private string editName;
        private FsItemStates state;

        /// <summary>
        /// 完整路径
        /// </summary>
        public string FullPath
        {
            get { return this.fullPath; }
            set
            {
                if (this.fullPath != value)
                {
                    this.fullPath = value;
                    this.NotifyPropertyChanged("FullPath");
                }
            }
        }

        /// <summary>
        /// 节点类型
        /// </summary>
        public FsItemTypeEnum Type
        {
            get { return this.type; }
            set
            {
                if (this.type != value)
                {
                    this.type = value;
                    this.NotifyPropertyChanged("Type");
                }
            }
        }

        /// <summary>
        /// 最后一次更新的时间
        /// </summary>
        public DateTime LastUpdateTime
        {
            get { return this.lastUpdateTime; }
            set
            {
                if (this.lastUpdateTime != value)
                {
                    this.lastUpdateTime = value;
                    this.NotifyPropertyChanged("LastUpdateTime");
                }
            }
        }

        /// <summary>
        /// 文件大小，单位是字节
        /// </summary>
        public long Size
        {
            get { return this.size; }
            set
            {
                if (this.size != value)
                {
                    this.size = value;
                }
            }
        }

        /// <summary>
        /// 文件图标
        /// </summary>
        public BitmapSource Icon
        {
            get { return this.icon; }
            set
            {
                if (this.icon != value)
                {
                    this.icon = value;
                    this.NotifyPropertyChanged("Icon");
                }
            }
        }

        /// <summary>
        /// 是否是隐藏文件或目录
        /// </summary>
        public bool IsHidden
        {
            get { return this.isHidden; }
            set
            {
                if (this.isHidden != value)
                {
                    this.isHidden = true;
                    this.NotifyPropertyChanged("IsHidden");
                }
            }
        }

        /// <summary>
        /// 编辑后的名称
        /// </summary>
        public string EditName
        {
            get { return this.editName; }
            set
            {
                if (this.editName != value)
                {
                    this.editName = value;
                    this.NotifyPropertyChanged("EditName");
                }
            }
        }

        /// <summary>
        /// 节点的当前状态
        /// </summary>
        public FsItemStates State
        {
            get { return this.state; }
            set
            {
                if (this.state != value)
                {
                    this.state = value;
                    this.NotifyPropertyChanged("State");
                }
            }
        }

        public FileItemVM(TreeViewModelContext context, object data = null) :
            base(context, data)
        {
        }
    }
}
