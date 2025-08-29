using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel.Terminal
{
    public class ProcessVM : ViewModelBase
    {
        private double cpuUsage;
        private double memUsage;
        private int pid;
        private string fullPath;


        public double CpuUsage
        {
            get { return cpuUsage; }
            set
            {
                if (cpuUsage != value)
                {
                    cpuUsage = value;
                    NotifyPropertyChanged("CpuUsage");
                }
            }
        }

        public double MemUsage
        {
            get { return memUsage; }
            set
            {
                if (memUsage != value)
                {
                    memUsage = value;
                    NotifyPropertyChanged("MemUsage");
                }
            }
        }

        public int PID
        {
            get { return pid; }
            set
            {
                if (pid != value)
                {
                    pid = value;
                    NotifyPropertyChanged("PID");
                }
            }
        }

        public string FullPath
        {
            get { return fullPath; }
            set
            {
                if (fullPath != value)
                {
                    fullPath = value;
                    NotifyPropertyChanged("FullPath");
                }
            }
        }

        public ProcessVM(Process process)
        {
            Update(process);
        }

        public void Update(Process process)
        {
            PID = process.Id;
            Name = process.ProcessName;
            //if (process.MainModule != null)
            //{
            //    this.FullPath = process.MainModule.FileName;
            //}
        }

        public bool CompareTo(Process process)
        {
            if (process.ProcessName != Name)
            {
                return false;
            }

            //if (process.MainModule != null)
            //{
            //    if (process.MainModule.FileName != this.FullPath)
            //    {
            //        return false;
            //    }
            //}

            return true;
        }
    }
}
