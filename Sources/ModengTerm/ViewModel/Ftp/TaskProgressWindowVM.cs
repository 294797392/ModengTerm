using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel.Ftp
{
    public class TaskProgressWindowVM : ViewModelBase
    {
        private double progress;
        private string filePath;
        private string operation;

        /// <summary>
        /// 操作进度
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
        /// 当前正在操作的文件路径
        /// </summary>
        public string FilePath
        {
            get { return this.filePath; }
            set
            {
                if (this.filePath != value)
                {
                    this.filePath = value;
                    this.NotifyPropertyChanged("FilePath");
                }
            }
        }

        /// <summary>
        /// 当前正在执行的操作
        /// </summary>
        public string Operation
        {
            get { return this.operation; }
            set
            {
                if (this.operation != value)
                {
                    this.operation = value;
                    this.NotifyPropertyChanged("Operation");
                }
            }
        }
    }
}
