using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using XTerminalBase.Channels;
using XTerminalBase.IVideoTerminal;
using XTerminalParser;

namespace XTerminalBase
{
    /// <summary>
    /// 处理虚拟终端的所有逻辑
    /// </summary>
    public class VideoTerminal
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTApplication");

        #endregion

        #region 实例变量

        /// <summary>
        /// 用来临时存储TextBlock的列表
        /// 对TextBlock的操作肯定都是在同一个线程里，所以不存在多线程的问题
        /// 省掉了每次都要创建集合的操作
        /// </summary>
        private List<VTextBlock> textBlocks;

        private List<VTextBlock> textBlocks2;

        private ChannelAuthorition authorition;

        /// <summary>
        /// 与终端进行通信的信道
        /// </summary>
        private VTChannel vtChannel;

        /// <summary>
        /// 终端字符解析器
        /// </summary>
        private VTParser vtParser;

        /// <summary>
        /// 所有行
        /// </summary>
        private List<VTextLine> textLines;

        /// <summary>
        /// 当前正在渲染的文本块
        /// 当前正在活动的文本块（Active Data）
        /// </summary>
        private VTextBlock activeTextBlock;

        // 当前渲染的文本的左上角X位置
        private double textOffsetX;

        // 空白字符的宽度
        private double whitespaceWidth;

        /// <summary>
        /// 一个字符的高度
        /// </summary>
        private double characterHeight;

        /// <summary>
        /// 光标所在行
        /// </summary>
        private int cursorRow;

        /// <summary>
        /// 光标所在列
        /// </summary>
        private int cursorCol;

        /// <summary>
        /// Terminal区域的总长宽
        /// </summary>
        private double fullWidth;
        private double fullHeight;

        /// <summary>
        /// SIMD - SELECT IMPLICIT MOVEMENT DIRECTION
        /// 0：数据的移动方向和字符方向一致（从左到右）
        /// 1：数据的移动方向和字符方向相反（从右到左）
        /// </summary>
        private int implicitMovementDirection;

        #region Modes - ECMA048 - Modes

        /// <summary>
        /// DCSM - DEVICE COMPONENT SELECT MODE
        /// PRESENTATION:
        /// 某些控制功能在Presentaion模式下执行，
        /// DATA;
        /// 某些控制功能在Data模式下执行，
        /// 
        /// 默认PRESENTATION状态
        /// 
        /// 受DCSM状态影响的控制功能有：CPR, CR, DCH, DL, EA, ECH, ED, EF, EL, ICH, IL, LF, NEL, RI, SLH, SLL, SPH, SPL
        /// </summary>
        private int deviceComponentSelectMode;

        #endregion

        /// <summary>
        /// UI线程上下文
        /// </summary>
        private SynchronizationContext uiSyncContext;

        #endregion

        #region 属性

        /// <summary>
        /// 终端设备的一些接口
        /// </summary>
        public IVTController Controller { get; set; }

        /// <summary>
        /// 输入设备
        /// </summary>
        public IInputDevice InputDevice { get; private set; }

        /// <summary>
        /// 终端显示器接口
        /// </summary>
        public IPresentationDevice PresentationDevice { get; private set; }

        /// <summary>
        /// 根据当前电脑键盘的按键状态，转换成对应的终端数据流
        /// </summary>
        public VTKeyboard Keyboard { get; private set; }

        public VTextOptions TextOptions { get; private set; }

        #endregion

        #region 构造方法

        public VideoTerminal()
        {
        }

        #endregion

        #region 公开接口

        public void Initialize()
        {
            this.uiSyncContext = SynchronizationContext.Current;

            // 0:和字符方向相同（向右）
            // 1:和字符方向相反（向左）
            this.implicitMovementDirection = 0;

            this.textBlocks = new List<VTextBlock>();
            this.textBlocks2 = new List<VTextBlock>();
            this.textLines = new List<VTextLine>();
            this.TextOptions = new VTextOptions();

            // 创建第一个TextBlock
            this.activeTextBlock = this.CreateTextBlock(0, 0, 0);

            this.Keyboard = new VTKeyboard();
            this.Keyboard.SetAnsiMode(true);
            this.Keyboard.SetKeypadMode(false);

            // 初始化视频终端
            this.PresentationDevice = this.Controller.CreatePresentationDevice();
            this.Controller.SwitchPresentaionDevice(null, this.PresentationDevice);
            this.InputDevice = this.Controller.GetInputDevice();
            this.InputDevice.InputEvent += this.VideoTerminal_InputEvent;

            // 初始化终端解析器
            this.vtParser = new VTParser();
            this.vtParser.ActionEvent += VtParser_ActionEvent;
            this.vtParser.Initialize();

            VTextBlock spaceText = new VTextBlock()
            {
                Text = " ",
                Style = VTextStyle.Default
            };
            VTextBlockMetrics spaceTextMetrics = this.PresentationDevice.MeasureText(spaceText);
            this.whitespaceWidth = spaceTextMetrics.WidthIncludingWhitespace;
            this.characterHeight = spaceTextMetrics.Height;
        }

        public void RunSSHClient(SSHChannelAuthorition authorition)
        {
            this.authorition = authorition;
            this.vtChannel = VTChannelFactory.CreateSSHClient(authorition.ServerAddress, authorition.ServerPort, authorition.UserName, authorition.Password);
            this.vtChannel.StatusChanged += this.VTChannel_StatusChanged;
            this.vtChannel.DataReceived += this.VTChannel_DataReceived;
            this.vtChannel.Connect();
        }

        /// <summary>
        /// 关闭连接并释放资源
        /// </summary>
        public void Exit()
        {
            //this.terminal.InputEvent -= this.VideoTerminal_InputEvent;

            //this.vtParser.ActionEvent -= this.VtParser_ActionEvent;

            //this.vtChannel.StatusChanged -= this.VTChannel_StatusChanged;
            //this.vtChannel.DataReceived -= this.VTChannel_DataReceived;
            //this.vtChannel.Disconnect();
        }

        #endregion

        #region 实例方法

        private VTextBlock CreateTextBlock(int row, int column, double offsetX)
        {
            VTextBlock textBlock = new VTextBlock()
            {
                ID = Guid.NewGuid().ToString(),
                Column = column,
                Row = row,
                X = offsetX,
                Y = row * this.characterHeight,
                Text = string.Empty,
                Style = VTextStyle.Default
            };

            // 找到文本块所属的行，并把文本块加到行里
            VTextLine ownerLine = this.textLines.FirstOrDefault(v => v.Row == row);
            if (ownerLine == null)
            {
                ownerLine = new VTextLine()
                {
                    Row = row,
                    OffsetY = this.characterHeight * row,
                };
                this.textLines.Add(ownerLine);
            }

            ownerLine.AddText(textBlock);
            textBlock.OwnerLine = ownerLine;

            return textBlock;
        }

        /// <summary>
        /// 获取光标所在文本行
        /// </summary>
        /// <returns></returns>
        private VTextLine GetCursorTextLine()
        {
            return this.textLines.FirstOrDefault(v => v.Row == this.cursorRow);
        }

        /// <summary>
        /// 获取光标所在文本块
        /// </summary>
        /// <returns></returns>
        private VTextBlock GetCursorTextBlock()
        {
            VTextLine cursorLine = this.GetCursorTextLine();
            if (cursorLine == null)
            {
                logger.WarnFormat("GetCursorTextBlock失败, 光标所在行不存在, cursorRow = {0}", this.cursorRow);
                return null;
            }

            // 获取光标所在文本块
            VTextBlock cursorText = cursorLine.HitTestText(this.cursorCol);
            if (cursorText == null)
            {
                logger.WarnFormat("GetCursorTextBlock失败, 获取光标所在TextBlock失败, cursorCol = {0}", this.cursorCol);
                return null;
            }

            return cursorText;
        }

        /// <summary>
        /// 重新测量Terminal所需要的大小
        /// 如果大小改变了，那么调整布局大小
        /// </summary>
        private void InvalidateMeasure()
        {
            if (this.activeTextBlock == null)
            {
                return;
            }

            double width = Math.Max(this.activeTextBlock.Boundary.RightBottom.X, this.fullWidth);
            double height = Math.Max(this.activeTextBlock.Boundary.RightBottom.Y, this.fullHeight);

            // 布局大小是否改变了
            bool sizeChanged = false;

            // 长宽和原来的完整长宽不一样就算改变了
            if (width != this.fullWidth || height != this.fullHeight)
            {
                sizeChanged = true;
            }

            if (sizeChanged)
            {
                this.fullWidth = width;
                this.fullHeight = height;

                this.uiSyncContext.Send((v) =>
                {
                    this.PresentationDevice.Resize(width, height);
                    this.PresentationDevice.ScrollToEnd(ScrollOrientation.Bottom);
                }, null);
            }
        }

        /// <summary>
        /// 执行删除行操作
        /// </summary>
        /// <param name="parameter"></param>
        private void PerformEraseLine(int parameter)
        {
            switch (parameter)
            {
                case 0:
                    {
                        // 当前光标处到结尾
                        // 获取光标所在文本

                        VTextLine cursorLine = this.GetCursorTextLine();
                        if (cursorLine == null)
                        {
                            logger.ErrorFormat("获取光标所在VTextLine失败, cursorRow = {0}", this.cursorRow);
                            return;
                        }

                        // 获取光标所在TextBlock和之后的所有textBlock
                        List<VTextBlock> textBlocks = cursorLine.GetTextBlockAfter(this.cursorCol);

                        VTextBlock textBlockOverCursor = textBlocks.FirstOrDefault();
                        if (textBlockOverCursor != null)
                        {
                            // 先删除文本
                            int startIndex = this.cursorCol - textBlockOverCursor.Column;
                            int count = textBlockOverCursor.Columns - startIndex;
                            textBlockOverCursor.DeleteText(startIndex, count);

                            // 删除剩余的文本块
                            if (textBlockOverCursor.Columns == 0)
                            {
                                // 文本块内容被删完了，直接删除文本块
                                cursorLine.DeleteText(textBlocks);
                                this.uiSyncContext.Send((v) =>
                                {
                                    this.PresentationDevice.DeleteText(textBlocks);
                                }, null);
                            }
                            else
                            {
                                textBlocks.RemoveAt(0);
                                cursorLine.DeleteText(textBlocks);
                                this.uiSyncContext.Send((v) =>
                                {
                                    this.PresentationDevice.DrawText(textBlockOverCursor);
                                    this.PresentationDevice.DeleteText(textBlocks);
                                }, null);
                            }
                        }
                        break;
                    }

                case 1:
                    {
                        // 删除从行首到当前光标处的内容
                        VTextLine cursorLine = this.GetCursorTextLine();
                        if (cursorLine == null)
                        {
                            logger.ErrorFormat("获取光标所在VTextLine失败, cursorRow = {0}", this.cursorRow);
                            return;
                        }

                        List<VTextBlock> textBlocks = cursorLine.GetTextBlockBefore(this.cursorCol);

                        VTextBlock textBlockOverCursor = textBlocks.LastOrDefault();
                        if (textBlockOverCursor != null)
                        {
                            int startIndex = 0;
                            int count = this.cursorCol - textBlockOverCursor.Column;
                            textBlockOverCursor.DeleteText(startIndex, count);

                            if (textBlockOverCursor.Columns == 0)
                            {
                                // 文本块内容被删完了，直接删除文本块
                                cursorLine.DeleteText(textBlocks);
                                this.uiSyncContext.Send((v) =>
                                {
                                    this.PresentationDevice.DeleteText(textBlocks);
                                }, null);
                            }
                            else
                            {
                                textBlocks.RemoveAt(textBlocks.Count - 1);
                                cursorLine.DeleteText(textBlocks);
                                this.uiSyncContext.Send((v) =>
                                {
                                    this.PresentationDevice.DrawText(textBlockOverCursor);
                                    this.PresentationDevice.DeleteText(textBlocks);
                                }, null);
                            }
                        }

                        break;
                    }

                case 2:
                    {
                        // 删除光标所在整行
                        VTextLine toDelete = this.GetCursorTextLine();
                        List<VTextBlock> textBlocks = toDelete.GetAllText();
                        toDelete.DeleteText(textBlocks);

                        this.uiSyncContext.Send((v) =>
                        {
                            this.PresentationDevice.DeleteText(textBlocks);
                        }, null);
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 把相对于一行的偏移转换成相对于TextBlock的偏移
        /// </summary>
        /// <param name="columnInLine"></param>
        /// <param name="relativeTo"></param>
        /// <returns></returns>
        private int TranslateColumn(int columnInLine, VTextBlock relativeTo)
        {
            return columnInLine - relativeTo.Column;
        }

        /// <summary>
        /// 从当前光标处开始删除字符
        /// </summary>
        /// <param name="n">要删除的字符数</param>
        private void PerformDeleteCharacters(int n)
        {
            // 获取光标所在文本块
            VTextBlock cursorText = this.GetCursorTextBlock();
            if (cursorText == null)
            {
                logger.WarnFormat("PerformDeleteCharacters, 获取光标所在TextBlock失败, cursorCol = {0}", this.cursorCol);
                return;
            }

            VTextLine cursorLine = cursorText.OwnerLine;

            // 剩余要删除的字符数
            int left = n;

            // 计算左对齐基准文本块
            VTextBlock baseText = null;
            VTextBlock baseText2 = null;

            int startIndex = this.TranslateColumn(this.cursorCol, cursorText);
            if (cursorText.Columns - startIndex >= left)
            {
                #region 可以一次性删完

                cursorText.DeleteText(startIndex, left);

                #endregion
            }
            else
            {
                #region 一次性删不完

                VTextBlock currentText = cursorText;

                while (left > 0)
                {
                    if (currentText.Columns > left)
                    {
                        // 可以一次性删完
                        currentText.DeleteText(0, left);
                        left = 0;
                        baseText2 = currentText;
                    }
                    else if (currentText.Columns == left)
                    {
                        currentText.DeleteText(0, left);
                        left -= currentText.Columns;
                    }
                    else
                    {
                        currentText.DeleteText(0);
                        left -= currentText.Columns;
                    }

                    currentText = currentText.Next;
                }

                #endregion
            }

            #region 计算左对齐第一个文本块

            if (cursorText.IsEmpty())
            {
                if (cursorText.Previous != null)
                {
                    baseText = cursorText.Previous;
                }
                else
                {
                    baseText = null;
                }
            }
            else
            {
                baseText = cursorText;
            }

            #endregion

            // 删除空的文本块
            cursorLine.DeleteEmpty();

            // 测量左对齐基准文本块
            this.PresentationDevice.MeasureText(baseText);

            // 使该行左对齐
            cursorLine.LeftAlignment();
        }

        #endregion

        #region 事件处理器

        /// <summary>
        /// 当用户按下按键的时候触发
        /// </summary>
        /// <param name="terminal"></param>
        private void VideoTerminal_InputEvent(IInputDevice terminal, VTInputEvent evt)
        {
            // todo:translate and send to remote host
            if (string.IsNullOrEmpty(evt.Text))
            {
                {
                    // 这里输入的都是键盘按键
                    byte[] bytes = this.Keyboard.TranslateInput(evt);
                    if (bytes == null)
                    {
                        return;
                    }

                    this.vtChannel.Write(bytes);
                }
            }
            else
            {
                // 这里输入的都是中文
            }
        }

        private void VtParser_ActionEvent(VTActions action, params object[] param)
        {
            switch (action)
            {
                case VTActions.Print:
                    {
                        char ch = param[0].ToString()[0];

                        switch (ch)
                        {
                            // 遇到下面的字符，渲染完后就重新创建TextBlock
                            //case ':':
                            //case '/':
                            //case '\\':
                            case ' ':
                                {
                                    //Console.WriteLine("渲染断字符, {0}", ch);
                                    this.activeTextBlock = this.CreateTextBlock(this.cursorRow, this.cursorCol, this.textOffsetX);
                                    this.activeTextBlock.InsertText(ch);
                                    this.uiSyncContext.Send((v) =>
                                    {
                                        this.PresentationDevice.DrawText(this.activeTextBlock);
                                    }, null);
                                    this.InvalidateMeasure();
                                    // 下次新创建的TextBlock的X偏移量
                                    this.textOffsetX = this.activeTextBlock.Boundary.RightTop.X;
                                    this.cursorCol++;
                                    this.activeTextBlock = null;
                                    break;
                                }

                            default:
                                {
                                    if (this.activeTextBlock == null)
                                    {
                                        this.activeTextBlock = this.CreateTextBlock(this.cursorRow, this.cursorCol, this.textOffsetX);
                                    }
                                    this.activeTextBlock.InsertText(ch);
                                    this.uiSyncContext.Send((v) =>
                                    {
                                        this.PresentationDevice.DrawText(this.activeTextBlock);
                                    }, null);
                                    this.InvalidateMeasure();
                                    // 下次新创建的TextBlock的X偏移量
                                    this.textOffsetX = this.activeTextBlock.Boundary.RightTop.X;
                                    this.cursorCol++;
                                    break;
                                }
                        }

                        break;
                    }

                case VTActions.CarriageReturn:
                    {
                        // CR
                        // 把光标移动到行开头
                        logger.DebugFormat("CR");
                        this.cursorCol = 0;
                        this.textOffsetX = 0;
                        break;
                    }

                case VTActions.LineFeed:
                    {
                        // LF
                        logger.DebugFormat("LF");
                        //this.textOffsetX = 0;
                        //this.cursorCol = 0;
                        this.cursorRow++;
                        this.activeTextBlock = this.CreateTextBlock(this.cursorRow, 0, 0);
                        break;
                    }

                case VTActions.EraseLine:
                    {
                        logger.DebugFormat("EraseLine");
                        this.PerformEraseLine(Convert.ToInt32(param[0]));
                        break;
                    }

                case VTActions.CursorBackward:
                    {
                        logger.DebugFormat("CursorBackward");
                        this.cursorCol--;
                        break;
                    }

                case VTActions.PlayBell:
                case VTActions.Bold:
                case VTActions.Foreground:
                case VTActions.Background:
                case VTActions.DefaultAttributes:
                case VTActions.DefaultBackground:
                case VTActions.DefaultForeground:
                    break;

                case VTActions.SetVTMode:
                    {
                        logger.DebugFormat("SetMode");
                        VTMode vtMode = (VTMode)param[0];
                        this.Keyboard.SetAnsiMode(vtMode == VTMode.AnsiMode);
                        break;
                    }

                case VTActions.SetCursorKeyMode:
                    {
                        logger.DebugFormat("SetCursorKeyMode");
                        VTCursorKeyMode cursorKeyMode = (VTCursorKeyMode)param[0];
                        this.Keyboard.SetCursorKeyMode(cursorKeyMode == VTCursorKeyMode.ApplicationMode);
                        break;
                    }

                case VTActions.SetKeypadMode:
                    {
                        logger.DebugFormat("SetKeypadMode");
                        VTKeypadMode keypadMode = (VTKeypadMode)param[0];
                        this.Keyboard.SetKeypadMode(keypadMode == VTKeypadMode.ApplicationMode);
                        break;
                    }

                case VTActions.DeleteCharacters:
                    {
                        logger.DebugFormat("DeleteCharacters");
                        this.PerformDeleteCharacters(Convert.ToInt32(param[0]));
                        break;
                    }

                default:
                    {
                        logger.WarnFormat("未执行的VTAction, {0}", action);
                        break;
                    }
            }
        }

        private void VTChannel_DataReceived(VTChannel client, byte[] bytes)
        {
            //string str = string.Join(",", bytes.Select(v => v.ToString()).ToList());
            //logger.InfoFormat("Received, {0}", str);
            this.vtParser.ProcessCharacters(bytes);
        }

        private void VTChannel_StatusChanged(object client, VTChannelState state)
        {
            logger.InfoFormat("客户端状态发生改变, {0}", state);
        }

        #endregion
    }
}
