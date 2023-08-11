using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace XTerminal.ViewModels.SFTP
{
    /// <summary>
    /// 存储一个正在传输的文件状态
    /// </summary>
    public class TransferFileVM : ItemViewModel
    {
        private TransferTypeEnum type;
        private TransferStatusEnum status;
        private double progress;
        private long size;
        private string localPath;
        private string remotePath;
        private double speed;

        public TransferTypeEnum Type
        {
            get { return this.type; }
            set
            {
                if(this.type != value)
                {
                    this.type = value;
                    this.NotifyPropertyChanged("Type");
                }
            }
        }

        public TransferStatusEnum Status
        {
            get { return this.status; }
            set
            {
                if(this.status != value)
                {
                    this.status = value;
                    this.NotifyPropertyChanged("Status");
                }
            }
        }

        /// <summary>
        /// 传输进度
        /// </summary>
        public double Progress
        {
            get { return this.progress; }
            set
            {
                if(this.progress != value)
                {
                    this.progress = value;
                    this.NotifyPropertyChanged("Progress");
                }
            }
        }

        /// <summary>
        /// 传输的文件大小
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
        /// 本地文件路径
        /// </summary>
        public string LocalPath
        {
            get { return this.localPath; }
            set
            {
                if (this.localPath != value)
                {
                    this.localPath = value;
                    this.NotifyPropertyChanged("LocalPath");
                }
            }
        }

        /// <summary>
        /// 远程路径
        /// </summary>
        public string RemotePath
        {
            get { return this.remotePath; }
            set
            {
                if (this.remotePath != value)
                {
                    this.remotePath = value;
                    this.NotifyPropertyChanged("RemotePath");
                }
            }
        }

        /// <summary>
        /// 传输速度
        /// </summary>
        public double Speed
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
    }
}
