using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Watch
{
    public class NetInterfaceInfo
    {
        private string id;
        private string name;
        private ulong bytesSent;
        private ulong bytesReceived;
        private string ipaddr;

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
        public ulong BytesSent
        {
            get { return this.bytesSent; }
            set
            {
                if (this.bytesSent != value)
                {
                    this.bytesSent = value;
                }
            }
        }

        /// <summary>
        /// 总接收字节数
        /// </summary>
        public ulong BytesReceived
        {
            get { return this.bytesReceived; }
            set
            {
                if (this.bytesReceived != value)
                {
                    this.bytesReceived = value;
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
                }
            }
        }
    }

    public class Win32NetworkInterfaceCopy : ObjectCopy<NetInterfaceInfo, NetworkInterface>
    {
        private string GetIPAddress(NetworkInterface @interface) 
        {
            IPInterfaceProperties properties = @interface.GetIPProperties();

            // 一张网卡可以设置多个IP地址，这里取第一个，取IPV4
            UnicastIPAddressInformation unicastIPAddress = properties.UnicastAddresses.FirstOrDefault(v => v.Address.AddressFamily == AddressFamily.InterNetwork);
            if (unicastIPAddress == null)
            {
                return string.Empty;
            }

            return unicastIPAddress.Address.ToString();
        }

        public override bool Compare(NetInterfaceInfo target, NetworkInterface source)
        {
            return source.Id == target.ID;
        }

        public override void CopyTo(NetInterfaceInfo target, NetworkInterface source)
        {
            target.ID = source.Id;
            target.Name = source.Description;

            IPv4InterfaceStatistics statistics = source.GetIPv4Statistics();
            target.BytesSent = (ulong)statistics.BytesSent;
            target.BytesReceived = (ulong)statistics.BytesReceived;
            target.IPAddress = this.GetIPAddress(source);
        }
    }
}
