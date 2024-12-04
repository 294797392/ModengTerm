using log4net.Repository.Hierarchy;
using ModengTerm.Base;
using ModengTerm.Enumerations;
using ModengTerm.Terminal.Watch;
using System;
using System.Drawing.Printing;
using System.Formats.Asn1;
using System.Windows.Interactivity;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels.Terminals.PanelContent
{
    public class ProcessVM : ItemViewModel
    {
        private int pid;
        private UnitValueDouble memory;
        private string displayMemory;
        private double totalProcessorTime;
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

        public double TotalProcessorTime
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

        public string DisplayCpuUsage
        {
            get { return this.displayCpuUsage; }
            set
            {
                if (this.displayCpuUsage != value)
                {
                    this.displayCpuUsage = value;
                    this.NotifyPropertyChanged("DisplayCpuUsage");
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
        private static readonly int ProcessorCount = Environment.ProcessorCount;

        public override bool Compare(ProcessVM target, ProcessInfo source)
        {
            return target.PID == source.PID;
        }

        public override void CopyTo(ProcessVM target, ProcessInfo source)
        {
            double previousProcessorTime = target.TotalProcessorTime;

            target.PID = source.PID;
            target.Name = source.Name;
            target.TotalProcessorTime = source.TotalProcessorTime;
            if (MTermUtils.UpdateReadable(target.Memory, source.MemoryUsage))
            {
                target.DisplayMemory = target.Memory.ToString();
            }

            if (this.Elapsed.Milliseconds > 0)
            {
                if (target.TotalProcessorTime != previousProcessorTime)
                {
                    double time = target.TotalProcessorTime - previousProcessorTime;
                    double cpuUsage = time / this.Elapsed.Milliseconds * 100;
                    cpuUsage /= ProcessVMCopy.ProcessorCount;
                    cpuUsage = Math.Round(cpuUsage, 2);
                    //target.DisplayCpuUsage = Math.Round(cpuUsage, 2).ToString(); // Math.Round(cpuUsage, 0).ToString().PadLeft(2, '0');
                    target.CpuUsage = cpuUsage;
                    target.DisplayCpuUsage = cpuUsage.ToString();
                }
            }
        }
    }
}