using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XTerminal.Base;
using XTerminal.Base.DataModels;

namespace XTerminal.Session
{
    public class SessionTransport
    {
        #region 类变量

        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("SessionTransport");

        #endregion

        #region 公开事件

        /// <summary>
        /// 当Session状态发生改变的时候触发
        /// </summary>
        public event Action<object, SessionStatusEnum> StatusChanged;

        /// <summary>
        /// 当收到数据流的时候触发
        /// </summary>
        public event Action<SessionTransport, byte[], int> DataReceived;

        #endregion

        #region 实例变量

        private SessionBase session;

        private Task receiveThread;
        private bool isRunning;

        private byte[] outputBuffer;

        #endregion

        #region 公开接口

        public int Initialize(XTermSession initialOptions)
        {
            this.outputBuffer = new byte[initialOptions.OutputBufferSize];

            this.session = SessionFactory.Create(initialOptions);
            this.session.StatusChanged += Session_StatusChanged;

            return ResponseCode.SUCCESS;
        }

        public void Release()
        {
            this.session.StatusChanged -= this.Session_StatusChanged;
            this.session = null;
        }

        public int Open()
        {
            int code = this.session.Open();
            if (code != ResponseCode.SUCCESS)
            {
                return code;
            }

            this.isRunning = true;
            this.receiveThread = Task.Factory.StartNew(this.ReceiveThreadProc);
            return ResponseCode.SUCCESS;
        }

        public void Close()
        {
            this.isRunning = false;
            this.receiveThread.Wait();
            this.session.Close();
        }

        public int Write(byte[] bytes)
        {
            try
            {
                this.session.Write(bytes);
                return ResponseCode.SUCCESS;
            }
            catch (Exception ex)
            {
                logger.Error("发送数据异常", ex);
                return ResponseCode.FAILED;
            }
        }

        #endregion

        #region 实例方法

        private void NotifyDataReceived(byte[] bytes, int size)
        {
            if (this.DataReceived != null)
            {
                this.DataReceived(this, bytes, size);
            }
        }

        protected void NotifyStatusChanged(SessionStatusEnum status)
        {
            if (this.StatusChanged != null)
            {
                this.StatusChanged(this, status);
            }
        }

        #endregion

        #region 事件处理器

        private void Session_StatusChanged(object sender, SessionStatusEnum status)
        {
            this.NotifyStatusChanged(status);
        }

        private void ReceiveThreadProc()
        {
            // 没读取道数据的次数
            // 如果超过3次，那么就判断为已经断开连接
            int zeros = 0;

            while (this.isRunning)
            {
                int n = this.session.Read(this.outputBuffer);

                if (n == -1)
                {
                    // 读取失败，直接断线处理
                }
                else if (n == 0)
                {
                    // 没读取到数据
                    zeros++;
                    if (zeros > 3)
                    {
                        // 读取失败，直接断线处理
                    }
                    else
                    {
                        Thread.Sleep(50);
                    }
                }
                else
                {
                    // 重置zero计数器
                    if(zeros != 0)
                    {
                        zeros = 0;
                    }

                    // 读取到了数据
                    this.NotifyDataReceived(this.outputBuffer, n);
                }
            }
        }

        #endregion
    }
}
