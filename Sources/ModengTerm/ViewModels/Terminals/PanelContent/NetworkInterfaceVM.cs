using ModengTerm.Terminal.Watch;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels.Terminals.PanelContent
{
    public class NetworkInterfaceVM : ItemViewModel
    {
        private string uploadSpeed;
        private string downloadSpeed;
        private string ipaddr;
        private ulong bytesSent;
        private ulong bytesReceived;
        private ulong prevBytesSent;
        private ulong prevBytesReceived;

        /// <summary>
        /// 上行速度
        /// </summary>
        public string UploadSpeed
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
        public string DownloadSpeed
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

        /// <summary>
        /// IP地址
        /// </summary>
        public string IPAddress
        {
            get { return this.ipaddr; }
            set
            {
                if (this.ipaddr != value)
                {
                    this.ipaddr = value;
                    this.NotifyPropertyChanged("IPAddress");
                }
            }
        }

        public ulong BytesSent
        {
            get { return this.bytesSent; }
            set
            {
                if (this.bytesSent != value)
                {
                    this.prevBytesSent = this.bytesSent;
                    this.bytesSent = value;
                    this.NotifyPropertyChanged("BytesSent");
                }
            }
        }

        public ulong BytesReceived
        {
            get { return this.bytesReceived; }
            set
            {
                if (this.bytesReceived != value)
                {
                    this.prevBytesReceived = this.bytesReceived;
                    this.bytesReceived = value;
                    this.NotifyPropertyChanged("BytesReceived");
                }
            }
        }

        /// <summary>
        /// 记录上一次发送的数据量
        /// </summary>
        public ulong PreviousBytesSent
        {
            get { return this.prevBytesSent; }
        }

        /// <summary>
        /// 记录上一次接收的数据量
        /// </summary>
        public ulong PreviousBytesReceived
        {
            get { return this.prevBytesReceived; }
        }

        public NetworkInterfaceVM() 
        {
            this.UploadSpeed = "0kb/s";
            this.DownloadSpeed = "0kb/s";
        }
    }

    public class NetworkInterfaceVMCopy : ObjectCopy<NetworkInterfaceVM, NetInterfaceInfo>
    {
        public override bool Compare(NetworkInterfaceVM target, NetInterfaceInfo source)
        {
            return target.ID.ToString() == source.ID;
        }

        public override void CopyTo(NetworkInterfaceVM target, NetInterfaceInfo source)
        {
            target.ID = source.ID;
            target.Name = source.Name;
            target.IPAddress = source.IPAddress;
            target.BytesSent = source.BytesSent;
            target.BytesReceived = source.BytesReceived;
        }
    }
}
