using ModengTerm.Enumerations;
using ModengTerm.Terminal.Watch;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels.Terminals.PanelContent
{
    public class NetworkInterfaceVM : ItemViewModel
    {
        #region 实例变量

        private string uploadSpeed;
        private string downloadSpeed;
        private string ipaddr;
        private ulong bytesSent;
        private ulong bytesReceived;
        private string ifaceId;

        #endregion

        #region 属性

        public string IfaceId
        {
            get { return this.ifaceId; }
            set
            {
                if (this.ifaceId != value)
                {
                    this.ifaceId = value;
                    this.NotifyPropertyChanged("IfaceId");
                }
            }
        }

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
                    this.bytesReceived = value;
                    this.NotifyPropertyChanged("BytesReceived");
                }
            }
        }

        #endregion

        #region 构造方法

        public NetworkInterfaceVM()
        {
            this.UploadSpeed = "0kb/s";
            this.DownloadSpeed = "0kb/s";
        }

        #endregion
    }

    public class NetworkInterfaceVMCopy : ObjectCopy<NetworkInterfaceVM, NetInterfaceInfo>
    {
        public override bool Compare(NetworkInterfaceVM target, NetInterfaceInfo source)
        {
            return target.IfaceId == source.ID;
        }

        public override void CopyTo(NetworkInterfaceVM target, NetInterfaceInfo source)
        {
            ulong previousBytesSent = target.BytesSent;
            ulong previousBytesReceived = target.BytesReceived;

            target.IfaceId = source.ID;
            target.Name = source.Name;
            target.IPAddress = source.IPAddress;
            target.BytesSent = source.BytesSent;
            target.BytesReceived = source.BytesReceived;

            double seconds = this.Elapsed.TotalSeconds;
            if (seconds > 0)
            {
                if (target.BytesReceived != previousBytesReceived)
                {
                    // 上次传输的数据和本次传输的数据的差值
                    ulong received = target.BytesReceived - previousBytesReceived;
                    double downspeedBytes = received / seconds;
                    SizeUnitsEnum downunit;
                    int downspeed = (int)ClientUtils.ConvertToHumanReadableUnit(downspeedBytes, out downunit, 1);
                    target.DownloadSpeed = string.Format("{0}{1}/s", downspeed, ClientUtils.Unit2Suffix(downunit));
                }

                if (target.BytesSent != previousBytesSent)
                {
                    ulong sent = target.BytesSent - previousBytesSent;
                    double upspeedBytes = sent / seconds;
                    SizeUnitsEnum upunit;
                    int upspeed = (int)ClientUtils.ConvertToHumanReadableUnit(upspeedBytes, out upunit, 1);
                    target.UploadSpeed = string.Format("{0}{1}/s", upspeed, ClientUtils.Unit2Suffix(upunit));
                }
            }
        }
    }
}
