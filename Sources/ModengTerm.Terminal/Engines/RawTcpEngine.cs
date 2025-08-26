using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Terminal.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Engines
{
    public class RawTcpEngine : AbstractEngin
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("RawTcpSession");

        private TcpListener tcpListener;
        private TcpClient client;
        private NetworkStream stream;

        public RawTcpEngine(XTermSession session) :
            base(session)
        {
        }

        public override int Open()
        {
            RawTcpTypeEnum type = this.session.GetOption<RawTcpTypeEnum>(OptionKeyEnum.RAW_TCP_TYPE);
            string ipaddr = this.session.GetOption<string>(OptionKeyEnum.RAW_TCP_ADDRESS);
            int port = this.session.GetOption<int>(OptionKeyEnum.RAW_TCP_PORT);

            switch (type)
            {
                case RawTcpTypeEnum.Client:
                    {
                        return this.InitTcpClient(ipaddr, port);
                    }

                case RawTcpTypeEnum.Server:
                    {
                        return this.InitTcpListener(ipaddr, port);
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        public override void Close()
        {
            if (this.client != null)
            {
                this.client.Close();
                this.client.Dispose();
            }

            if (this.tcpListener != null)
            {
                this.tcpListener.Stop();
            }
        }

        public override void Resize(int row, int col)
        {
            // do nothing...
        }

        public override void Write(byte[] buffer)
        {
            this.stream.Write(buffer, 0, buffer.Length);
            this.stream.Flush();
        }

        internal override int Read(byte[] buffer)
        {
            return this.stream.Read(buffer, 0, buffer.Length);
        }


        private int InitTcpClient(string ipaddr, int port) 
        {
            try
            {
                this.client = new TcpClient();
                this.client.Connect(IPAddress.Parse(ipaddr), port);

                this.stream = this.client.GetStream();

                logger.InfoFormat("RawTcpSession连接成功, {0}:{1}", ipaddr, port);

                return ResponseCode.SUCCESS;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("RawTcpSession连接异常, {0}:{1}", ipaddr, port), ex);
                return ResponseCode.FAILED;
            }
        }

        private int InitTcpListener(string ipaddr, int port)
        {
            IPAddress addr = string.IsNullOrEmpty(ipaddr) ? IPAddress.Any : IPAddress.Parse(ipaddr);

            this.tcpListener = new TcpListener(addr, port);
            this.tcpListener.Start();
            try
            {
                TcpClient client = this.tcpListener.AcceptTcpClient();

                this.stream = client.GetStream();

                this.client = client;

                logger.InfoFormat("RawTcpSession监听成功, 客户端已连接");

                return ResponseCode.SUCCESS;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("RawTcpSession监听异常, {0}:{1}", ipaddr, port), ex);
                return ResponseCode.FAILED;       
            }
        }
    }
}
