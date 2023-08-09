﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using XTerminal.Base;
using XTerminal.Base.DataModels;
using XTerminal.Base.DataModels.Session;
using XTerminal.Document;
using XTerminal.Document.Rendering;
using XTerminal.Parser;
using XTerminal.Rendering;
using XTerminal.Session;

namespace XTerminal
{
    /// <summary>
    /// 处理虚拟终端的所有逻辑
    /// </summary>
    public class VideoTerminal
    {
        private enum OutsideScrollResult
        {
            /// <summary>
            /// 没滚动
            /// </summary>
            None,

            /// <summary>
            /// 鼠标往上滚动
            /// </summary>
            ScrollTop,

            /// <summary>
            /// 鼠标往下滚动
            /// </summary>
            ScrollDown
        }

        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("VideoTerminal");

        private static readonly byte[] OS_OperationStatusResponse = new byte[4] { (byte)'\x1b', (byte)'[', (byte)'0', (byte)'n' };
        private static readonly byte[] CPR_CursorPositionReportResponse = new byte[7] { 0x1b, (byte)'[', (byte)'?', (byte)'0', (byte)';', (byte)'0', (byte)'R' };
        //private static readonly byte[] CPR_CursorPositionReportResponse = new byte[5] { (byte)'\x1b', (byte)'[', (byte)'0', (byte)';', (byte)'0' };
        private static readonly byte[] DA_DeviceAttributesResponse = new byte[7] { 0x1b, (byte)'[', (byte)'?', (byte)'1', (byte)':', (byte)'0', (byte)'c' };

        #endregion

        #region 公开事件

        public event Action<VideoTerminal, SessionStatusEnum> SessionStatusChanged;

        #endregion

        #region 实例变量

        /// <summary>
        /// 与终端进行通信的信道
        /// </summary>
        private SessionTransport sessionTransport;

        /// <summary>
        /// 终端字符解析器
        /// </summary>
        private VTParser vtParser;

        /// <summary>
        /// 主缓冲区文档模型
        /// </summary>
        private VTDocument mainDocument;

        /// <summary>
        /// 备用缓冲区文档模型
        /// </summary>
        private VTDocument alternateDocument;

        /// <summary>
        /// 当前正在使用的文档模型
        /// </summary>
        private VTDocument activeDocument;

        /// <summary>
        /// UI线程上下文
        /// </summary>
        private SynchronizationContext uiSyncContext;

        private XTermSession sessionInfo;
        private MouseOptions mouseOptions;

        /// <summary>
        /// DECAWM是否启用
        /// </summary>
        private bool autoWrapMode;

        private bool xtermBracketedPasteMode;

        /// <summary>
        /// 闪烁光标的线程
        /// </summary>
        private Thread cursorBlinkingThread;

        #region Terminal Info

        /// <summary>
        /// 当前终端可以显示的总行数
        /// </summary>
        private int terminalRows;

        /// <summary>
        /// 当前终端可以显示的总列数
        /// </summary>
        private int terminalColumns;

        #endregion

        #region History & Scroll

        /// <summary>
        /// 存储所有的历史行
        /// Row(scrollValue) -> VTextLine
        /// 注意该字典里保存的是mainDocument的历史行，alternateDocument没有保存，需要单独考虑alternateDocument
        /// </summary>
        private Dictionary<int, VTHistoryLine> historyLines;

        /// <summary>
        /// 历史行的第一行
        /// </summary>
        private VTHistoryLine firstHistoryLine;

        /// <summary>
        /// 历史行的最后一行
        /// </summary>
        private VTHistoryLine lastHistoryLine;

        /// <summary>
        /// 记录滚动条滚动到底的时候，滚动条的值
        /// </summary>
        private int scrollMax;

        /// <summary>
        /// 记录当前滚动条滚动的值
        /// 也就是当前Surface上渲染的第一行的PhysicsRow
        /// 默认值是0
        /// </summary>
        private int scrollValue;

        /// <summary>
        /// surface相对于屏幕的坐标
        /// </summary>
        private VTRect surfaceRect;

        #endregion

        #region SelectionRange

        /// <summary>
        /// 鼠标是否按下
        /// </summary>
        private bool isMouseDown;
        private VTPoint mouseDownPos;

        /// <summary>
        /// 当前鼠标是否处于Selection状态
        /// </summary>
        private bool selectionState;

        /// <summary>
        /// 存储选中的文本信息
        /// </summary>
        private VTextSelection textSelection;

        #endregion

        private int renderCounter;
        private int dataReceivedCounter;

        /// <summary>
        /// 是否正在运行
        /// </summary>
        private bool isRunning;

        /// <summary>
        /// 输入编码方式
        /// </summary>
        private Encoding inputEncoding;

        #endregion

        #region 属性

        /// <summary>
        /// activeDocument的光标信息
        /// 该坐标是基于ViewableDocument的坐标
        /// </summary>
        private VTCursor Cursor { get { return this.activeDocument.Cursor; } }

        /// <summary>
        /// 获取当前光标所在行
        /// </summary>
        public VTextLine ActiveLine { get { return this.activeDocument.ActiveLine; } }

        /// <summary>
        /// 获取当前选中的行
        /// </summary>
        public VTextSelection Selection { get { return this.textSelection; } }

        /// <summary>
        /// 获取当前光标所在行
        /// </summary>
        public int CursorRow { get { return this.Cursor.Row; } }

        /// <summary>
        /// 获取当前光标所在列
        /// 下一个字符要显示的位置
        /// </summary>
        public int CursorCol { get { return this.Cursor.Column; } }

        /// <summary>
        /// 渲染终端输出的画布
        /// </summary>
        public ITerminalSurface Surface { get { return this.activeDocument.Surface; } }

        /// <summary>
        /// 文档画布容器
        /// </summary>
        public ITerminalScreen TerminalScreen { get; set; }

        /// <summary>
        /// 根据当前电脑键盘的按键状态，转换成标准的ANSI控制序列
        /// </summary>
        public VTKeyboard Keyboard { get; private set; }

        public VTextOptions TextOptions { get; private set; }

        /// <summary>
        /// 获取当前滚动条是否滚动到底了
        /// </summary>
        public bool ScrollAtBottom
        {
            get
            {
                return this.scrollValue == this.scrollMax;
            }
        }

        /// <summary>
        /// 获取当前滚动条是否滚动到顶了
        /// </summary>
        public bool ScrollAtTop
        {
            get
            {
                return this.scrollValue == 0;
            }
        }

        #endregion

        #region 构造方法

        public VideoTerminal()
        {
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 初始化终端模拟器
        /// </summary>
        /// <param name="sessionInfo"></param>
        public void Initialize(XTermSession sessionInfo)
        {
            this.sessionInfo = sessionInfo;
            this.mouseOptions = sessionInfo.MouseOptions;
            this.inputEncoding = Encoding.GetEncoding(sessionInfo.InputEncoding);
            this.uiSyncContext = SynchronizationContext.Current;

            // DECAWM
            this.autoWrapMode = sessionInfo.TerminalOptions.DECPrivateAutoWrapMode;

            // 初始化变量
            this.historyLines = new Dictionary<int, VTHistoryLine>();
            this.TextOptions = new VTextOptions();
            this.textSelection = new VTextSelection();

            this.isRunning = true;

            this.terminalRows = sessionInfo.TerminalOptions.Rows;
            this.terminalColumns = sessionInfo.TerminalOptions.Columns;

            #region 初始化键盘

            this.Keyboard = new VTKeyboard();
            this.Keyboard.Encoding = Encoding.GetEncoding(sessionInfo.InputEncoding);
            this.Keyboard.SetAnsiMode(true);
            this.Keyboard.SetKeypadMode(false);

            #endregion

            #region 注册事件

            this.TerminalScreen.InputEvent += this.OnInputEvent;
            this.TerminalScreen.ScrollChanged += this.OnScrollChanged;
            this.TerminalScreen.VTMouseDown += this.OnMouseDown;
            this.TerminalScreen.VTMouseMove += this.OnMouseMove;
            this.TerminalScreen.VTMouseUp += this.OnMouseUp;
            this.TerminalScreen.VTMouseWheel += this.OnMouseWheel;

            #endregion

            #region 初始化终端解析器

            this.vtParser = new VTParser();
            this.vtParser.ActionEvent += VtParser_ActionEvent;
            this.vtParser.Initialize();

            #endregion

            #region 初始化文档模型

            VTDocumentOptions documentOptions = new VTDocumentOptions()
            {
                ColumnSize = this.sessionInfo.TerminalOptions.Columns,
                RowSize = this.sessionInfo.TerminalOptions.Rows,
                DECPrivateAutoWrapMode = this.sessionInfo.TerminalOptions.DECPrivateAutoWrapMode,
                CursorStyle = this.sessionInfo.MouseOptions.CursorStyle,
                Interval = this.sessionInfo.MouseOptions.CursorInterval,
                FontFamily = sessionInfo.AppearanceOptions.FontFamily,
                FontSize = sessionInfo.AppearanceOptions.FontSize,
                Foreground = sessionInfo.AppearanceOptions.Foreground
            };
            this.mainDocument = new VTDocument(documentOptions) { Name = "MainDocument", Surface = this.TerminalScreen.CreateSurface() };
            this.alternateDocument = new VTDocument(documentOptions) { Name = "AlternateDocument", Surface = this.TerminalScreen.CreateSurface() };
            this.activeDocument = this.mainDocument;
            this.firstHistoryLine = VTHistoryLine.Create(0, null, this.ActiveLine);
            this.historyLines[0] = this.firstHistoryLine;
            this.lastHistoryLine = this.firstHistoryLine;
            this.TerminalScreen.SwitchSurface(null, this.activeDocument.Surface);

            #endregion

            #region 初始化光标

            this.Surface.Draw(this.Cursor);
            this.cursorBlinkingThread = new Thread(this.CursorBlinkingThreadProc);
            this.cursorBlinkingThread.IsBackground = true;
            this.cursorBlinkingThread.Start();
            // 先初始化备用缓冲区的光标渲染上下文
            this.alternateDocument.Surface.Draw(this.alternateDocument.Cursor);

            #endregion

            #region 连接终端通道

            SessionTransport transport = new SessionTransport();
            transport.StatusChanged += this.VTSession_StatusChanged;
            transport.DataReceived += this.VTSession_DataReceived;
            transport.Initialize(sessionInfo);
            transport.Open();
            this.sessionTransport = transport;

            #endregion
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Release()
        {
            this.isRunning = false;

            this.TerminalScreen.InputEvent -= this.OnInputEvent;
            this.TerminalScreen.ScrollChanged -= this.OnScrollChanged;
            this.TerminalScreen.VTMouseDown -= this.OnMouseDown;
            this.TerminalScreen.VTMouseMove -= this.OnMouseMove;
            this.TerminalScreen.VTMouseUp -= this.OnMouseUp;
            this.TerminalScreen.VTMouseWheel -= this.OnMouseWheel;

            this.vtParser.ActionEvent -= VtParser_ActionEvent;
            this.vtParser.Release();

            this.cursorBlinkingThread.Join();

            this.sessionTransport.StatusChanged -= this.VTSession_StatusChanged;
            this.sessionTransport.DataReceived -= this.VTSession_DataReceived;
            this.sessionTransport.Close();
            this.sessionTransport.Release();

            this.mainDocument.Dispose();
            this.alternateDocument.Dispose();

            this.historyLines.Clear();
            this.firstHistoryLine = null;
            this.lastHistoryLine = null;
        }

        /// <summary>
        /// 复制当前选中的行
        /// </summary>
        public void CopySelection()
        {
            if (this.textSelection.IsEmpty)
            {
                logger.WarnFormat("CopySelection失败, 起始字符索引或者结束字符索引小于0");
                return;
            }

            string text = this.textSelection.GetText(this.historyLines);

            // 调用剪贴板API复制到剪贴板
            Clipboard.SetText(text);
        }

        /// <summary>
        /// 将剪贴板的数据发送给会话
        /// </summary>
        /// <returns>发送成功返回SUCCESS，失败返回错误码</returns>
        public int Paste()
        {
            string text = Clipboard.GetText();
            if (string.IsNullOrEmpty(text))
            {
                return ResponseCode.SUCCESS;
            }

            byte[] bytes = this.inputEncoding.GetBytes(text);

            int code = this.sessionTransport.Write(bytes);
            if (code != ResponseCode.SUCCESS)
            {
                logger.ErrorFormat("粘贴数据失败, {0}", code);
            }

            return code;
        }

        /// <summary>
        /// 选中全部的文本
        /// </summary>
        public void SelectAll()
        {
            this.textSelection.Reset();

            if (this.firstHistoryLine == null || this.lastHistoryLine == null)
            {
                logger.WarnFormat("SelectAll失败, 历史记录里的第一行或者最后一行为空");
                return;
            }

            //this.textSelection.Start.LineHit = this.firstHistoryLine;
            //this.textSelection.Start.CharacterIndex = 0;

            //this.textSelection.End.LineHit = this.lastHistoryLine;
            //this.textSelection.End.CharacterIndex = this.lastHistoryLine.Text.Length - 1;
        }

        /// <summary>
        /// 保存成HTML文档
        /// 带有颜色的
        /// </summary>
        /// <param name="filePath"></param>
        public void SaveAsHtml(string filePath)
        {

        }

        /// <summary>
        /// 保存成普通文本文档
        /// </summary>
        /// <param name="filePath"></param>
        public void SaveAsText(string filePath)
        {
            string text = this.textSelection.GetText(this.historyLines);
            File.WriteAllText(filePath, text);
        }

        #endregion

        #region 实例方法

        private void PerformDeviceStatusReport(StatusType statusType)
        {
            switch (statusType)
            {
                case StatusType.OS_OperatingStatus:
                    {
                        // Result ("OK") is CSI 0 n
                        this.sessionTransport.Write(OS_OperationStatusResponse);
                        break;
                    }

                case StatusType.CPR_CursorPositionReport:
                    {
                        // 打开VIM后会收到这个请求

                        // Result is CSI ? r ; c R
                        int cursorRow = this.CursorRow;
                        int cursorCol = this.CursorCol;
                        CPR_CursorPositionReportResponse[3] = (byte)cursorRow;
                        CPR_CursorPositionReportResponse[5] = (byte)cursorCol;
                        this.sessionTransport.Write(CPR_CursorPositionReportResponse);
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 如果需要布局则进行布局
        /// 如果不需要布局，那么就看是否需要重绘某些文本行
        /// </summary>
        /// <param name="document">要渲染的文档</param>
        /// <param name="scrollValue">
        /// 是否要移动滚动条，设置为-1表示不移动滚动条
        /// 注意这里只是更新UI上的滚动条位置，并不会实际的去滚动内容
        /// </param>
        private void DrawDocument(VTDocument document, int scrollValue = -1)
        {
            // 当前行的Y方向偏移量
            double offsetY = 0;

            bool arrangeDirty = document.IsArrangeDirty;

            this.uiSyncContext.Send((state) =>
            {
                #region 渲染文档

                VTextLine next = document.FirstLine;

                while (next != null)
                {
                    // 更新Y偏移量信息
                    next.OffsetY = offsetY;

                    if (next.IsRenderDirty)
                    {
                        // 此时说明该行有字符变化，需要重绘
                        this.Surface.Draw(next);
                        //logger.ErrorFormat("renderCounter = {0}", this.renderCounter++);
                    }
                    else if (next.IsMeasureDirty)
                    {
                        // 字符没有变化，那么只重新测量然后更新一下文本的偏移量就好了
                        this.Surface.MeasureLine(next);
                    }

                    // 更新行偏移量
                    if (arrangeDirty)
                    {
                        this.Surface.Arrange(next);
                    }

                    // 更新下一个文本行的Y偏移量
                    offsetY += next.Height;

                    // 如果最后一行渲染完毕了，那么就退出
                    if (next == document.LastLine)
                    {
                        break;
                    }

                    next = next.NextLine;
                }

                #endregion

                #region 渲染光标

                int characterIndex = this.ActiveLine.FindCharacterIndex(this.CursorCol - 1);
                if (characterIndex >= 0)
                {
                    VTRect rect = this.Surface.MeasureLine(this.ActiveLine, characterIndex, 1);
                    this.Cursor.OffsetY = this.ActiveLine.OffsetY;
                    this.Cursor.OffsetX = rect.Right;
                    this.Surface.Arrange(this.Cursor);
                }

                #endregion

                #region 移动滚动条

                if (scrollValue != -1)
                {
                    this.TerminalScreen.ScrollTo(scrollValue);
                }

                #endregion

                #region 更新选中高亮几何图形

                if (this.textSelection.IsRenderDirty)
                {
                    // 此时的VTextLine测量数据都是最新的
                    this.UpdateSelectionGeometry(this.activeDocument, this.textSelection);
                    this.Surface.Draw(this.textSelection);
                    this.textSelection.SetRenderDirty(false);
                }

                #endregion

            }, null);

            if (this.activeDocument == this.mainDocument)
            {
                if (this.ScrollAtBottom)
                {
                    // 滚动到底了，说明是ActiveLine就是当前正在输入的行
                    // 更新下历史行的大小和文字，不然在执行光标选中的时候文本为空，会影响到测量
                    this.lastHistoryLine.SetVTextLine(this.ActiveLine);
                }
            }

            document.SetArrangeDirty(false);
        }

        /// <summary>
        /// 滚动到指定的历史记录
        /// 并更新UI上的滚动条位置
        /// </summary>
        /// <param name="scrollValue">要显示的第一行历史记录</param>
        private void ScrollToHistory(int scrollValue)
        {
            // 要滚动到的值
            int newScroll = scrollValue;
            // 滚动之前的值
            int oldScroll = this.scrollValue;

            // 更新当前滚动条的值，一定要先更新，因为DrawDocument函数会用到该值
            this.scrollValue = scrollValue;

            // 需要进行滚动的行数
            int scrolledRows = Math.Abs(newScroll - oldScroll);

            #region 更新要显示的行

            if (scrolledRows >= terminalRows)
            {
                // 先找到Surface上要显示的第一行数据
                VTHistoryLine historyLine;
                if (!this.historyLines.TryGetValue(scrollValue, out historyLine))
                {
                    logger.ErrorFormat("ScrollTo失败, 找不到对应的VTHistoryLine, scrollValue = {0}", scrollValue);
                    return;
                }

                // 此时说明把所有行都滚动到Surface外了，需要重新显示所有行
                // 找到后面的行数显示
                VTHistoryLine currentHistory = historyLine;
                VTextLine currentTextLine = this.activeDocument.FirstLine;
                for (int i = 0; i < terminalRows; i++)
                {
                    // 直接使用VTHistoryLine的List<VTCharacter>的引用
                    // 冻结状态下的VTextLine不会再有修改了
                    // 非冻结状态(ActiveLine)需要重新创建一个集合
                    currentTextLine.SetHistory(currentHistory);
                    currentHistory = currentHistory.NextLine;
                    currentTextLine = currentTextLine.NextLine;
                }
            }
            else
            {
                // 算出来移动前的第一行/最后一行与移动后的第一行/最后一行的差值
                // 这个差值就是TextSelection.Geometry的OffsetY
                VTextLine line1 = null;
                VTextLine line2 = null;

                // 此时说明只需要更新移动出去的行就可以了
                if (newScroll > oldScroll)
                {
                    line1 = this.activeDocument.FirstLine;

                    // 往下滚动，把上面的拿到下面，从第一行开始
                    for (int i = 0; i < scrolledRows; i++)
                    {
                        VTHistoryLine historyLine = this.historyLines[oldScroll + terminalRows + i];

                        // 该值永远是第一行，因为下面被Move到最后一行了
                        VTextLine firstLine = this.activeDocument.FirstLine;

                        firstLine.Move(VTextLine.MoveOptions.MoveToLast);

                        firstLine.SetHistory(historyLine);
                    }

                    line2 = this.activeDocument.FirstLine;
                }
                else
                {
                    line1 = this.activeDocument.LastLine;

                    // 往上滚动，把下面的拿到上面，从最后一行开始
                    for (int i = 1; i <= scrolledRows; i++)
                    {
                        VTHistoryLine historyLine = this.historyLines[oldScroll - i];

                        VTextLine lastLine = this.activeDocument.LastLine;

                        lastLine.Move(VTextLine.MoveOptions.MoveToFirst);

                        lastLine.SetHistory(historyLine);
                    }

                    line2 = this.activeDocument.LastLine;
                }
            }

            this.activeDocument.SetArrangeDirty(true);

            #endregion

            #region 如果有TextSelection，那么更新TextSelection

            // 只有当前是主缓冲区才支持用鼠标滚轮滚动
            // 备用缓冲区不支持，也就是说VIM，man这种程序不支持用鼠标滚轮
            if (this.activeDocument == this.mainDocument)
            {
                if (!this.textSelection.IsEmpty)
                {
                    // 此时的VTextLine测量数据都是最新的
                    this.UpdateSelectionGeometry(this.activeDocument, this.textSelection);
                    this.textSelection.SetRenderDirty(true);
                }
            }

            #endregion

            #region 重新渲染

            this.DrawDocument(this.activeDocument, scrollValue);

            #endregion
        }

        /// <summary>
        /// 当光标在容器外面移动的时候，进行滚动
        /// </summary>
        /// <param name="mousePosition">当前鼠标的坐标</param>
        /// <param name="surfaceBoundary">相对于电脑显示器的画布的边界框</param>
        /// <returns>是否执行了滚动动作</returns>
        private OutsideScrollResult ScrollIfCursorOutsideSurface(VTPoint mousePosition, VTRect surfaceBoundary)
        {
            OutsideScrollResult scrollResult = OutsideScrollResult.None;

            // 要滚动到的目标行
            int scrollTarget = -1;

            if (mousePosition.Y < 0)
            {
                // 光标在容器上面
                if (!this.ScrollAtTop)
                {
                    // 不在最上面，往上滚动一行
                    scrollTarget = this.scrollValue - 1;
                    scrollResult = OutsideScrollResult.ScrollTop;
                }
            }
            else if (mousePosition.Y > surfaceBoundary.Height)
            {
                // 光标在容器下面
                if (!this.ScrollAtBottom)
                {
                    scrollTarget = this.scrollValue + 1;
                    scrollResult = OutsideScrollResult.ScrollDown;
                }
            }

            if (scrollTarget != -1)
            {
                this.ScrollToHistory(scrollTarget);
            }

            return scrollResult;
        }

        /// <summary>
        /// 使用像素坐标对VTextLine做命中测试
        /// </summary>
        /// <param name="mousePosition">鼠标坐标</param>
        /// <param name="surfaceBoundary">相对于电脑显示器的画布的边界框，也是鼠标的限定范围</param>
        /// <param name="pointer">存储命中测试结果的变量</param>
        /// <remarks>如果传递进来的鼠标位置在窗口外，那么会把鼠标限定在距离鼠标最近的Surface边缘处</remarks>
        /// <returns>
        /// 是否获取成功
        /// 当光标不在某一行或者不在某个字符上的时候，就获取失败
        /// </returns>
        private bool GetTextPointer(VTPoint mousePosition, VTRect surfaceBoundary, VTextPointer pointer)
        {
            double mouseX = mousePosition.X;
            double mouseY = mousePosition.Y;

            if (mouseX < 0)
            {
                mouseX = 0;
            }
            if (mouseX > surfaceBoundary.Width)
            {
                mouseX = surfaceBoundary.Width;
            }

            if (mouseY < 0)
            {
                mouseY = 0;
            }
            if (mouseY > surfaceBoundary.Height)
            {
                mouseY = surfaceBoundary.Height;
            }

            #region 先找到鼠标所在行

            VTextLine cursorLine = VTextSelectionHelper.HitTestVTextLine(this.activeDocument.FirstLine, mouseY);
            if (cursorLine == null)
            {
                // 这里说明鼠标没有在任何一行上
                logger.DebugFormat("没有找到鼠标位置对应的行, cursorY = {0}", mouseY);
                return false;
            }

            #endregion

            #region 再计算鼠标悬浮于哪个字符上

            int characterIndex;
            VTRect characterBounds;
            if (!VTextSelectionHelper.HitTestVTCharacter(this.Surface, cursorLine, mouseX, out characterIndex, out characterBounds))
            {
                return false;
            }

            #endregion

            // 命中成功再更新TextPointer，保证pointer不为空
            pointer.PhysicsRow = cursorLine.PhysicsRow;
            pointer.CharacterIndex = characterIndex;

            return true;
        }

        /// <summary>
        /// 自动判断ch是多字节字符还是单字节字符，创建一个VTCharacter
        /// </summary>
        /// <param name="ch">多字节或者单字节的字符</param>
        /// <returns></returns>
        private VTCharacter CreateCharacter(object ch)
        {
            if (ch is char)
            {
                return VTCharacter.Create(Convert.ToChar(ch), 2, VTCharacterFlags.MulitByteChar);
            }
            else
            {
                return VTCharacter.Create(Convert.ToChar(ch), 1, VTCharacterFlags.SingleByteChar);
            }
        }

        /// <summary>
        /// 根据当前滚动条的状态和选中状态重新构建高亮几何图形
        /// </summary>
        /// <param name="document">当前显示的文档</param>
        /// <param name="selection">鼠标命中信息</param>
        private void UpdateSelectionGeometry(VTDocument document, VTextSelection selection)
        {
            selection.Geometry.Clear();

            // 单独处理选中的是同一行的情况
            if (selection.Start.PhysicsRow == selection.End.PhysicsRow)
            {
                // 找到对应的文本行
                VTextLine textLine = document.FindLine(selection.Start.PhysicsRow);
                if (textLine == null)
                {
                    // 当选中了一行之后，然后该行被移动到屏幕外了，会出现这种情况
                    return;
                }

                VTextPointer leftPointer = selection.Start.CharacterIndex < selection.End.CharacterIndex ? selection.Start : selection.End;
                VTextPointer rightPointer = selection.Start.CharacterIndex < selection.End.CharacterIndex ? selection.End : selection.Start;

                VTRect leftBounds = this.Surface.MeasureCharacter(textLine, leftPointer.CharacterIndex);
                VTRect rightBounds = this.Surface.MeasureCharacter(textLine, rightPointer.CharacterIndex);

                double x = leftBounds.Left;
                double y = textLine.OffsetY;
                double width = rightBounds.Right - leftBounds.Left;
                double height = textLine.Height;

                VTRect bounds = new VTRect(x, y, width, height);
                selection.Geometry.Add(bounds);
                return;
            }

            // 下面处理选中了多行的状态
            VTextPointer topPointer = selection.Start.PhysicsRow > selection.End.PhysicsRow ? selection.End : selection.Start;
            VTextPointer bottomPointer = selection.Start.PhysicsRow > selection.End.PhysicsRow ? selection.Start : selection.End;

            VTextLine topLine = document.FindLine(topPointer.PhysicsRow);
            VTextLine bottomLine = document.FindLine(bottomPointer.PhysicsRow);

            if (topLine != null && bottomLine != null)
            {
                // 此时说明选中的内容都在屏幕里
                // 构建上边和下边的矩形
                VTRect topBounds = this.Surface.MeasureCharacter(topLine, topPointer.CharacterIndex);
                VTRect bottomBounds = this.Surface.MeasureCharacter(bottomLine, bottomPointer.CharacterIndex);

                // 第一行的矩形
                selection.Geometry.Add(new VTRect(topBounds.X, topLine.OffsetY, this.surfaceRect.Width - topBounds.X, topLine.Height));

                // 中间的矩形
                double y = topLine.OffsetY + topBounds.Height;
                double height = bottomLine.OffsetY - (topLine.OffsetY + topBounds.Height);
                selection.Geometry.Add(new VTRect(0, y, this.surfaceRect.Width, height));

                // 最后一行的矩形
                selection.Geometry.Add(new VTRect(0, bottomLine.OffsetY, bottomBounds.Right, bottomLine.Height));
                return;
            }

            if (topLine != null && bottomLine == null)
            {
                // 选中的内容有一部分被移到屏幕外了，滚动条往上移动
                VTRect topBounds = this.Surface.MeasureCharacter(topLine, topPointer.CharacterIndex);

                // 第一行的矩形
                selection.Geometry.Add(new VTRect(topBounds.X, topLine.OffsetY, this.surfaceRect.Width - topBounds.X, topLine.Height));

                // 剩下的矩形
                double height = document.LastLine.Bounds.Bottom - topLine.Bounds.Bottom;
                selection.Geometry.Add(new VTRect(0, topLine.Bounds.Bottom, this.surfaceRect.Width, height));
                return;
            }

            if (topLine == null && bottomLine != null)
            {
                // 选中的内容有一部分被移到屏幕外了，滚动条往下移动
                VTRect bottomBounds = this.Surface.MeasureCharacter(bottomLine, bottomPointer.CharacterIndex);

                // 最后一行的矩形
                selection.Geometry.Add(new VTRect(0, bottomLine.OffsetY, bottomBounds.Right, bottomLine.Height));

                // 剩下的矩形
                selection.Geometry.Add(new VTRect(0, 0, this.surfaceRect.Width, bottomLine.OffsetY));
                return;
            }
        }

        #endregion

        #region 事件处理器

        /// <summary>
        /// 当用户按下按键的时候触发
        /// </summary>
        /// <param name="terminal"></param>
        private void OnInputEvent(ITerminalScreen canvasPanel, VTInputEvent evt)
        {
            byte[] bytes = this.Keyboard.TranslateInput(evt);
            if (bytes == null)
            {
                return;
            }

            // 这里输入的都是键盘按键
            int code = this.sessionTransport.Write(bytes);
            if (code != ResponseCode.SUCCESS)
            {
                logger.ErrorFormat("处理输入异常, {0}", ResponseCode.GetMessage(code));
            }
        }

        private void VtParser_ActionEvent(VTActions action, object parameter)
        {
            switch (action)
            {
                case VTActions.Print:
                    {
                        // 根据测试得出结论：
                        // 在VIM模式下输入中文字符，VIM会自动把光标往后移动2列，所以判断VIM里一个中文字符占用2列的宽度
                        // 在正常模式下，如果遇到中文字符，也使用2列来显示
                        // 也就是说，如果终端列数一共是80，那么可以显示40个中文字符，显示完40个中文字符后就要换行

                        // 如果在shell里删除一个中文字符，那么会执行两次光标向后移动的动作，然后EraseLine - ToEnd
                        // 由此可得出结论，不论是VIM还是shell，一个中文字符都是按照占用两列的空间来计算的

                        char ch = Convert.ToChar(parameter);
                        VTDebug.WriteAction("Print:{0}, Cursor={1},{2}", ch, this.CursorRow, this.CursorCol);
                        VTCharacter character = this.CreateCharacter(parameter);
                        this.activeDocument.PrintCharacter(this.ActiveLine, character, this.CursorCol);
                        this.activeDocument.SetCursor(this.CursorRow, this.CursorCol + character.ColumnSize);
                        break;
                    }

                case VTActions.CarriageReturn:
                    {
                        // CR
                        // 把光标移动到行开头
                        this.activeDocument.SetCursor(this.CursorRow, 0);
                        VTDebug.WriteAction("CarriageReturn, Cursor={0},{1}", this.CursorRow, this.CursorCol);
                        break;
                    }

                case VTActions.FF:
                case VTActions.VT:
                case VTActions.LF:
                    {
                        // LF
                        // 滚动边距会影响到LF（DECSTBM_SetScrollingRegion），在实现的时候要考虑到滚动边距

                        if (this.activeDocument == this.mainDocument)
                        {
                            // 如果滚动条不在最底部，那么先把滚动条滚动到底
                            if (!this.ScrollAtBottom)
                            {
                                this.ScrollToHistory(this.scrollMax);
                            }
                        }

                        // 想像一下有一个打印机往一张纸上打字，当打印机想移动到下一行打字的时候，它会发出一个LineFeed指令，让纸往上移动一行
                        // LineFeed，字面意思就是把纸上的下一行喂给打印机使用
                        this.activeDocument.LineFeed();

                        VTextLine oldLastLine = this.ActiveLine.PreviousLine;
                        VTextLine newLastLine = this.ActiveLine;
                        newLastLine.PhysicsRow = oldLastLine.PhysicsRow + 1;

                        // 换行之后记录历史行
                        // 注意用户可以输入Backspace键或者上下左右光标键来修改最新行的内容，所以最新一行的内容是实时变化的，目前的解决方案是在渲染整个文档的时候去更新最后一个历史行的数据
                        // MainScrrenBuffer和AlternateScrrenBuffer里的行分别记录
                        // AlternateScreenBuffer是用来给man，vim等程序使用的
                        // 暂时只记录主缓冲区里的数据，备用缓冲区需要考虑下怎么记录，因为VIM，Man等程序用的是备用缓冲区，用户是可以实时编辑缓冲区里的数据的
                        if (this.activeDocument == this.mainDocument)
                        {
                            // 可以确保换行之前的行已经被用户输入完了，不会被更改了，所以这里冻结一下换行之前的历史行的数据，冻结之后，该历史行的数据就不会再更改了
                            // 有几种特殊情况：
                            // 1. 如果主机一次性返回了多行数据，那么有可能前面的几行都没有测量，所以这里要先判断上一行是否有测量过

                            #region 更新当前的最后一个历史行

                            if (oldLastLine.IsMeasureDirty)
                            {
                                this.Surface.MeasureLine(oldLastLine);
                            }
                            this.lastHistoryLine.SetVTextLine(oldLastLine);

                            #endregion

                            #region 再创建新行对应的历史行

                            // 再创建最新行的历史行
                            // 先测量下最新的行，确保有高度
                            this.Surface.MeasureLine(this.ActiveLine);
                            int newHistoryRow = newLastLine.PhysicsRow;
                            VTHistoryLine newHistory = VTHistoryLine.Create(newLastLine.PhysicsRow, this.lastHistoryLine, this.ActiveLine);
                            this.historyLines[newHistoryRow] = newHistory;
                            this.lastHistoryLine = newHistory;

                            #endregion

                            #region 滚动滚动条

                            // 滚动条滚动到底
                            // 计算滚动条可以滚动的最大值
                            int scrollMax = newHistoryRow - this.terminalRows + 1;
                            if (scrollMax > 0)
                            {
                                // 更新滚动条的值
                                this.scrollMax = scrollMax;
                                this.scrollValue = scrollMax;

                                // 如果当前有选中的内容，那么把选中内容设置为脏状态，下次在渲染的时候会更新
                                // 选中的区域往上移动
                                if (!this.textSelection.IsEmpty)
                                {
                                    this.textSelection.SetRenderDirty(true);
                                }

                                logger.DebugFormat("scrollMax = {0}", scrollMax);
                                this.uiSyncContext.Send((state) =>
                                {
                                    this.TerminalScreen.UpdateScrollInfo(scrollMax);
                                    this.TerminalScreen.ScrollToEnd(ScrollOrientation.Down);
                                }, null);
                            }

                            #endregion
                        }

                        VTDebug.WriteAction("LF/FF/VT, Cursor={0},{1}, {2}", this.CursorRow, this.CursorCol, action);
                        break;
                    }

                case VTActions.RI_ReverseLineFeed:
                    {
                        // 和LineFeed相反，也就是把光标往上移一个位置
                        // 在用man命令的时候会触发这个指令
                        // 反向换行 – 执行\n的反向操作，将光标向上移动一行，维护水平位置，如有必要，滚动缓冲区 *
                        this.activeDocument.ReverseLineFeed();
                        VTDebug.WriteAction("ReverseLineFeed");
                        break;
                    }

                case VTActions.PlayBell:
                    {
                        // 响铃
                        break;
                    }

                #region Erase

                case VTActions.EL_EraseLine:
                    {
                        List<int> parameters = parameter as List<int>;
                        EraseType eraseType = (EraseType)VTParameter.GetParameter(parameters, 0, 0);
                        VTDebug.WriteAction("EL_EraseLine, eraseType = {0}, 从光标位置{1},{2}开始erase", eraseType, this.CursorRow, this.CursorCol);
                        this.activeDocument.EraseLine(this.ActiveLine, this.CursorCol, eraseType);
                        break;
                    }

                case VTActions.ED_EraseDisplay:
                    {
                        List<int> parameters = parameter as List<int>;
                        EraseType eraseType = (EraseType)VTParameter.GetParameter(parameters, 0, 0);
                        VTDebug.WriteAction("ED_EraseDisplay, eraseType = {0}, Cursor={1},{2}", eraseType, this.CursorRow, this.CursorCol);
                        this.activeDocument.EraseDisplay(this.ActiveLine, this.CursorCol, eraseType);
                        break;
                    }

                #endregion

                #region 光标移动

                // 下面的光标移动指令不能进行VTDocument的滚动
                // 光标的移动坐标是相对于可视区域内的坐标
                // 服务器发送过来的光标原点是从(1,1)开始的，我们程序里的是(0,0)开始的，所以要减1

                case VTActions.BS:
                    {
                        this.activeDocument.SetCursor(this.CursorRow, this.CursorCol - 1);
                        VTDebug.WriteAction("CursorBackward, Cursor = {0}, {1}", this.CursorRow, this.CursorCol);
                        break;
                    }

                case VTActions.CursorBackward:
                    {
                        List<int> parameters = parameter as List<int>;
                        int n = VTParameter.GetParameter(parameters, 0, 1);
                        this.activeDocument.SetCursor(this.CursorRow, this.CursorCol - n);
                        VTDebug.WriteAction("CursorBackward, Cursor = {0}, {1}, n = {2}", this.CursorRow, this.CursorCol, n);
                        break;
                    }

                case VTActions.CUF_CursorForward:
                    {
                        List<int> parameters = parameter as List<int>;
                        int n = VTParameter.GetParameter(parameters, 0, 1);
                        this.activeDocument.SetCursor(this.CursorRow, this.CursorCol + n);
                        VTDebug.WriteAction("CUF_CursorForward, Cursor = {0}, {1}, n = {2}", this.CursorRow, this.CursorCol, n);
                        break;
                    }

                case VTActions.CUU_CursorUp:
                    {
                        List<int> parameters = parameter as List<int>;
                        int n = VTParameter.GetParameter(parameters, 0, 1);
                        this.activeDocument.SetCursor(this.CursorRow - n, this.CursorCol);
                        VTDebug.WriteAction("CUU_CursorUp, Cursor = {0}, {1}, n = {2}", this.CursorRow, this.CursorCol, n);
                        break;
                    }

                case VTActions.CUD_CursorDown:
                    {
                        List<int> parameters = parameter as List<int>;
                        int n = VTParameter.GetParameter(parameters, 0, 1);
                        this.activeDocument.SetCursor(this.CursorRow + n, this.CursorCol);
                        VTDebug.WriteAction("CUD_CursorDown, Cursor = {0}, {1}, n = {2}", this.CursorRow, this.CursorCol, n);
                        break;
                    }

                case VTActions.CUP_CursorPosition:
                    {
                        List<int> parameters = parameter as List<int>;

                        int row = 0, col = 0;
                        if (parameters.Count == 2)
                        {
                            // VT的光标原点是(1,1)，我们程序里的是(0,0)，所以要减1
                            row = parameters[0] - 1;
                            col = parameters[1] - 1;
                        }
                        else
                        {
                            // 如果没有参数，那么说明就是定位到原点(0,0)
                        }

                        VTDebug.WriteAction("CUP_CursorPosition, row = {0}, col = {1}", row, col);
                        this.activeDocument.SetCursor(row, col);
                        break;
                    }

                case VTActions.CHA_CursorHorizontalAbsolute:
                    {
                        List<int> parameters = parameter as List<int>;

                        // 将光标移动到当前行中的第n列
                        int n = VTParameter.GetParameter(parameters, 0, -1);
                        if (n == -1)
                        {
                            VTDebug.WriteAction("CHA_CursorHorizontalAbsolute失败");
                            return;
                        }

                        this.ActiveLine.PadColumns(n);
                        this.activeDocument.SetCursor(this.CursorRow, n - 1);
                        VTDebug.WriteAction("CHA_CursorHorizontalAbsolute, targetColumn = {0}", n);
                        break;
                    }

                #endregion

                #region 文本特效

                case VTActions.Bold:
                case VTActions.BoldUnset:
                case VTActions.Underline:
                case VTActions.UnderlineUnset:
                case VTActions.Italics:
                case VTActions.ItalicsUnset:
                case VTActions.DoublyUnderlined:
                case VTActions.DoublyUnderlinedUnset:
                case VTActions.Foreground:
                case VTActions.DefaultForeground:
                case VTActions.Background:
                case VTActions.DefaultBackground:
                    {
                        bool unset;
                        VTextDecorations decoration = XDocumentUtils.VTAction2TextDecoration(action, out unset);
                        VTextAttribute textAttribute = this.ActiveLine.Attributes.FirstOrDefault(v => v.Decoration == decoration);
                        if (textAttribute == null)
                        {
                            textAttribute = new VTextAttribute()
                            {
                                StartCharacter = this.CursorCol,
                                Decoration = decoration,
                            };
                        }

                        if (unset)
                        {
                            textAttribute.EndCharacter = this.CursorCol;
                            textAttribute.Completed = true;
                        }
                        break;
                    }

                case VTActions.DefaultAttributes:
                case VTActions.Faint:
                case VTActions.CrossedOutUnset:
                case VTActions.ReverseVideo:
                case VTActions.ReverseVideoUnset:
                    break;

                #endregion

                #region DECPrivateMode

                case VTActions.DECANM_AnsiMode:
                    {
                        bool enable = Convert.ToBoolean(parameter);
                        VTDebug.WriteAction("DECANM_AnsiMode, enable = {0}", enable);
                        this.Keyboard.SetAnsiMode(enable);
                        break;
                    }

                case VTActions.DECCKM_CursorKeysMode:
                    {
                        bool enable = Convert.ToBoolean(parameter);
                        VTDebug.WriteAction("DECCKM_CursorKeysMode, enable = {0}", enable);
                        this.Keyboard.SetCursorKeyMode(enable);
                        break;
                    }

                case VTActions.DECKPAM_KeypadApplicationMode:
                    {
                        VTDebug.WriteAction("DECKPAM_KeypadApplicationMode");
                        this.Keyboard.SetKeypadMode(true);
                        break;
                    }

                case VTActions.DECKPNM_KeypadNumericMode:
                    {
                        VTDebug.WriteAction("DECKPNM_KeypadNumericMode");
                        this.Keyboard.SetKeypadMode(false);
                        break;
                    }

                case VTActions.DECAWM_AutoWrapMode:
                    {
                        bool enable = Convert.ToBoolean(parameter);
                        this.autoWrapMode = enable;
                        VTDebug.WriteAction("DECAWM_AutoWrapMode, enable = {0}", enable);
                        this.activeDocument.DECPrivateAutoWrapMode = enable;
                        break;
                    }

                case VTActions.XTERM_BracketedPasteMode:
                    {
                        this.xtermBracketedPasteMode = Convert.ToBoolean(parameter);
                        VTDebug.WriteAction("未实现XTERM_BracketedPasteMode");
                        break;
                    }

                case VTActions.ATT610_StartCursorBlink:
                    {
                        break;
                    }

                case VTActions.DECTCEM_TextCursorEnableMode:
                    {
                        break;
                    }

                #endregion

                #region 文本操作

                case VTActions.DCH_DeleteCharacter:
                    {
                        // 从指定位置删除n个字符，删除后的字符串要左对齐，默认删除1个字符
                        List<int> parameters = parameter as List<int>;
                        int count = VTParameter.GetParameter(parameters, 0, 1);
                        VTDebug.WriteAction("DCH_DeleteCharacter, {0}, cursorPos = {1}", count, this.CursorCol);
                        this.activeDocument.DeleteCharacter(this.ActiveLine, this.CursorCol, count);
                        break;
                    }

                case VTActions.ICH_InsertCharacter:
                    {
                        // 在当前光标处插入N个空白字符,这会将所有现有文本移到右侧。 向右溢出屏幕的文本会被删除
                        // 目前没发现这个操作对终端显示有什么影响，所以暂时不实现
                        List<int> parameters = parameter as List<int>;
                        int count = VTParameter.GetParameter(parameters, 0, 1);
                        VTDebug.WriteAction("未实现InsertCharacters, {0}, cursorPos = {1}", count, this.CursorCol);
                        break;
                    }

                #endregion

                #region 上下滚动

                #endregion

                case VTActions.UseAlternateScreenBuffer:
                    {
                        VTDebug.WriteAction("UseAlternateScreenBuffer");

                        ITerminalSurface remove = this.mainDocument.Surface;
                        ITerminalSurface add = this.alternateDocument.Surface;
                        this.TerminalScreen.SwitchSurface(remove, add);

                        // 这里只重置行数，在用户调整窗口大小的时候需要执行终端的Resize操作
                        this.alternateDocument.SetScrollMargin(0, 0);
                        this.alternateDocument.DeleteAll();
                        this.activeDocument = this.alternateDocument;
                        break;
                    }

                case VTActions.UseMainScreenBuffer:
                    {
                        VTDebug.WriteAction("UseMainScreenBuffer");

                        ITerminalSurface remove = this.alternateDocument.Surface;
                        ITerminalSurface add = this.mainDocument.Surface;
                        this.TerminalScreen.SwitchSurface(remove, add);

                        this.mainDocument.DirtyAll();
                        this.activeDocument = this.mainDocument;
                        break;
                    }

                case VTActions.DSR_DeviceStatusReport:
                    {
                        // DSR，参考https://invisible-island.net/xterm/ctlseqs/ctlseqs.html

                        List<int> parameters = parameter as List<int>;
                        StatusType statusType = (StatusType)Convert.ToInt32(parameters[0]);
                        VTDebug.WriteAction("DSR_DeviceStatusReport, statusType = {0}", statusType);
                        this.PerformDeviceStatusReport(statusType);
                        break;
                    }

                case VTActions.DA_DeviceAttributes:
                    {
                        VTDebug.WriteAction("DA_DeviceAttributes");
                        this.sessionTransport.Write(DA_DeviceAttributesResponse);
                        break;
                    }

                case VTActions.DECSTBM_SetScrollingRegion:
                    {
                        // 设置可滚动区域
                        // 不可以操作滚动区域以外的行，只能对滚动区域内的行进行操作
                        // 对于滚动区域的作用的解释，举个例子说明
                        // 比方说marginTop是1，marginBottom也是1
                        // 那么在执行LineFeed动作的时候，默认情况下，是把第一行挂到最后一行的后面，有了margin之后，就要把第二行挂到倒数第二行的后面
                        // ScrollMargin会对很多动作产生影响：LF，RI_ReverseLineFeed，DeleteLine，InsertLine

                        // 视频终端的规范里说，如果topMargin等于bottomMargin，或者bottomMargin大于屏幕高度，那么忽略这个指令
                        // 边距还会影响插入行 (IL) 和删除行 (DL)、向上滚动 (SU) 和向下滚动 (SD) 修改的行。

                        // Notes on DECSTBM
                        // * The value of the top margin (Pt) must be less than the bottom margin (Pb).
                        // * The maximum size of the scrolling region is the page size
                        // * DECSTBM moves the cursor to column 1, line 1 of the page
                        // * https://github.com/microsoft/terminal/issues/1849

                        // 当前终端屏幕可显示的行数量
                        int lines = this.terminalRows;

                        List<int> parameters = parameter as List<int>;
                        int topMargin = VTParameter.GetParameter(parameters, 0, 1);
                        int bottomMargin = VTParameter.GetParameter(parameters, 1, lines);

                        if (bottomMargin < 0 || topMargin < 0)
                        {
                            VTDebug.WriteAction("DECSTBM_SetScrollingRegion参数不正确，忽略本次设置, topMargin = {0}, bottomMargin = {1}", topMargin, bottomMargin);
                            return;
                        }
                        if (topMargin >= bottomMargin)
                        {
                            VTDebug.WriteAction("DECSTBM_SetScrollingRegion参数不正确，topMargin大于等bottomMargin，忽略本次设置, topMargin = {0}, bottomMargin = {1}", topMargin, bottomMargin);
                            return;
                        }
                        if (bottomMargin > lines)
                        {
                            VTDebug.WriteAction("DECSTBM_SetScrollingRegion参数不正确，bottomMargin大于当前屏幕总行数, bottomMargin = {0}, lines = {1}", bottomMargin, lines);
                            return;
                        }

                        // 如果topMargin等于1，那么就表示使用默认值，也就是没有marginTop，所以当topMargin == 1的时候，marginTop改为0
                        int marginTop = topMargin == 1 ? 0 : topMargin;
                        // 如果bottomMargin等于控制台高度，那么就表示使用默认值，也就是没有marginBottom，所以当bottomMargin == 控制台高度的时候，marginBottom改为0
                        int marginBottom = lines - bottomMargin;
                        VTDebug.WriteAction("SetScrollingRegion, topMargin = {0}, bottomMargin = {1}", marginTop, marginBottom);
                        this.activeDocument.SetScrollMargin(marginTop, marginBottom);
                        break;
                    }

                case VTActions.IL_InsertLine:
                    {
                        // 将 <n> 行插入光标位置的缓冲区。 光标所在的行及其下方的行将向下移动。
                        List<int> parameters = parameter as List<int>;
                        int lines = VTParameter.GetParameter(parameters, 0, 1);
                        VTDebug.WriteAction("IL_InsertLine, lines = {0}", lines);
                        if (lines > 0)
                        {
                            this.activeDocument.InsertLines(this.ActiveLine, lines);
                        }
                        break;
                    }

                case VTActions.DL_DeleteLine:
                    {
                        // 从缓冲区中删除<n> 行，从光标所在的行开始。
                        List<int> parameters = parameter as List<int>;
                        int lines = VTParameter.GetParameter(parameters, 0, 1);
                        VTDebug.WriteAction("DL_DeleteLine, lines = {0}", lines);
                        if (lines > 0)
                        {
                            this.activeDocument.DeleteLines(this.ActiveLine, lines);
                        }
                        break;
                    }

                default:
                    {
                        VTDebug.WriteAction("未执行的VTAction, {0}", action);
                        break;
                    }
            }
        }

        private void VTSession_DataReceived(SessionTransport client, byte[] bytes, int size)
        {
            //string str = string.Join(",", bytes.Select(v => v.ToString()).ToList());
            //logger.InfoFormat("Received, {0}", str);
            this.vtParser.ProcessCharacters(bytes, size);

            // 全部字符都处理完了之后，只渲染一次

            this.DrawDocument(this.activeDocument);
            //logger.ErrorFormat("receivedCounter = {0}", this.dataReceivedCounter++);
            //this.activeDocument.Print();
            //logger.ErrorFormat("TotalRows = {0}", this.activeDocument.TotalRows);
        }

        private void VTSession_StatusChanged(object client, SessionStatusEnum status)
        {
            logger.InfoFormat("会话状态发生改变, {0}", status);
            if (this.SessionStatusChanged != null)
            {
                this.SessionStatusChanged(this, status);
            }
        }

        private void CursorBlinkingThreadProc()
        {
            while (this.isRunning)
            {
                VTCursor cursor = this.Cursor;

                cursor.IsVisible = !cursor.IsVisible;

                try
                {
                    double opacity = cursor.IsVisible ? 1 : 0;

                    this.uiSyncContext.Send((state) =>
                    {
                        this.Surface.SetOpacity(cursor, opacity);
                    }, null);
                }
                catch (Exception e)
                {
                    logger.ErrorFormat(string.Format("渲染光标异常, {0}", e));
                }
                finally
                {
                    Thread.Sleep(cursor.Interval);
                }
            }
        }


        /// <summary>
        /// 当滚动条滚动的时候触发
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="scrollValue">滚动到的行数</param>
        private void OnScrollChanged(ITerminalScreen arg1, int scrollValue)
        {
            this.ScrollToHistory(scrollValue);
        }

        private void OnMouseDown(ITerminalScreen screen, VTPoint mousePosition)
        {
            this.isMouseDown = true;
            this.mouseDownPos = mousePosition;
            this.surfaceRect = this.Surface.GetRectRelativeToDesktop();
        }

        private void OnMouseMove(ITerminalScreen arg1, VTPoint mousePosition)
        {
            if (!this.isMouseDown)
            {
                return;
            }

            if (!this.selectionState)
            {
                // 此时说明开始选中操作
                this.selectionState = true;
                this.textSelection.Reset();
                this.Surface.Draw(this.textSelection);
                this.Surface.Arrange(this.textSelection);
            }

            // 如果还没有测量起始字符，那么测量起始字符
            if (this.textSelection.Start.CharacterIndex == -1)
            {
                if (!this.GetTextPointer(mousePosition, this.surfaceRect, this.textSelection.Start))
                {
                    // 没有命中起始字符，那么直接返回啥都不做
                    //logger.DebugFormat("没命中起始字符");
                    return;
                }
            }

            // 首先检测鼠标是否在Surface边界框的外面
            // 如果在Surface的外面并且行数超出了Surface可以显示的最多行数，那么根据鼠标方向进行滚动，每次滚动一行
            OutsideScrollResult scrollResult = this.ScrollIfCursorOutsideSurface(mousePosition, this.surfaceRect);

            // 整理思路是算出来StartTextPointer和EndTextPointer之间的几何图形
            // 然后渲染几何图形，SelectionRange本质上就是一堆矩形
            VTextPointer startPointer = this.textSelection.Start;
            VTextPointer endPointer = this.textSelection.End;

            // 得到当前鼠标的命中信息
            if (!this.GetTextPointer(mousePosition, this.surfaceRect, endPointer))
            {
                // 只有在没有Outside滚动的时候，才返回
                // Outside滚动会导致GetTextPointer失败，虽然失败，还是要更新SelectionRange
                if (scrollResult == OutsideScrollResult.None)
                {
                    return;
                }
            }

            #region 起始字符和结束字符测量出来的索引位置都是-1，啥都不做

            if (startPointer.CharacterIndex < 0 || endPointer.CharacterIndex < 0)
            {
                logger.WarnFormat("鼠标命中的起始字符和结束字符位置都小于0");
                return;
            }

            #endregion

            #region 起始字符和结束字符是同一个字符，啥都不做

            if (startPointer.CharacterIndex == endPointer.CharacterIndex)
            {
                //logger.WarnFormat("鼠标命中的起始字符和结束字符是相同字符");
                return;
            }

            #endregion

            #region 计算并重新渲染选中内容的几何图形，要考虑到滚动条滚动的情况

            // 此时的VTextLine测量数据都是最新的
            // 主缓冲区和备用缓冲区都支持选中
            this.UpdateSelectionGeometry(this.activeDocument, this.textSelection);
            this.Surface.Draw(this.textSelection);

            #endregion
        }

        private void OnMouseUp(ITerminalScreen arg1, VTPoint cursorPos)
        {
            this.isMouseDown = false;
            this.selectionState = false;
        }

        private void OnMouseWheel(ITerminalScreen screen, bool upper)
        {
            // 只有主缓冲区才可以用鼠标滚轮进行滚动
            if (this.activeDocument != this.mainDocument)
            {
                return;
            }

            if (upper)
            {
                // 向上滚动

                // 先判断是不是已经滚动到顶了
                if (this.ScrollAtTop)
                {
                    // 滚动到顶直接返回
                    return;
                }

                if (this.scrollValue < this.mouseOptions.ScrollDelta)
                {
                    // 一次可以全部滚完并且还有剩余
                    this.ScrollToHistory(0);
                }
                else
                {
                    this.ScrollToHistory(this.scrollValue - this.mouseOptions.ScrollDelta);
                }
            }
            else
            {
                // 向下滚动

                if (this.ScrollAtBottom)
                {
                    // 滚动到底直接返回
                    return;
                }

                // 剩余可以往下滚动的行数
                int remainScroll = this.scrollMax - this.scrollValue;

                if (remainScroll >= this.mouseOptions.ScrollDelta)
                {
                    this.ScrollToHistory(this.scrollValue + this.mouseOptions.ScrollDelta);
                }
                else
                {
                    // 直接滚动到底
                    this.ScrollToHistory(this.scrollMax);
                }
            }
        }

        #endregion
    }
}