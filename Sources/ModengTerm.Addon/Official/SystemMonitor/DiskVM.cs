using ModengTerm.Base;
using ModengTerm.Terminal.Watch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.Addon.Official.SystemMonitor
{
    /// <summary>
    /// 磁盘驱动器ViewModel
    /// </summary>
    public class DiskVM : ItemViewModel
    {
        #region 实例变量

        private string format;
        private string freeRatio;

        #endregion

        #region 属性

        /// <summary>
        /// 磁盘总空间
        /// </summary>
        public UnitValueDouble TotalSpace { get; private set; }

        /// <summary>
        /// 磁盘空闲空间
        /// </summary>
        public UnitValueDouble FreeSpace { get; private set; }

        /// <summary>
        /// 磁盘格式
        /// </summary>
        public string Format
        {
            get { return format; }
            set
            {
                if (format != value)
                {
                    format = value;
                }
            }
        }

        public string FreeRatio
        {
            get { return freeRatio; }
            set
            {
                if (freeRatio != value)
                {
                    freeRatio = value;
                    NotifyPropertyChanged("FreeRatio");
                }
            }
        }

        #endregion

        #region 构造方法

        public DiskVM()
        {
            TotalSpace = new UnitValueDouble();
            FreeSpace = new UnitValueDouble();
        }

        #endregion
    }

    public class DiskVMCopy : ObjectCopy<DiskVM, VTDrive>
    {
        public override bool Compare(DiskVM target, VTDrive source)
        {
            return target.Name == source.Name;
        }

        public override void CopyTo(DiskVM target, VTDrive source)
        {
            target.ID = source.Name;
            target.Name = source.Name;
            target.Format = source.Format;

            bool sizeChanged = false;

            sizeChanged = VTBaseUtils.UpdateReadable(target.TotalSpace, source.TotalSpace);
            sizeChanged = VTBaseUtils.UpdateReadable(target.FreeSpace, source.FreeSpace);

            if (sizeChanged)
            {
                target.FreeRatio = string.Format("{0}/{1}", target.FreeSpace, target.TotalSpace);
            }
        }
    }
}
