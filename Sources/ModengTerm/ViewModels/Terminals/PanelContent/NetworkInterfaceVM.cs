using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels.Terminals.PanelContent
{
    public class NetworkInterfaceVM : ItemViewModel
    {
        private int uploadSpeed;
        private int downloadSpeed;

        /// <summary>
        /// 上行速度
        /// </summary>
        public int UploadSpeed 
        {
            get { return this.uploadSpeed; }
            set
            {
                if (this.uploadSpeed != value) 
                {
                    this.uploadSpeed = value;
                    this.NotifyPropertyChanged("UploadSpeed");
                }
            }
        }

        /// <summary>
        /// 下行速度
        /// </summary>
        public int DownloadSpeed 
        {
            get { return this.downloadSpeed; }
            set
            {
                if (this.downloadSpeed != value)
                {
                    this.downloadSpeed = value;
                    this.NotifyPropertyChanged("DownloadSpeed");
                }
            }
        }
    }
}
