using ModengTerm.FileTrans;
using ModengTerm.FileTrans.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel.FileTrans
{
    /// <summary>
    /// 文件上传状态ViewModel
    /// </summary>
    public class FileStatusVM : ItemViewModel
    {
        private double progress;
        private string message;
        private string speed;
        private ProcessStates state;

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
        public FsItemTypeEnum Type { get; set; }

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

        public FileStatusVM()
        {
            this.PrevTransferTime = DateTime.MinValue;
        }
    }
}
