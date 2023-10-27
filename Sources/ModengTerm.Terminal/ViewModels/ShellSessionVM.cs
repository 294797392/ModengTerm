using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Terminal.DataModels;
using ModengTerm.Terminal.Document;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.Rendering;
using ModengTerm.Terminal.Session;
using ModengTerm.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base.Enumerations;

namespace ModengTerm.Terminal.ViewModels
{
    public class ShellSessionVM : OpenedSessionVM
    {
        #region 类变量

        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("ShellSessionVM");

        #endregion

        #region 实例变量

        private int viewportRow;
        private int viewportColumn;

        /// <summary>
        /// 与终端进行通信的信道
        /// </summary>
        private SessionTransport sessionTransport;

        /// <summary>
        /// 终端引擎
        /// </summary>
        private VideoTerminal videoTerminal;

        private Encoding writeEncoding;

        private bool sendAll;

        /// <summary>
        /// 书签管理器
        /// </summary>
        private VTBookmark bookmarkMgr;

        /// <summary>
        /// 存储当前的录制状态
        /// </summary>
        private RecordContext recordContext;

        #endregion

        #region 属性

        /// <summary>
        /// 可视区域的行数
        /// </summary>
        public int ViewportRow
        {
            get { return viewportRow; }
            set
            {
                if (this.viewportRow != value)
                {
                    this.viewportRow = value;
                    this.NotifyPropertyChanged("ViewportRow");
                }
            }
        }

        /// <summary>
        /// 可视区域的列数
        /// </summary>
        public int ViewportColumn
        {
            get { return this.viewportColumn; }
            set
            {
                if (this.viewportColumn != value)
                {
                    this.viewportColumn = value;
                    this.NotifyPropertyChanged("ViewportColumn");
                }
            }
        }

        /// <summary>
        /// 向外部公开终端模拟器的控制接口
        /// </summary>
        public IVideoTerminal VideoTerminal { get { return this.videoTerminal; } }

        /// <summary>
        /// 是否向所有终端发送数据
        /// </summary>
        public bool SendAll
        {
            get { return this.sendAll; }
            set
            {
                if (this.sendAll != value)
                {
                    this.sendAll = value;
                    this.NotifyPropertyChanged("SendAll");
                }
            }
        }

        /// <summary>
        /// 书签管理器
        /// </summary>
        public VTBookmark BookmarkMgr { get { return this.bookmarkMgr; } }

        #endregion

        #region 构造方法

        public ShellSessionVM(XTermSession session) :
            base(session)
        {
        }

        #endregion

        #region OpenedSessionVM Member

        protected override int OnOpen()
        {
            this.writeEncoding = Encoding.GetEncoding(this.Session.GetOption<string>(OptionKeyEnum.WRITE_ENCODING));

            this.bookmarkMgr = new VTBookmark(this.Session);
            this.recordContext = new RecordContext();

            SessionTransport transport = new SessionTransport();

            #region 初始化终端

            VTOptions options = new VTOptions()
            {
                Session = this.Session,
                WindowHost = this.Content as IDrawingWindow,
                SessionTransport = transport
            };
            this.videoTerminal = new VideoTerminal();
            this.videoTerminal.ViewportChanged += this.VideoTerminal_ViewportChanged;
            this.videoTerminal.Initialize(options);

            #endregion

            #region 连接终端通道

            transport.StatusChanged += this.SessionTransport_StatusChanged;
            transport.DataReceived += this.SessionTransport_DataReceived;
            transport.Initialize(this.Session);
            transport.OpenAsync();
            this.sessionTransport = transport;

            #endregion

            return ResponseCode.SUCCESS;
        }

        protected override void OnClose()
        {
            this.sessionTransport.StatusChanged -= this.SessionTransport_StatusChanged;
            this.sessionTransport.DataReceived -= this.SessionTransport_DataReceived;
            this.sessionTransport.Close();
            this.sessionTransport.Release();

            this.videoTerminal.ViewportChanged -= this.VideoTerminal_ViewportChanged;
            this.videoTerminal.Release();
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 向SSH主机发送用户输入
        /// 用户每输入一个字符，就调用一次这个函数
        /// </summary>
        /// <param name="input">用户输入信息</param>
        public void SendInput(UserInput input)
        {
            if (this.sessionTransport.Status != SessionStatusEnum.Connected)
            {
                return;
            }

            VTKeyboard keyboard = this.videoTerminal.Keyboard;

            byte[] bytes = keyboard.TranslateInput(input);
            if (bytes == null)
            {
                return;
            }

            VTDebug.Context.WriteInteractive(VTSendTypeEnum.UserInput, bytes);

            // 这里输入的都是键盘按键
            int code = this.sessionTransport.Write(bytes);
            if (code != ResponseCode.SUCCESS)
            {
                logger.ErrorFormat("处理输入异常, {0}", ResponseCode.GetMessage(code));
            }
        }

        public void SendInput(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            byte[] bytes = this.writeEncoding.GetBytes(text);

            int code = this.sessionTransport.Write(bytes);
            if (code != ResponseCode.SUCCESS)
            {
                logger.ErrorFormat("发送数据失败, {0}", code);
            }
        }

        /// <summary>
        /// 设置某一行的标签状态
        /// 如果该行所在的文档是备用缓冲区，那么什么都不做
        /// </summary>
        /// <param name="textLine">要设置的行</param>
        /// <param name="targetState">要设置的标签状态</param>
        public void SetBookmarkState(VTextLine textLine, VTBookmarkStates targetState)
        {
            VTDocument document = textLine.OwnerDocument;

            // 备用缓冲区不可以新建书签
            if (document.IsAlternate)
            {
                return;
            }

            if (textLine.BookmarkState == targetState)
            {
                return;
            }

            textLine.BookmarkState = targetState;

            // 重绘
            textLine.RequestInvalidate();

            // 更新历史行里的标签状态
            document.Scrollbar.UpdateHistory(textLine);

            switch (targetState)
            {
                case VTBookmarkStates.Enabled:
                    {
                        this.bookmarkMgr.AddBookmark(textLine);
                        break;
                    }

                case VTBookmarkStates.None:
                    {
                        this.bookmarkMgr.RemoveBookmark(textLine);
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 设置录像状态
        /// </summary>
        /// <param name="targetState">要设置的状态</param>
        public void SetRecordState(RecordStateEnum targetState)
        {
            if (this.recordContext.State == targetState)
            {
                return;
            }

            switch (targetState)
            {
                case RecordStateEnum.Stop:
                    {
                        PlaybackFile playbackFile = this.recordContext.PlaybackFile;
                        playbackFile.Close();
                        break;
                    }

                case RecordStateEnum.Start:
                    {
                        PlaybackFile playbackFile = this.recordContext.PlaybackFile;
                        //playbackFile.OpenWrite();
                        break;
                    }

                case RecordStateEnum.Resume:
                case RecordStateEnum.Pause:
                    {
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            this.recordContext.State = targetState;
        }

        #endregion

        #region 事件处理器

        private void VideoTerminal_ViewportChanged(IVideoTerminal vt, int newRow, int newColumn)
        {
            this.ViewportRow = newRow;
            this.ViewportColumn = newColumn;
        }

        private void SessionTransport_DataReceived(SessionTransport client, byte[] bytes, int size)
        {
            this.videoTerminal.ProcessData(bytes, size);

            switch (this.recordContext.State)
            {
                case RecordStateEnum.Pause:
                    {
                        break;
                    }

                case RecordStateEnum.Stop:
                    {
                        break;
                    }

                case RecordStateEnum.Resume:
                case RecordStateEnum.Start:
                    {
                        byte[] frameData = new byte[size];
                        if (size != bytes.Length)
                        {
                            Buffer.BlockCopy(bytes, 0, frameData, 0, frameData.Length);
                        }

                        PlaybackFrame frame = new PlaybackFrame()
                        {
                            Timestamp = DateTime.Now.ToFileTime(),
                            Data = frameData
                        };

                        PlaybackFile playbackFile = this.recordContext.PlaybackFile;

                        playbackFile.WriteFrame(frame);

                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        private void SessionTransport_StatusChanged(object client, SessionStatusEnum status)
        {
            logger.InfoFormat("会话状态发生改变, {0}", status);

            try
            {
                switch (status)
                {
                    case SessionStatusEnum.Connected:
                        {
                            break;
                        }

                    case SessionStatusEnum.Connecting:
                        {
                            break;
                        }

                    case SessionStatusEnum.ConnectionError:
                        {
                            break;
                        }

                    case SessionStatusEnum.Disconnected:
                        {
                            break;
                        }

                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                logger.Error("SessionTransport_StatusChanged异常", ex);
            }

            base.NotifyStatusChanged(status);
        }

        #endregion
    }
}
