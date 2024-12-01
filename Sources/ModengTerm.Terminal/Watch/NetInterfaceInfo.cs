using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Watch
{
    public class NetInterfaceInfo
    {
        private string id;
        private string name;
        private int downloadSpeed;
        private int uploadSpeed;
        private ulong totalSend;
        private ulong totalReceive;

        public string ID
        {
            get { return this.id; }
            set
            {
                if (this.id != value)
                {
                    this.id = value;
                }
            }
        }

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
        /// 总发送字节数
        /// </summary>
        public ulong TotalSend
        {
            get { return this.totalSend; }
            set
            {
                if (this.totalSend != value)
                {
                    this.totalSend = value;
                }
            }
        }

        /// <summary>
        /// 总接收字节数
        /// </summary>
        public ulong TotalReceive
        {
            get { return this.totalReceive; }
            set
            {
                if (this.totalReceive != value)
                {
                    this.totalReceive = value;
                }
            }
        }

        public int DownloadSpeed
        {
            get { return this.downloadSpeed; }
            set
            {
                if (this.downloadSpeed != value)
                {
                    this.downloadSpeed = value;
                }
            }
        }

        public int UploadSpeed
        {
            get { return this.uploadSpeed; }
            set
            {
                if (this.uploadSpeed != value)
                {
                    this.uploadSpeed = value;
                }
            }
        }
    }

    public class Win32NetworkInterfaceCopy : ObjectCopy<NetInterfaceInfo, NetworkInterface>
    {
        public override bool Compare(NetInterfaceInfo target, NetworkInterface source)
        {
            return source.Id == target.ID;
        }

        public override void CopyTo(NetInterfaceInfo target, NetworkInterface source)
        {
            target.ID = source.Id;
            target.Name = source.Description;

            IPInterfaceStatistics statistics = source.GetIPStatistics();
            target.TotalSend = (ulong)statistics.BytesSent;
            target.TotalReceive = (ulong)statistics.BytesReceived;
        }
    }
}
