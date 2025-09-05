using ModengTerm.FileTrans.DataModels;
using ModengTerm.FileTrans.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel.FileTrans
{
    public class FsItemVM : TreeNodeViewModel
    {
        private string fullPath;
        private FsItemTypeEnum type;
        private DateTime lastUpdateTime;
        private long size;
        private BitmapSource icon;

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

        public FsItemVM(TreeViewModelContext context, object data = null) :
            base(context, data)
        {
        }
    }
}
