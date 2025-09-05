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

        /// <summary>
        /// 目标完整路径
        /// </summary>
        public string TargetFullPath { get; set; }

        /// <summary>
        /// 本地完整路径
        /// </summary>
        public string LocalFullPath { get; set; }

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
    }
}
