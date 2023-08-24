using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XTerminal.Base;
using XTerminal.Base.DataModels;
using XTerminal.Base.Enumerations;

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

        private SessionDriver driver;

        private Task receiveThread;
        private bool isRunning;

        private byte[] readBuffer;

        private int row;
        private int col;

        #endregion

        #region 公开接口

        public int Initialize(XTermSession session)
        {
            int bufferSize = session.GetOption<int>(OptionKeyEnum.READ_BUFFER_SIZE);
            this.readBuffer = new byte[bufferSize];
            this.row = session.GetOption<int>(OptionKeyEnum.SSH_TERM_ROW);
            this.col = session.GetOption<int>(OptionKeyEnum.SSH_TERM_COL);

            this.driver = SessionFactory.Create(session);
            this.driver.StatusChanged += Session_StatusChanged;

            return ResponseCode.SUCCESS;
        }

        public void Release()
        {
            this.driver.StatusChanged -= this.Session_StatusChanged;
            this.driver = null;
        }

        public int Open()
        {
            int code = this.driver.Open();
            if (code != ResponseCode.SUCCESS)
            {
                return code;
            }

            this.driver.Status = SessionStatusEnum.Connected;

            this.isRunning = true;
            this.receiveThread = Task.Factory.StartNew(this.ReceiveThreadProc);
            return ResponseCode.SUCCESS;
        }

        public void Close()
        {
            this.isRunning = false;
            this.receiveThread.Wait();
            this.driver.Close();
        }

        public int Write(byte[] bytes)
        {
            if (!this.CheckDriverStatus())
            {
                return ResponseCode.SUCCESS;
            }

            try
            {
                this.driver.Write(bytes);
                return ResponseCode.SUCCESS;
            }
            catch (Exception ex)
            {
                logger.Error("发送数据异常", ex);
                return ResponseCode.FAILED;
            }
        }

        /// <summary>
        /// 重置终端大小
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void Resize(int row, int col)
        {
            if (!this.CheckDriverStatus())
            {
                return;
            }

            if (this.row == row && this.col == col) 
            {
                return;
            }

            this.driver.Resize(row, col);

            this.row = row;
            this.col = col;
        }

        #endregion

        #region 实例方法

        private bool CheckDriverStatus()
        {
            if (this.driver.Status != SessionStatusEnum.Connected)
            {
                return false;
            }

            return true;
        }

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
                int n = this.driver.Read(this.readBuffer);

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
                    this.NotifyDataReceived(this.readBuffer, n);
                }
            }
        }

        #endregion
    }
}
