using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels.Terminals
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
                if (this.cpuUsage != value)
                {
                    this.cpuUsage = value;
                    this.NotifyPropertyChanged("CpuUsage");
                }
            }
        }

        public double MemUsage
        {
            get { return memUsage; }
            set
            {
                if (this.memUsage != value)
                {
                    this.memUsage = value;
                    this.NotifyPropertyChanged("MemUsage");
                }
            }
        }

        public int PID
        {
            get { return pid; }
            set
            {
                if (this.pid != value)
                {
                    this.pid = value;
                    this.NotifyPropertyChanged("PID");
                }
            }
        }

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

        public ProcessVM(Process process)
        {
            this.Update(process);
        }

        public void Update(Process process)
        {
            this.PID = process.Id;
            this.Name = process.ProcessName;
            //if (process.MainModule != null)
            //{
            //    this.FullPath = process.MainModule.FileName;
            //}
        }

        public bool CompareTo(Process process)
        {
            if (process.ProcessName != this.Name)
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
