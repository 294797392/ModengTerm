using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Terminal;
using ModengTerm.Terminal.Modem;
using ModengTerm.Terminal.Session;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels.Terminals
{
    public enum ModemTypeEnum
    {
        XModem,
        YModem,
        ZModem
    }

    public class ModemTransferVM : ViewModelBase, IHostStream
    {
        #region 事件

        /// <summary>
        /// 当传输结束的时候触发
        /// </summary>
        public event Action<ModemTransferVM, double, int> ProgressChanged;

        #endregion

        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("ModemTransferVM");

        #endregion

        #region 实例变量

        private double progress;
        private SynchronizedStream synchronizedStream;
        private int readTimeout;

        #endregion

        #region 属性

        public XTermSession Session { get; set; }

        public SessionTransport Transport { get; set; }

        public ModemTypeEnum Type { get; set; }

        /// <summary>
        /// 指定要发送还是接收文件
        /// </summary>
        public SendReceive SendReceive { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public List<string> FilePaths { get; set; }

        public double Progress
        {
            get { return progress; }
            set
            {
                if (this.progress != value)
                {
                    this.progress = value;
                    this.NotifyPropertyChanged("Progress");
                }
            }
        }

        #endregion

        #region 公开接口

        public int StartAsync()
        {
            this.readTimeout = 50000;
            this.synchronizedStream = new SynchronizedStream();
            this.Transport.DataReceived += Transport_DataReceived;

            ModemBase modem = this.CreateModem();
            modem.ProgressChanged += Modem_ProgressChanged;
            modem.HostStream = this;
            modem.Session = this.Session;

            Task.Factory.StartNew(() =>
            {
                try
                {
                    switch (this.SendReceive)
                    {
                        case SendReceive.Send:
                            {
                                modem.Send(this.FilePaths);
                                break;
                            }

                        case SendReceive.Receive:
                            {
                                modem.Receive(this.FilePaths);
                                break;
                            }

                        default:
                            throw new NotImplementedException();
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("文件传输异常", ex);
                    this.ProgressChanged?.Invoke(this, -1, ResponseCode.EXCEPTION);
                }
                finally
                {
                    modem.ProgressChanged -= Modem_ProgressChanged;
                    this.Transport.DataReceived -= Transport_DataReceived;
                    this.synchronizedStream.Close();
                }
            });

            return ResponseCode.SUCCESS;
        }

        public void Stop()
        {
        }

        #endregion

        #region 实例方法

        private ModemBase CreateModem()
        {
            switch (this.Type)
            {
                case ModemTypeEnum.XModem: return new XModem();
                case ModemTypeEnum.YModem: return new YModem();
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region IHostStream

        public int Send(byte[] buffer)
        {
            return this.Transport.Write(buffer);
        }

        public int Receive(byte[] buffer)
        {
            return this.synchronizedStream.Read(buffer, 0, buffer.Length, this.readTimeout);
        }

        public int Receive(byte[] buffer, int timeout)
        {
            return this.synchronizedStream.Read(buffer, 0, buffer.Length, timeout);
        }

        public int Receive(byte[] buffer, int offset, int count)
        {
            return this.synchronizedStream.Read(buffer, offset, count, this.readTimeout);
        }

        public int Receive(byte[] buffer, int offset, int count, int timeout)
        {
            return this.synchronizedStream.Read(buffer, offset, count, timeout);
        }

        #endregion

        #region 事件处理器

        private void Transport_DataReceived(SessionTransport transport, byte[] buffer, int size)
        {
            this.synchronizedStream.Write(buffer, size);
        }

        private void Modem_ProgressChanged(ModemBase modem, double progress, int code)
        {
            this.Progress = progress;

            this.ProgressChanged?.Invoke(this, progress, code);
        }

        #endregion
    }
}
