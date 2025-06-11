using ModengTerm.Base;
using WPFToolkit.MVVM;

namespace ModengTerm.Addon.Official.SystemMonitor
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
            get { return ifaceId; }
            set
            {
                if (ifaceId != value)
                {
                    ifaceId = value;
                    NotifyPropertyChanged("IfaceId");
                }
            }
        }

        /// <summary>
        /// 上行速度
        /// </summary>
        public string UploadSpeed
        {
            get { return uploadSpeed; }
            set
            {
                if (uploadSpeed != value)
                {
                    uploadSpeed = value;
                    NotifyPropertyChanged("UploadSpeed");
                }
            }
        }

        /// <summary>
        /// 下行速度
        /// </summary>
        public string DownloadSpeed
        {
            get { return downloadSpeed; }
            set
            {
                if (downloadSpeed != value)
                {
                    downloadSpeed = value;
                    NotifyPropertyChanged("DownloadSpeed");
                }
            }
        }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IPAddress
        {
            get { return ipaddr; }
            set
            {
                if (ipaddr != value)
                {
                    ipaddr = value;
                    NotifyPropertyChanged("IPAddress");
                }
            }
        }

        /// <summary>
        /// 总发送字节数
        /// </summary>
        public ulong BytesSent
        {
            get { return bytesSent; }
            set
            {
                if (bytesSent != value)
                {
                    bytesSent = value;
                    NotifyPropertyChanged("BytesSent");
                }
            }
        }

        /// <summary>
        /// 总接收字节数
        /// </summary>
        public ulong BytesReceived
        {
            get { return bytesReceived; }
            set
            {
                if (bytesReceived != value)
                {
                    bytesReceived = value;
                    NotifyPropertyChanged("BytesReceived");
                }
            }
        }

        #endregion

        #region 构造方法

        public NetworkInterfaceVM()
        {
            UploadSpeed = "0kb/s";
            DownloadSpeed = "0kb/s";
        }

        #endregion
    }

    public class NetworkInterfaceVMCopy : ObjectCopy<NetworkInterfaceVM, VTNetDevice>
    {
        public override bool Compare(NetworkInterfaceVM target, VTNetDevice source)
        {
            return target.IfaceId == source.ID;
        }

        public override void CopyTo(NetworkInterfaceVM target, VTNetDevice source)
        {
            ulong previousBytesSent = target.BytesSent;
            ulong previousBytesReceived = target.BytesReceived;

            target.IfaceId = source.ID;
            target.Name = source.Name;
            target.IPAddress = source.IPAddress;
            target.BytesSent = source.BytesSent;
            target.BytesReceived = source.BytesReceived;

            double seconds = Elapsed.TotalSeconds;
            if (seconds > 0)
            {
                if (target.BytesReceived != previousBytesReceived)
                {
                    // 上次传输的数据和本次传输的数据的差值
                    ulong received = target.BytesReceived - previousBytesReceived;
                    double downspeedBytes = received / seconds;
                    UnitType downunit;
                    int downspeed = (int)VTBaseUtils.ConvertToHumanReadableUnit(downspeedBytes, out downunit, 1);
                    target.DownloadSpeed = string.Format("{0}{1}/s", downspeed, VTBaseUtils.Unit2Suffix(downunit));
                }

                if (target.BytesSent != previousBytesSent)
                {
                    ulong sent = target.BytesSent - previousBytesSent;
                    double upspeedBytes = sent / seconds;
                    UnitType upunit;
                    int upspeed = (int)VTBaseUtils.ConvertToHumanReadableUnit(upspeedBytes, out upunit, 1);
                    target.UploadSpeed = string.Format("{0}{1}/s", upspeed, VTBaseUtils.Unit2Suffix(upunit));
                }
            }
        }
    }
}
