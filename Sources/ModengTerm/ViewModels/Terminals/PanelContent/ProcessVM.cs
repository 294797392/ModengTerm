using ModengTerm.Base;
using ModengTerm.Terminal.Watch;
using System;
using System.ComponentModel;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels.Terminals.PanelContent
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
            get { return this.pid; }
            set
            {
                if (this.pid != value)
                {
                    this.pid = value;
                    this.NotifyPropertyChanged("PID");
                }
            }
        }

        public UnitValueDouble Memory
        {
            get { return this.memory; }
            set
            {
                if (this.memory != value)
                {
                    this.memory = value;
                    this.NotifyPropertyChanged("Memory");
                }
            }
        }

        public string DisplayMemory
        {
            get { return this.displayMemory; }
            set
            {
                if (this.displayMemory != value)
                {
                    this.displayMemory = value;
                    this.NotifyPropertyChanged("DisplayMemory");
                }
            }
        }

        public long TotalProcessorTime
        {
            get { return this.totalProcessorTime; }
            set
            {
                if (this.totalProcessorTime != value)
                {
                    this.totalProcessorTime = value;
                }
            }
        }

        public double CpuUsage
        {
            get { return this.cpuUsage; }
            set
            {
                if (this.cpuUsage != value)
                {
                    this.cpuUsage = value;
                }
            }
        }

        public ProcessVM()
        {
            this.Memory = new UnitValueDouble();
            this.CpuUsage = 0;
            this.displayCpuUsage = "0";
        }
    }

    public class ProcessVMCopy : ObjectCopy<ProcessVM, ProcessInfo>
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("");

        /// <summary>
        /// 总的CPU占用（内核 + 空闲 + 用户）
        /// </summary>
        public ulong TotalProcessorTime { get; set; }

        public override bool Compare(ProcessVM target, ProcessInfo source)
        {
            return target.PID == source.PID;
        }

        public override void CopyTo(ProcessVM target, ProcessInfo source)
        {
            long previousProcessorTime = target.TotalProcessorTime;

            target.PID = source.PID;
            target.Name = source.Name;
            target.TotalProcessorTime = source.TotalProcessorTime;
            if (MTermUtils.UpdateReadable(target.Memory, source.MemoryUsage))
            {
                target.DisplayMemory = target.Memory.ToString();
            }

            if (this.Elapsed.TotalMilliseconds > 0 && this.TotalProcessorTime > 0)
            {
                double processorTime = (source.TotalProcessorTime - previousProcessorTime);

                //logger.ErrorFormat("{0}, userProcessorTime = {1}", processorTime, this.TotalProcessorTime);

                target.CpuUsage = Math.Round(processorTime / this.TotalProcessorTime * 100, 2);
            }
        }
    }
}