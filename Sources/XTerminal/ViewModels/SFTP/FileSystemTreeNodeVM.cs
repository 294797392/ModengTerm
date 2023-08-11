using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace XTerminal.ViewModels.SFTP
{
    public abstract class FileSystemTreeNodeVM : TreeNodeViewModel
    {
        private long size;
        private bool isHidden;
        private DateTime lastUpdateTime;
        private string accessProperty;

        /// <summary>
        /// 当前文件/目录的完整路径
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// 文件大小，字节为单位
        /// </summary>
        public long Size 
        {
            get { return this.size; }
            set
            {
                if (this.size != value)
                {
                    this.size = value;
                    this.NotifyPropertyChanged("Size");
                }
            }
        }

        /// <summary>
        /// 是否是隐藏文件
        /// </summary>
        public bool IsHidden
        {
            get { return this.isHidden; }
            set
            {
                if (this.isHidden != value)
                {
                    this.isHidden = value;
                    this.NotifyPropertyChanged("IsHidden");
                }
            }
        }

        /// <summary>
        /// 修改时间
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
        /// 文件权限属性
        /// </summary>
        public string AccessProperty
        {
            get { return this.accessProperty; }
            set 
            {
                if(this.accessProperty != value) 
                {
                    this.accessProperty = value;
                    this.NotifyPropertyChanged("AccessProperty");
                }
            }
        }

        /// <summary>
        /// 文件节点类型
        /// </summary>
        public abstract FileSystemNodeTypeEnum Type { get; }

        public FileSystemTreeNodeVM(TreeViewModelContext context, object data = null) : 
            base(context, data)
        {
        }
    }
}
