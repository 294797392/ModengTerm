using ModengTerm.Base;
using ModengTerm.Enumerations;
using ModengTerm.Terminal.Watch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels.Terminals.PanelContent
{
    /// <summary>
    /// 磁盘驱动器ViewModel
    /// </summary>
    public class DiskVM : UpdatableVM<DiskInfo>
    {
        #region 实例变量

        private string format;
        private string freeRatio;

        #endregion

        #region 属性

        /// <summary>
        /// 磁盘总空间
        /// </summary>
        public UnitValue TotalSpace { get; private set; }

        /// <summary>
        /// 磁盘空闲空间
        /// </summary>
        public UnitValue FreeSpace { get; private set; }

        /// <summary>
        /// 磁盘格式
        /// </summary>
        public string Format
        {
            get { return this.format; }
            set
            {
                if (this.format != value)
                {
                    this.format = value;
                }
            }
        }

        public string FreeRatio
        {
            get { return this.freeRatio; }
            set
            {
                if (this.freeRatio != value)
                {
                    this.freeRatio = value;
                    this.NotifyPropertyChanged("FreeRatio");
                }
            }
        }

        #endregion

        #region 构造方法

        public DiskVM()
        {
            this.TotalSpace = new UnitValue();
            this.FreeSpace = new UnitValue();
        }

        #endregion

        public override void Update(DiskInfo diskInfo)
        {
            this.ID = diskInfo.ID;
            this.Name = diskInfo.Name;
            this.Format = diskInfo.Format;

            bool sizeChanged = false;

            sizeChanged = ClientUtils.UpdateSpaceSize(this.TotalSpace, diskInfo.TotalSpace);
            sizeChanged = ClientUtils.UpdateSpaceSize(this.FreeSpace, diskInfo.FreeSpace);

            if (sizeChanged)
            {
                this.FreeRatio = string.Format("{0}/{1}", this.FreeSpace, this.TotalSpace);
            }
        }
    }
}
