using ModengTerm.Base;
using System;
using System.ComponentModel;
using WPFToolkit.MVVM;

namespace ModengTerm.Addon.Official.SystemMonitor
{
    public class ProcessVM : ItemViewModel
    {
        private int pid;
        private UnitValueDouble memory;
        private string displayMemory;
        private long totalProcessorTime;
        private string displayCpuUsage;
        private double cpuUsage;

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

        public UnitValueDouble Memory
        {
            get { return memory; }
            set
            {
                if (memory != value)
                {
                    memory = value;
                    NotifyPropertyChanged("Memory");
                }
            }
        }

        public string DisplayMemory
        {
            get { return displayMemory; }
            set
            {
                if (displayMemory != value)
                {
                    displayMemory = value;
                    NotifyPropertyChanged("DisplayMemory");
                }
            }
        }

        public long TotalProcessorTime
        {
            get { return totalProcessorTime; }
            set
            {
                if (totalProcessorTime != value)
                {
                    totalProcessorTime = value;
                }
            }
        }

        public double CpuUsage
        {
            get { return cpuUsage; }
            set
            {
                if (cpuUsage != value)
                {
                    cpuUsage = value;
                }
            }
        }

        public ProcessVM()
        {
            Memory = new UnitValueDouble();
            CpuUsage = 0;
            displayCpuUsage = "0";
        }
    }

    public class ProcessVMCopy : ObjectCopy<ProcessVM, VTProcess>
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("");

        /// <summary>
        /// 总的CPU占用（内核 + 空闲 + 用户）
        /// </summary>
        public ulong TotalProcessorTime { get; set; }

        public override bool Compare(ProcessVM target, VTProcess source)
        {
            return target.PID == source.PID;
        }

        public override void CopyTo(ProcessVM target, VTProcess source)
        {
            long previousProcessorTime = target.TotalProcessorTime;

            target.PID = source.PID;
            target.Name = source.Name;
            target.TotalProcessorTime = source.TotalProcessorTime;
            if (VTBaseUtils.UpdateReadable(target.Memory, source.MemoryUsage))
            {
                target.DisplayMemory = target.Memory.ToString();
            }

            if (Elapsed.TotalMilliseconds > 0 && TotalProcessorTime > 0)
            {
                double processorTime = source.TotalProcessorTime - previousProcessorTime;

                //logger.ErrorFormat("{0}, userProcessorTime = {1}", processorTime, this.TotalProcessorTime);

                target.CpuUsage = Math.Round(processorTime / TotalProcessorTime * 100, 2);
            }
        }
    }
}