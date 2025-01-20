using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Watch
{
    public interface IDiskInfo
    {
        string Name { get; set; }

        /// <summary>
        /// 总空间，bytes
        /// </summary>
        double TotalSpace { get; set; }

        /// <summary>
        /// 空闲空间，bytes
        /// </summary>
        double FreeSpace { get; set; }

        /// <summary>
        /// 可用空间，bytes
        /// </summary>
        double AvailableSpace { get; set; }
    }
}
