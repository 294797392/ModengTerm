using log4net.Filter;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Terminal;
using ModengTerm.Terminal.Modem;
using ModengTerm.Terminal.Session;
using System;
using System.IO;
using System.Threading.Tasks;
using WPFToolkit.MVVM;
using WPFToolkit.Utility;

namespace ModengTerm.ViewModels.Terminals
{
    public enum ModemTypeEnum
    {
        XModem,
        YModem,
        ZModem
    }

    public enum SendReceive
    {
        Send,

        Receive,
    }

    public class ModemTransferVM : ViewModelBase, IHostStream
    {
        #region 事件

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
        public string FilePath { get; set; }

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
            FileStream stream = null;
            try
            {
                stream = this.OpenFile();
            }
            catch (Exception ex)
            {
                MessageBoxUtils.Info("打开文件失败, 请检查文件是否被占用");
                logger.Error("打开文件异常", ex);
                return ResponseCode.FAILED;
            }

            this.readTimeout = 50000;

            ModemBase modem = this.CreateModem();
            modem.ProgressChanged += Modem_ProgressChanged;
            modem.HostStream = this;
            modem.Session = this.Session;
            this.synchronizedStream = new SynchronizedStream();
            this.Transport.DataReceived += Transport_DataReceived;

            Task.Factory.StartNew(() =>
            {
                try
                {
                    switch (this.SendReceive)
                    {
                        case SendReceive.Send:
                            {
                                modem.Send(stream);
                                break;
                            }

                        case SendReceive.Receive:
                            {
                                modem.Receive(stream);
                                break;
                            }

                        default:
                            throw new NotImplementedException();
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("文件传输异常", ex);
                }
                finally
                {
                    modem.ProgressChanged -= Modem_ProgressChanged;
                    this.Transport.DataReceived -= Transport_DataReceived;
                    this.synchronizedStream.Close();
                    stream.Close();
                    stream.Dispose();
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
                default:
                    throw new NotImplementedException();
            }
        }

        private FileStream OpenFile()
        {
            switch (this.SendReceive)
            {
                case SendReceive.Send:
                    {
                        return File.OpenRead(this.FilePath);
                    }

                case SendReceive.Receive:
                    {
                        return new FileStream(this.FilePath, FileMode.Create, FileAccess.ReadWrite);
                    }

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

        private void Modem_ProgressChanged(ModemBase modem, double progress)
        {
            this.Progress = progress;
        }

        #endregion
    }
}
