using ModengTerm.FileTrans;
using ModengTerm.FileTrans.Enumerations;
using ModengTerm.Ftp.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel.Ftp
{
    /// <summary>
    /// 文件上传列表是一个常规的文件列表树形结构
    /// 考虑到在上传文件的时候，有可能该文件所在的目录没有创建，所以得先创建目录
    /// 子任务必须等父任务结束运行之后再运行
    /// </summary>
    public class TaskTreeVM : TreeViewModel<TreeViewModelContext>
    {

    }

    /// <summary>
    /// 文件上传状态ViewModel
    /// </summary>
    public class TaskTreeNodeVM : TreeNodeViewModel
    {
        private double progress;
        private string message;
        private string speed;
        private ProcessStates state;
        private BitmapSource icon;

        /// <summary>
        /// 目标完整路径
        /// </summary>
        public string TargetFullPath { get; set; }

        /// <summary>
        /// 本地完整路径
        /// </summary>
        public string SourceFullPath { get; set; }

        /// <summary>
        /// 上传进度
        /// </summary>
        public double Progress
        {
            get { return this.progress; }
            set
            {
                if (this.progress != value)
                {
                    this.progress = value;
                    this.NotifyPropertyChanged("Progress");
                }
            }
        }

        /// <summary>
        /// 文件类型
        /// </summary>
        public FsItemTypeEnum SourceItemType { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public FsOperationTypeEnum OpType { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message
        {
            get { return this.message; }
            set
            {
                if (this.message != value)
                {
                    this.message = value;
                    this.NotifyPropertyChanged("Message");
                }
            }
        }

        /// <summary>
        /// 上次传输时间
        /// </summary>
        public DateTime PrevTransferTime { get; set; }

        /// <summary>
        /// 实时传输速度
        /// </summary>
        public string Speed
        {
            get { return this.speed; }
            set
            {
                if (this.speed != value)
                {
                    this.speed = value;
                    this.NotifyPropertyChanged("Speed");
                }
            }
        }

        /// <summary>
        /// 文件上传状态
        /// </summary>
        public ProcessStates State
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

        public TaskTreeNodeVM(TreeViewModelContext context) :
            base(context)
        {
            this.PrevTransferTime = DateTime.MinValue;
        }
    }
}
