using ModengTerm.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.OfficialAddons.PerfMon
{
    public class CpuMemVM : ViewModelBase
    {
        private double cpuPercent;
        private double memoryPercent;
        private string displayMemoryUsage;

        /// <summary>
        /// CPU占用百分比
        /// 计算公式：
        /// （CPU内核时间 + CPU用户时间） / （CPU内核时间 + CPU用户时间 + CPU空闲时间）
        /// 参考：
        /// https://blog.csdn.net/zxf347085420/article/details/137209188
        /// </summary>
        public double CpuPercent
        {
            get { return this.cpuPercent; }
            set
            {
                if (this.cpuPercent != value)
                {
                    this.cpuPercent = value;
                    this.NotifyPropertyChanged("CpuPercent");
                }
            }
        }

        /// <summary>
        /// 内存占用百分比
        /// </summary>
        public double MemoryPercent
        {
            get { return this.memoryPercent; }
            set
            {
                if (this.memoryPercent != value)
                {
                    this.memoryPercent = value;
                    this.NotifyPropertyChanged("MemoryPercent");
                }
            }
        }

        public string DisplayMemoryUsage
        {
            get { return this.displayMemoryUsage; }
            set
            {
                if ((this.displayMemoryUsage != value))
                {
                    this.displayMemoryUsage = value;
                    this.NotifyPropertyChanged("DisplayMemoryUsage");
                }
            }
        }
    }
}
