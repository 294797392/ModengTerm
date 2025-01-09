using ModengTerm.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Watch
{
    public class VTProcess
    {
        private int pid;
        private string name;
        private string filePath;
        private long totalProcessorTime;
        private bool canRead;

        /// <summary>
        /// 进程Id
        /// </summary>
        public int PID
        {
            get { return this.pid; }
            set
            {
                if (this.pid != value)
                {
                    this.pid = value;
                }
            }
        }

        /// <summary>
        /// 进程名字
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                }
            }
        }

        /// <summary>
        /// 可执行文件完整路径
        /// </summary>
        public string FilePath
        {
            get { return this.filePath; }
            set
            {
                if (this.filePath != value)
                {
                    this.filePath = value;
                }
            }
        }

        /// <summary>
        /// 内存占用字节数
        /// </summary>
        public UnitValue64 MemoryUsage { get; private set; }

        /// <summary>
        /// Cpu总使用时间
        /// 用户时间+内核时间
        /// </summary>
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

        /// <summary>
        /// 指示是否有权限读取该进程相关的数据
        /// </summary>
        public bool CanRead
        {
            get { return this.canRead; }
            set
            {
                if (this.canRead != value)
                {
                    this.canRead = value;
                }
            }
        }

        public VTProcess()
        {
            this.CanRead = true;
            this.MemoryUsage = new UnitValue64();
        }
    }

    public class Win32ProcessCopy : ObjectCopy<VTProcess, Process>
    {
        public override bool Compare(VTProcess target, Process source)
        {
            return source.Id == target.PID && source.ProcessName == target.Name;
        }

        public override void CopyTo(VTProcess target, Process source)
        {
            target.PID = source.Id;
            target.MemoryUsage.Value = (ulong)source.WorkingSet64;
            target.MemoryUsage.Unit = UnitType.Byte;
            target.Name = source.ProcessName;

            if (source.Id == 0 || source.Id == 4)
            {
                // 系统进程，不读取其他额外信息，因为没权限
                return;
            }

            if (!target.CanRead)
            {
                // 之前读取过其他信息但是报异常了，现在也不读
                return;
            }

            // 极端情况下可能会出问题：如果用户启动了一个和无权限进程一样名字的进程，并且此时PID也和之前的无权限进程一样，那么这个进程就会复用之前的对象，之前的对象CanRead == false，导致不会去读取数据

            try
            {
                if (source.MainModule != null && source.MainModule.FileName != null)
                {
                    target.FilePath = source.MainModule.FileName;
                }

                //target.TotalProcessorTime = source.TotalProcessorTime.TotalMilliseconds;
                target.TotalProcessorTime = (long)source.TotalProcessorTime.Ticks;
            }
            catch (Win32Exception e)
            {
                target.CanRead = false;
            }
            catch (Exception e)
            {
                target.CanRead = false;
            }
        }
    }

    public class UnixProcessCopy : ObjectCopy<VTProcess, string>
    {
        public override bool Compare(VTProcess target, string source)
        {
            return source.StartsWith(target.PID.ToString());
        }

        public override void CopyTo(VTProcess target, string source)
        {
            string[] items = source.Split(',');

            int pid;
            if (int.TryParse(items[0], out pid))
            {
                target.PID = pid;
            }

            target.Name = items[1];

            ulong rss;
            if (ulong.TryParse(items[2], out rss))
            {
                target.MemoryUsage.Value = rss;
                target.MemoryUsage.Unit = UnitType.KB;
            }

            long utime = 0, ktime = 0;
            long.TryParse(items[3], out utime);
            long.TryParse(items[4], out ktime);

            target.TotalProcessorTime = utime + ktime;
        }
    }
}
