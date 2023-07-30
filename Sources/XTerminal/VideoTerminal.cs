using System;
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
    /// ���������ն˵������߼�
    /// </summary>
    public class VideoTerminal
    {
        private enum OutsideScrollResult
        {
            /// <summary>
            /// û����
            /// </summary>
            None,

            /// <summary>
            /// ������Ϲ���
            /// </summary>
            ScrollTop,

            /// <summary>
            /// ������¹���
            /// </summary>
            ScrollDown
        }

        #region �����

        private static log4net.ILog logger = log4net.LogManager.GetLogger("VideoTerminal");

        private static readonly byte[] OS_OperationStatusResponse = new byte[4] { (byte)'\x1b', (byte)'[', (byte)'0', (byte)'n' };
        private static readonly byte[] CPR_CursorPositionReportResponse = new byte[6] { (byte)'\x1b', (byte)'[', (byte)'0', (byte)';', (byte)'0', (byte)'R' };
        //private static readonly byte[] CPR_CursorPositionReportResponse = new byte[5] { (byte)'\x1b', (byte)'[', (byte)'0', (byte)';', (byte)'0' };
        private static readonly byte[] DA_DeviceAttributesResponse = new byte[7] { 0x1b, (byte)'[', (byte)'?', (byte)'1', (byte)':', (byte)'0', (byte)'c' };

        #endregion

        #region �����¼�

        public event Action<VideoTerminal, SessionStatusEnum> SessionStatusChanged;

        #endregion

        #region ʵ������

        /// <summary>
        /// ���ն˽���ͨ�ŵ��ŵ�
        /// </summary>
        private SessionTransport sessionTransport;

        /// <summary>
        /// �ն��ַ�������
        /// </summary>
        private VTParser vtParser;

        /// <summary>
        /// ���������ĵ�ģ��
        /// </summary>
        private VTDocument mainDocument;

        /// <summary>
        /// ���û������ĵ�ģ��
        /// </summary>
        private VTDocument alternateDocument;

        /// <summary>
        /// ��ǰ����ʹ�õ��ĵ�ģ��
        /// </summary>
        private VTDocument activeDocument;

        /// <summary>
        /// UI�߳�������
        /// </summary>
        private SynchronizationContext uiSyncContext;

        private XTermSession sessionInfo;
        private MouseOptions mouseOptions;

        /// <summary>
        /// DECAWM�Ƿ�����
        /// </summary>
        private bool autoWrapMode;

        private bool xtermBracketedPasteMode;

        /// <summary>
        /// ��˸�����߳�
        /// </summary>
        private Thread cursorBlinkingThread;

        #region History & Scroll

        /// <summary>
        /// �洢���е���ʷ��
        /// Row(scrollValue) -> VTextLine
        /// ע����ֵ��ﱣ�����mainDocument����ʷ�У�alternateDocumentû�б��棬��Ҫ��������alternateDocument
        /// </summary>
        private Dictionary<int, VTHistoryLine> historyLines;

        /// <summary>
        /// ��ʷ�еĵ�һ��
        /// </summary>
        private VTHistoryLine firstHistoryLine;

        /// <summary>
        /// ��ʷ�е����һ��
        /// </summary>
        private VTHistoryLine lastHistoryLine;

        /// <summary>
        /// ��¼�������������׵�ʱ�򣬹�������ֵ
        /// </summary>
        private int scrollMax;

        /// <summary>
        /// ��¼��ǰ������������ֵ
        /// Ҳ���ǵ�ǰSurface����Ⱦ�ĵ�һ�е�PhysicsRow
        /// Ĭ��ֵ��0
        /// </summary>
        private int scrollValue;

        /// <summary>
        /// ����갴�µ�ʱ�򣬼�¼Canvas�������Ļ������
        /// </summary>
        private VTRect surfaceRect;

        #endregion

        #region SelectionRange

        /// <summary>
        /// ����Ƿ���
        /// </summary>
        private bool isMouseDown;
        private VTPoint mouseDownPos;

        /// <summary>
        /// ��ǰ����Ƿ���Selection״̬
        /// </summary>
        private bool selectionState;

        /// <summary>
        /// �洢ѡ�е��ı���Ϣ
        /// </summary>
        private VTextSelection textSelection;

        #endregion

        private int renderCounter;
        private int dataReceivedCounter;

        /// <summary>
        /// �Ƿ���������
        /// </summary>
        private bool isRunning;

        /// <summary>
        /// ������뷽ʽ
        /// </summary>
        private Encoding inputEncoding;

        #endregion

        #region ����

        /// <summary>
        /// activeDocument�Ĺ����Ϣ
        /// �������ǻ���ViewableDocument������
        /// </summary>
        private VTCursor Cursor { get { return this.activeDocument.Cursor; } }

        /// <summary>
        /// ��ȡ��ǰ���������
        /// </summary>
        public VTextLine ActiveLine { get { return this.activeDocument.ActiveLine; } }

        /// <summary>
        /// ��ȡ��ǰѡ�е���
        /// </summary>
        public VTextSelection Selection { get { return this.textSelection; } }

        /// <summary>
        /// ��ȡ��ǰ���������
        /// </summary>
        public int CursorRow { get { return this.Cursor.Row; } }

        /// <summary>
        /// ��ȡ��ǰ���������
        /// </summary>
        public int CursorCol { get { return this.Cursor.Column; } }

        /// <summary>
        /// ��Ⱦ�ն�����Ļ���
        /// </summary>
        public ITerminalSurface Surface { get { return this.activeDocument.Surface; } }

        /// <summary>
        /// �ĵ���������
        /// </summary>
        public ITerminalScreen TerminalScreen { get; set; }

        /// <summary>
        /// ���ݵ�ǰ���Լ��̵İ���״̬��ת���ɱ�׼��ANSI��������
        /// </summary>
        public VTKeyboard Keyboard { get; private set; }

        public VTextOptions TextOptions { get; private set; }

        /// <summary>
        /// ��ȡ��ǰ�������Ƿ����������
        /// </summary>
        public bool ScrollAtBottom
        {
            get
            {
                return this.scrollValue == this.scrollMax;
            }
        }

        /// <summary>
        /// ��ȡ��ǰ�������Ƿ����������
        /// </summary>
        public bool ScrollAtTop
        {
            get
            {
                return this.scrollValue == 0;
            }
        }

        #endregion

        #region ���췽��

        public VideoTerminal()
        {
        }

        #endregion

        #region �����ӿ�

        /// <summary>
        /// ��ʼ���ն�ģ����
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

            // ��ʼ������
            this.historyLines = new Dictionary<int, VTHistoryLine>();
            this.TextOptions = new VTextOptions();
            this.textSelection = new VTextSelection();

            this.isRunning = true;

            #region ��ʼ������

            this.Keyboard = new VTKeyboard();
            this.Keyboard.Encoding = Encoding.GetEncoding(sessionInfo.InputEncoding);
            this.Keyboard.SetAnsiMode(true);
            this.Keyboard.SetKeypadMode(false);

            #endregion

            #region ��ʼ���ն˽�����

            this.vtParser = new VTParser();
            this.vtParser.ActionEvent += VtParser_ActionEvent;
            this.vtParser.Initialize();

            #endregion

            #region ��ʼ������¼�

            this.TerminalScreen.InputEvent += this.VideoTerminal_InputEvent;
            this.TerminalScreen.ScrollChanged += this.TerminalScreen_ScrollChanged;
            this.TerminalScreen.VTMouseDown += this.TerminalScreen_VTMouseDown;
            this.TerminalScreen.VTMouseMove += this.TerminalScreen_VTMouseMove;
            this.TerminalScreen.VTMouseUp += this.TerminalScreen_VTMouseUp;
            this.TerminalScreen.VTMouseWheel += this.TerminalScreen_VTMouseWheel;

            #endregion

            #region ��ʼ���ĵ�ģ��

            VTDocumentOptions documentOptions = new VTDocumentOptions()
            {
                ColumnSize = this.sessionInfo.TerminalOptions.Columns,
                RowSize = this.sessionInfo.TerminalOptions.Rows,
                DECPrivateAutoWrapMode = this.sessionInfo.TerminalOptions.DECPrivateAutoWrapMode,
                CursorStyle = this.sessionInfo.MouseOptions.CursorStyle,
                Interval = this.sessionInfo.MouseOptions.CursorInterval,
                CanvasCreator = this.TerminalScreen
            };
            this.mainDocument = new VTDocument(documentOptions) { Name = "MainDocument" };
            this.alternateDocument = new VTDocument(documentOptions) { Name = "AlternateDocument" };
            this.activeDocument = this.mainDocument;
            this.firstHistoryLine = VTHistoryLine.Create(0, null, this.ActiveLine);
            this.historyLines[0] = this.firstHistoryLine;
            this.lastHistoryLine = this.firstHistoryLine;
            this.TerminalScreen.SwitchSurface(null, this.activeDocument.Surface);

            #endregion

            #region ��ʼ�����

            this.Surface.Draw(this.Cursor);
            this.cursorBlinkingThread = new Thread(this.CursorBlinkingThreadProc);
            this.cursorBlinkingThread.IsBackground = true;
            this.cursorBlinkingThread.Start();
            // �ȳ�ʼ�����û������Ĺ����Ⱦ������
            this.alternateDocument.Surface.Draw(this.alternateDocument.Cursor);

            #endregion

            #region �����ն�ͨ��

            SessionTransport transport = new SessionTransport();
            transport.StatusChanged += this.VTSession_StatusChanged;
            transport.DataReceived += this.VTSession_DataReceived;
            transport.Initialize(sessionInfo);
            transport.Open();
            this.sessionTransport = transport;

            #endregion
        }

        /// <summary>
        /// �ͷ���Դ
        /// </summary>
        public void Release()
        {
            this.isRunning = false;

            this.vtParser.ActionEvent -= VtParser_ActionEvent;
            this.vtParser.Release();

            this.TerminalScreen.InputEvent -= this.VideoTerminal_InputEvent;
            this.TerminalScreen.ScrollChanged -= this.TerminalScreen_ScrollChanged;
            this.TerminalScreen.VTMouseDown -= this.TerminalScreen_VTMouseDown;
            this.TerminalScreen.VTMouseMove -= this.TerminalScreen_VTMouseMove;
            this.TerminalScreen.VTMouseUp -= this.TerminalScreen_VTMouseUp;
            this.TerminalScreen.VTMouseWheel -= this.TerminalScreen_VTMouseWheel;

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
        /// ���Ƶ�ǰѡ�е���
        /// </summary>
        public void CopySelection()
        {
            if (this.textSelection.IsEmpty)
            {
                return;
            }

            string text = this.textSelection.GetText(this.historyLines);

            // ���ü�����API���Ƶ�������
            Clipboard.SetText(text);
        }

        /// <summary>
        /// ������������ݷ��͸��Ự
        /// </summary>
        /// <returns>���ͳɹ�����SUCCESS��ʧ�ܷ��ش�����</returns>
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
                logger.ErrorFormat("ճ������ʧ��, {0}", code);
            }

            return code;
        }

        /// <summary>
        /// ѡ��ȫ�����ı�
        /// </summary>
        public void SelectAll()
        {
            this.textSelection.Reset();

            if (this.firstHistoryLine == null || this.lastHistoryLine == null)
            {
                logger.WarnFormat("SelectAllʧ��, ��ʷ��¼��ĵ�һ�л������һ��Ϊ��");
                return;
            }

            //this.textSelection.Start.LineHit = this.firstHistoryLine;
            //this.textSelection.Start.CharacterIndex = 0;

            //this.textSelection.End.LineHit = this.lastHistoryLine;
            //this.textSelection.End.CharacterIndex = this.lastHistoryLine.Text.Length - 1;
        }

        /// <summary>
        /// �����HTML�ĵ�
        /// ������ɫ��
        /// </summary>
        /// <param name="filePath"></param>
        public void SaveAsHtml(string filePath)
        {

        }

        /// <summary>
        /// �������ͨ�ı��ĵ�
        /// </summary>
        /// <param name="filePath"></param>
        public void SaveAsText(string filePath)
        {
            string text = this.textSelection.GetText(this.historyLines);
            File.WriteAllText(filePath, text);
        }

        #endregion

        #region ʵ������

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
                        // ��VIM����յ��������
                        // ����CPR��ʱ����ΪCPR��Ϣ������и�R���ᵼ�´�VIM��ֱ�ӽ����滻ģʽ
                        // ���BUG��ʱû�ҵ�������ԭ��ͽ��������������ʱ��ע�͵�

                        //// Result is CSI r ; c R
                        //int cursorRow = this.CursorRow;
                        //int cursorCol = this.CursorCol;
                        //CPR_CursorPositionReportResponse[2] = (byte)cursorRow;
                        //CPR_CursorPositionReportResponse[4] = (byte)cursorCol;
                        //this.sessionTransport.Write(CPR_CursorPositionReportResponse);
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// �����Ҫ��������в���
        /// �������Ҫ���֣���ô�Ϳ��Ƿ���Ҫ�ػ�ĳЩ�ı���
        /// </summary>
        /// <param name="document">Ҫ��Ⱦ���ĵ�</param>
        /// <param name="scrollValue">
        /// �Ƿ�Ҫ�ƶ�������������Ϊ-1��ʾ���ƶ�������
        /// ע������ֻ�Ǹ���UI�ϵĹ�����λ�ã�������ʵ�ʵ�ȥ��������
        /// </param>
        private void DrawDocument(VTDocument document, int scrollValue = -1, bool arrangeSelectionArea = false)
        {
            // ��ǰ�е�Y����ƫ����
            double offsetY = 0;

            bool arrangeDirty = document.IsArrangeDirty;
            bool activeLineDirty = this.ActiveLine.IsRenderDirty;

            this.uiSyncContext.Send((state) =>
            {
                #region ��Ⱦ�ĵ�

                VTextLine next = document.FirstLine;

                while (next != null)
                {
                    // ����Yƫ������Ϣ
                    next.OffsetY = offsetY;

                    if (next.IsRenderDirty)
                    {
                        // ��ʱ˵���������ַ��仯����Ҫ�ػ�
                        this.Surface.Draw(next);
                        //logger.ErrorFormat("renderCounter = {0}", this.renderCounter++);
                    }
                    else if (next.IsMeasureDirety)
                    {
                        // �ַ�û�б仯����ôֻ���²���Ȼ�����һ���ı���ƫ�����ͺ���
                        this.Surface.MeasureLine(next);
                    }

                    // ������ƫ����
                    if (arrangeDirty)
                    {
                        this.Surface.Arrange(next);
                    }

                    // ������һ���ı��е�Yƫ����
                    offsetY += next.Height;

                    // ������һ����Ⱦ����ˣ���ô���˳�
                    if (next == document.LastLine)
                    {
                        break;
                    }

                    next = next.NextLine;
                }

                #endregion

                #region ��Ⱦ���

                this.Cursor.OffsetY = this.ActiveLine.OffsetY;
                this.Cursor.OffsetX = this.Surface.MeasureBlock(this.ActiveLine, this.ActiveLine.FindCharacterIndex(this.CursorCol)).Width;
                this.Surface.Arrange(this.Cursor);

                #endregion

                #region �ƶ�������

                if (scrollValue != -1)
                {
                    this.TerminalScreen.ScrollTo(scrollValue);
                }

                #endregion

                #region ����ѡ�������λ��

                if (arrangeSelectionArea)
                {
                    this.Surface.Arrange(this.textSelection);
                }

                #endregion

            }, null);

            if (this.activeDocument == this.mainDocument)
            {
                if (this.ScrollAtBottom)
                {
                    // ���������ˣ�˵����ActiveLine���ǵ�ǰ�����������
                    // ��������ʷ�еĴ�С�����֣���Ȼ��ִ�й��ѡ�е�ʱ���ı�Ϊ�գ���Ӱ�쵽����
                    this.lastHistoryLine.Freeze(this.ActiveLine);
                }
            }

            document.SetArrangeDirty(false);
        }

        /// <summary>
        /// ������ָ������ʷ��¼
        /// ������UI�ϵĹ�����λ��
        /// </summary>
        /// <param name="scrollValue">Ҫ��ʾ�ĵ�һ����ʷ��¼</param>
        private void ScrollToHistory(int scrollValue)
        {
            // Ҫ��������ֵ
            int newScroll = scrollValue;
            // ����֮ǰ��ֵ
            int oldScroll = this.scrollValue;

            // ���µ�ǰ��������ֵ��һ��Ҫ�ȸ��£���ΪDrawDocument�������õ���ֵ
            this.scrollValue = scrollValue;

            // �ն˿�����ʾ��������
            int terminalRows = this.sessionInfo.TerminalOptions.Rows;

            // ������������
            int scrolledRows = Math.Abs(newScroll - oldScroll);

            // �Ƿ���Ҫ�ƶ�ѡ������
            bool arrangeSelectionArea = false;

            if (scrolledRows >= terminalRows)
            {
                // ���ҵ�Surface��Ҫ��ʾ�ĵ�һ������
                VTHistoryLine historyLine;
                if (!this.historyLines.TryGetValue(scrollValue, out historyLine))
                {
                    logger.ErrorFormat("ScrollToʧ��, �Ҳ�����Ӧ��VTHistoryLine, scrollValue = {0}", scrollValue);
                    return;
                }

                // ��ʱ˵���������ж�������Surface���ˣ���Ҫ������ʾ������
                // �ҵ������������ʾ
                VTHistoryLine currentHistory = historyLine;
                VTextLine currentTextLine = this.activeDocument.FirstLine;
                for (int i = 0; i < terminalRows; i++)
                {
                    // ֱ��ʹ��VTHistoryLine��List<VTCharacter>������
                    // ����״̬�µ�VTextLine���������޸���
                    // �Ƕ���״̬(ActiveLine)��Ҫ���´���һ������
                    currentTextLine.SetHistory(currentHistory);
                    currentHistory = currentHistory.NextLine;
                    currentTextLine = currentTextLine.NextLine;
                }

                // �����ǰ��ѡ�����ݣ���ô����ѡ�����ݵ�ͼ��Ϊλ��
                if (!this.textSelection.IsEmpty)
                {
                    this.textSelection.OffsetY = -9999;
                }
            }
            else
            {
                // ������ƶ�ǰ�ĵ�һ��/���һ�����ƶ���ĵ�һ��/���һ�еĲ�ֵ
                // �����ֵ����TextSelection.Geometry��OffsetY
                VTextLine line1 = null;
                VTextLine line2 = null;

                // ��ʱ˵��ֻ��Ҫ�����ƶ���ȥ���оͿ�����
                if (newScroll > oldScroll)
                {
                    line1 = this.activeDocument.FirstLine;

                    // ���¹�������������õ����棬�ӵ�һ�п�ʼ
                    for (int i = 0; i < scrolledRows; i++)
                    {
                        VTHistoryLine historyLine = this.historyLines[oldScroll + terminalRows + i];

                        // ��ֵ��Զ�ǵ�һ�У���Ϊ���汻Move�����һ����
                        VTextLine firstLine = this.activeDocument.FirstLine;

                        firstLine.Move(VTextLine.MoveOptions.MoveToLast);

                        firstLine.SetHistory(historyLine);
                    }

                    line2 = this.activeDocument.FirstLine;
                }
                else
                {
                    line1 = this.activeDocument.LastLine;

                    // ���Ϲ�������������õ����棬�����һ�п�ʼ
                    for (int i = 1; i <= scrolledRows; i++)
                    {
                        VTHistoryLine historyLine = this.historyLines[oldScroll - i];

                        VTextLine lastLine = this.activeDocument.LastLine;

                        lastLine.Move(VTextLine.MoveOptions.MoveToFirst);

                        lastLine.SetHistory(historyLine);
                    }

                    line2 = this.activeDocument.LastLine;
                }

                // �����ǰ��ѡ�����ݣ���ô����ѡ�����ݵ�ͼ��Ϊλ��
                if (!this.textSelection.IsEmpty)
                {
                    this.textSelection.OffsetY += line1.OffsetY - line2.OffsetY;
                    arrangeSelectionArea = true;
                }
            }

            this.activeDocument.SetArrangeDirty(true);
            this.DrawDocument(this.activeDocument, scrollValue, arrangeSelectionArea);
        }

        /// <summary>
        /// ����������������ƶ���ʱ�򣬽��й���
        /// </summary>
        /// <param name="mousePosition">��ǰ��������</param>
        /// <param name="surfaceBoundary">����ڵ�����ʾ���Ļ����ı߽��</param>
        /// <returns>�Ƿ�ִ���˹�������</returns>
        private OutsideScrollResult ScrollIfCursorOutsideSurface(VTPoint mousePosition, VTRect surfaceBoundary)
        {
            OutsideScrollResult scrollResult = OutsideScrollResult.None;

            // Ҫ��������Ŀ����
            int scrollTarget = -1;

            if (mousePosition.Y < 0)
            {
                // �������������
                if (!this.ScrollAtTop)
                {
                    // ���������棬���Ϲ���һ��
                    scrollTarget = this.scrollValue - 1;
                    scrollResult = OutsideScrollResult.ScrollTop;
                }
            }
            else if (mousePosition.Y > surfaceBoundary.Height)
            {
                // �������������
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
        /// ʹ�����������VTextLine�����в���
        /// </summary>
        /// <param name="mousePosition">�������</param>
        /// <param name="surfaceBoundary">����ڵ�����ʾ���Ļ����ı߽��Ҳ�������޶���Χ</param>
        /// <param name="pointer">�洢���в��Խ���ı���</param>
        /// <remarks>������ݽ��������λ���ڴ����⣬��ô�������޶��ھ�����������Surface��Ե��</remarks>
        /// <returns>
        /// �Ƿ��ȡ�ɹ�
        /// ����겻��ĳһ�л��߲���ĳ���ַ��ϵ�ʱ�򣬾ͻ�ȡʧ��
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

            pointer.CharacterIndex = -1;

            #region ���ҵ����������

            // �п��ܵ�ǰ�й���������Ҫ����ʷ���￪ʼ��
            // �Ȼ�ȡ����ǰ��Ļ����ʾ����ʷ�е�����

            VTHistoryLine topHistoryLine;
            if (!this.historyLines.TryGetValue(this.scrollValue, out topHistoryLine))
            {
                logger.DebugFormat("GetTextPointerʧ��, ��������ʷ�м�¼, currentScroll = {0}", this.scrollValue);
                return false;
            }

            double lineOffsetY;
            VTHistoryLine lineHit = VTextSelectionHelper.HitTestVTextLine(topHistoryLine, mouseY, out lineOffsetY);
            if (lineHit == null)
            {
                // ����˵�����û�����κ�һ����
                logger.DebugFormat("û���ҵ����λ�ö�Ӧ����, cursorY = {0}", mouseY);
                return false;
            }

            pointer.PhysicsRow = lineHit.PhysicsRow;

            #endregion

            #region �ټ�������������ĸ��ַ���

            int characterIndex;
            VTRect characterBounds;
            if (!VTextSelectionHelper.HitTestVTCharacter(this.Surface, lineHit, mouseX, out characterIndex, out characterBounds))
            {
                return false;
            }

            pointer.CharacterIndex = characterIndex;
            pointer.CharacterBounds = characterBounds;
            pointer.OffsetY = lineOffsetY;

            #endregion

            return true;
        }

        /// <summary>
        /// �Զ��ж�ch�Ƕ��ֽ��ַ����ǵ��ֽ��ַ�������һ��VTCharacter
        /// </summary>
        /// <param name="ch">���ֽڻ��ߵ��ֽڵ��ַ�</param>
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

        #endregion

        #region �¼�������

        /// <summary>
        /// ���û����°�����ʱ�򴥷�
        /// </summary>
        /// <param name="terminal"></param>
        private void VideoTerminal_InputEvent(ITerminalScreen canvasPanel, VTInputEvent evt)
        {
            byte[] bytes = this.Keyboard.TranslateInput(evt);
            if (bytes == null)
            {
                return;
            }

            // ��������Ķ��Ǽ��̰���
            int code = this.sessionTransport.Write(bytes);
            if (code != ResponseCode.SUCCESS)
            {
                logger.ErrorFormat("���������쳣, {0}", ResponseCode.GetMessage(code));
            }
        }

        private void VtParser_ActionEvent(VTActions action, object parameter)
        {
            switch (action)
            {
                case VTActions.Print:
                    {
                        // ���ݲ��Եó����ۣ�
                        // ��VIMģʽ�����������ַ���VIM���Զ��ѹ�������ƶ�2�У������ж�VIM��һ�������ַ�ռ��2�еĿ��
                        // ������ģʽ�£�������������ַ���Ҳʹ��2������ʾ
                        // Ҳ����˵������ն�����һ����80����ô������ʾ40�������ַ�����ʾ��40�������ַ����Ҫ����

                        // �����shell��ɾ��һ�������ַ�����ô��ִ�����ι������ƶ��Ķ�����Ȼ��EraseLine - ToEnd
                        // �ɴ˿ɵó����ۣ�������VIM����shell��һ�������ַ����ǰ���ռ�����еĿռ��������

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
                        // �ѹ���ƶ����п�ͷ
                        this.activeDocument.SetCursor(this.CursorRow, 0);
                        VTDebug.WriteAction("CarriageReturn, Cursor={0},{1}", this.CursorRow, this.CursorCol);
                        break;
                    }

                case VTActions.FF:
                case VTActions.VT:
                case VTActions.LF:
                    {
                        // LF
                        // �����߾��Ӱ�쵽LF��DECSTBM_SetScrollingRegion������ʵ�ֵ�ʱ��Ҫ���ǵ������߾�

                        if (this.activeDocument == this.mainDocument)
                        {
                            // ���������������ײ�����ô�Ȱѹ�������������
                            if (!this.ScrollAtBottom)
                            {
                                this.ScrollToHistory(this.scrollMax);
                            }
                        }

                        // ����һ����һ����ӡ����һ��ֽ�ϴ��֣�����ӡ�����ƶ�����һ�д��ֵ�ʱ�����ᷢ��һ��LineFeedָ���ֽ�����ƶ�һ��
                        // LineFeed��������˼���ǰ�ֽ�ϵ���һ��ι����ӡ��ʹ��
                        this.activeDocument.LineFeed();

                        // ����֮���¼��ʷ��
                        // ע���û���������Backspace�������������ҹ������޸������е����ݣ���������һ�е�������ʵʱ�仯�ģ�Ŀǰ�Ľ������������Ⱦ�����ĵ���ʱ��ȥ�������һ����ʷ�е�����
                        // MainScrrenBuffer��AlternateScrrenBuffer����зֱ��¼
                        // AlternateScreenBuffer��������man��vim�ȳ���ʹ�õ�
                        // ��ʱֻ��¼��������������ݣ����û�������Ҫ��������ô��¼����ΪVIM��Man�ȳ����õ��Ǳ��û��������û��ǿ���ʵʱ�༭������������ݵ�
                        if (this.activeDocument == this.mainDocument)
                        {
                            // ����ȷ������֮ǰ�����Ѿ����û��������ˣ����ᱻ�����ˣ��������ﶳ��һ�»���֮ǰ����ʷ�е����ݣ�����֮�󣬸���ʷ�е����ݾͲ����ٸ�����
                            // �м������������
                            // 1. �������һ���Է����˶������ݣ���ô�п���ǰ��ļ��ж�û�в�������������Ҫ���ж���һ���Ƿ��в�����

                            if (this.ActiveLine.PreviousLine.IsMeasureDirety)
                            {
                                this.Surface.MeasureLine(this.ActiveLine.PreviousLine);
                            }
                            this.lastHistoryLine.Freeze(this.ActiveLine.PreviousLine);

                            // �ٴ��������е���ʷ��
                            // �Ȳ��������µ��У�ȷ���и߶�
                            this.Surface.MeasureLine(this.ActiveLine);
                            int historyIndex = this.lastHistoryLine.PhysicsRow + 1;
                            VTHistoryLine historyLine = VTHistoryLine.Create(historyIndex, this.lastHistoryLine, this.ActiveLine);
                            this.historyLines[historyIndex] = historyLine;
                            this.lastHistoryLine = historyLine;

                            // ��������������
                            int terminalRows = this.sessionInfo.TerminalOptions.Rows;
                            // ������������Թ��������ֵ
                            int scrollMax = historyIndex - terminalRows + 1;
                            if (scrollMax > 0)
                            {
                                // ���¹�������ֵ
                                this.scrollMax = scrollMax;
                                this.scrollValue = scrollMax;

                                logger.DebugFormat("scrollMax = {0}", scrollMax);
                                this.uiSyncContext.Send((state) =>
                                {
                                    this.TerminalScreen.UpdateScrollInfo(scrollMax);
                                    this.TerminalScreen.ScrollToEnd(ScrollOrientation.Down);

                                    // �����ǰ��ѡ�е����ݣ���ô����ѡ�е�����
                                    // ѡ�е����������ƶ�
                                    if (!this.textSelection.IsEmpty)
                                    {
                                        this.textSelection.OffsetY -= this.lastHistoryLine.PreviousLine.Height;
                                        this.Surface.Arrange(this.textSelection);
                                    }

                                }, null);
                            }
                        }

                        VTDebug.WriteAction("LF/FF/VT, Cursor={0},{1}, {2}", this.CursorRow, this.CursorCol, action);
                        break;
                    }

                case VTActions.RI_ReverseLineFeed:
                    {
                        // ��LineFeed�෴��Ҳ���ǰѹ��������һ��λ��
                        // ����man�����ʱ��ᴥ�����ָ��
                        // ������ �C ִ��\n�ķ������������������ƶ�һ�У�ά��ˮƽλ�ã����б�Ҫ������������ *
                        this.activeDocument.ReverseLineFeed();
                        VTDebug.WriteAction("ReverseLineFeed");
                        break;
                    }

                #region Erase

                case VTActions.EL_EraseLine:
                    {
                        List<int> parameters = parameter as List<int>;
                        EraseType eraseType = (EraseType)VTParameter.GetParameter(parameters, 0, 0);
                        VTDebug.WriteAction("EL_EraseLine, eraseType = {0}, �ӹ��λ��{1},{2}��ʼerase", eraseType, this.CursorRow, this.CursorCol);
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

                #region ����ƶ�

                // ����Ĺ���ƶ�ָ��ܽ���VTDocument�Ĺ���
                // �����ƶ�����������ڿ��������ڵ�����
                // ���������͹����Ĺ��ԭ���Ǵ�(1,1)��ʼ�ģ����ǳ��������(0,0)��ʼ�ģ�����Ҫ��1

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
                            // VT�Ĺ��ԭ����(1,1)�����ǳ��������(0,0)������Ҫ��1
                            row = parameters[0] - 1;
                            col = parameters[1] - 1;
                        }
                        else
                        {
                            // ���û�в�������ô˵�����Ƕ�λ��ԭ��(0,0)
                        }

                        VTDebug.WriteAction("CUP_CursorPosition, row = {0}, col = {1}", row, col);
                        this.activeDocument.SetCursor(row, col);
                        break;
                    }

                case VTActions.CHA_CursorHorizontalAbsolute:
                    {
                        List<int> parameters = parameter as List<int>;

                        // ������ƶ�����ǰ���еĵ�n��
                        int n = VTParameter.GetParameter(parameters, 0, -1);
                        if (n == -1)
                        {
                            VTDebug.WriteAction("CHA_CursorHorizontalAbsoluteʧ��");
                            return;
                        }

                        this.ActiveLine.PadColumns(n);
                        this.activeDocument.SetCursor(this.CursorRow, n);
                        VTDebug.WriteAction("CHA_CursorHorizontalAbsolute, targetColumn = {0}", n);
                        break;
                    }

                #endregion

                #region �ı���Ч

                case VTActions.PlayBell:
                case VTActions.Bold:
                case VTActions.Foreground:
                case VTActions.Background:
                case VTActions.DefaultAttributes:
                case VTActions.DefaultBackground:
                case VTActions.DefaultForeground:
                case VTActions.Underline:
                case VTActions.UnderlineUnset:
                case VTActions.Faint:
                case VTActions.ItalicsUnset:
                case VTActions.CrossedOutUnset:
                case VTActions.DoublyUnderlined:
                case VTActions.DoublyUnderlinedUnset:
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
                        VTDebug.WriteAction("δʵ��XTERM_BracketedPasteMode");
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

                #region �ı�����

                case VTActions.DCH_DeleteCharacter:
                    {
                        // ��ָ��λ��ɾ��n���ַ���ɾ������ַ���Ҫ����룬Ĭ��ɾ��1���ַ�
                        List<int> parameters = parameter as List<int>;
                        int count = VTParameter.GetParameter(parameters, 0, 1);
                        VTDebug.WriteAction("DCH_DeleteCharacter, {0}, cursorPos = {1}", count, this.CursorCol);
                        this.activeDocument.DeleteCharacter(this.ActiveLine, this.CursorCol, count);
                        break;
                    }

                case VTActions.ICH_InsertCharacter:
                    {
                        // �ڵ�ǰ��괦����N���հ��ַ�,��Ὣ���������ı��Ƶ��Ҳࡣ ���������Ļ���ı��ᱻɾ��
                        // Ŀǰû��������������ն���ʾ��ʲôӰ�죬������ʱ��ʵ��
                        List<int> parameters = parameter as List<int>;
                        int count = VTParameter.GetParameter(parameters, 0, 1);
                        VTDebug.WriteAction("δʵ��InsertCharacters, {0}, cursorPos = {1}", count, this.CursorCol);
                        break;
                    }

                #endregion

                #region ���¹���

                #endregion

                case VTActions.UseAlternateScreenBuffer:
                    {
                        VTDebug.WriteAction("UseAlternateScreenBuffer");

                        ITerminalSurface remove = this.mainDocument.Surface;
                        ITerminalSurface add = this.alternateDocument.Surface;
                        this.TerminalScreen.SwitchSurface(remove, add);

                        // ����ֻ�������������û��������ڴ�С��ʱ����Ҫִ���ն˵�Resize����
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
                        // ���ÿɹ�������
                        // �����Բ�����������������У�ֻ�ܶԹ��������ڵ��н��в���
                        // ���ڹ�����������õĽ��ͣ��ٸ�����˵��
                        // �ȷ�˵marginTop��1��marginBottomҲ��1
                        // ��ô��ִ��LineFeed������ʱ��Ĭ������£��ǰѵ�һ�йҵ����һ�еĺ��棬����margin֮�󣬾�Ҫ�ѵڶ��йҵ������ڶ��еĺ���
                        // ScrollMargin��Ժܶද������Ӱ�죺LF��RI_ReverseLineFeed��DeleteLine��InsertLine

                        // ��Ƶ�ն˵Ĺ淶��˵�����topMargin����bottomMargin������bottomMargin������Ļ�߶ȣ���ô�������ָ��
                        // �߾໹��Ӱ������� (IL) ��ɾ���� (DL)�����Ϲ��� (SU) �����¹��� (SD) �޸ĵ��С�

                        // Notes on DECSTBM
                        // * The value of the top margin (Pt) must be less than the bottom margin (Pb).
                        // * The maximum size of the scrolling region is the page size
                        // * DECSTBM moves the cursor to column 1, line 1 of the page
                        // * https://github.com/microsoft/terminal/issues/1849

                        // ��ǰ�ն���Ļ����ʾ��������
                        int lines = this.sessionInfo.TerminalOptions.Rows;

                        List<int> parameters = parameter as List<int>;
                        int topMargin = VTParameter.GetParameter(parameters, 0, 1);
                        int bottomMargin = VTParameter.GetParameter(parameters, 1, lines);

                        if (bottomMargin < 0 || topMargin < 0)
                        {
                            VTDebug.WriteAction("DECSTBM_SetScrollingRegion��������ȷ�����Ա�������, topMargin = {0}, bottomMargin = {1}", topMargin, bottomMargin);
                            return;
                        }
                        if (topMargin >= bottomMargin)
                        {
                            VTDebug.WriteAction("DECSTBM_SetScrollingRegion��������ȷ��topMargin���ڵ�bottomMargin�����Ա�������, topMargin = {0}, bottomMargin = {1}", topMargin, bottomMargin);
                            return;
                        }
                        if (bottomMargin > lines)
                        {
                            VTDebug.WriteAction("DECSTBM_SetScrollingRegion��������ȷ��bottomMargin���ڵ�ǰ��Ļ������, bottomMargin = {0}, lines = {1}", bottomMargin, lines);
                            return;
                        }

                        // ���topMargin����1����ô�ͱ�ʾʹ��Ĭ��ֵ��Ҳ����û��marginTop�����Ե�topMargin == 1��ʱ��marginTop��Ϊ0
                        int marginTop = topMargin == 1 ? 0 : topMargin;
                        // ���bottomMargin���ڿ���̨�߶ȣ���ô�ͱ�ʾʹ��Ĭ��ֵ��Ҳ����û��marginBottom�����Ե�bottomMargin == ����̨�߶ȵ�ʱ��marginBottom��Ϊ0
                        int marginBottom = lines - bottomMargin;
                        VTDebug.WriteAction("SetScrollingRegion, topMargin = {0}, bottomMargin = {1}", marginTop, marginBottom);
                        this.activeDocument.SetScrollMargin(marginTop, marginBottom);
                        break;
                    }

                case VTActions.IL_InsertLine:
                    {
                        // �� <n> �в�����λ�õĻ������� ������ڵ��м����·����н������ƶ���
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
                        // �ӻ�������ɾ��<n> �У��ӹ�����ڵ��п�ʼ��
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
                        VTDebug.WriteAction("δִ�е�VTAction, {0}", action);
                        break;
                    }
            }
        }

        private void VTSession_DataReceived(SessionTransport client, byte[] bytes, int size)
        {
            //string str = string.Join(",", bytes.Select(v => v.ToString()).ToList());
            //logger.InfoFormat("Received, {0}", str);
            this.vtParser.ProcessCharacters(bytes, size);

            // ȫ���ַ�����������֮��ֻ��Ⱦһ��

            this.DrawDocument(this.activeDocument);
            //logger.ErrorFormat("receivedCounter = {0}", this.dataReceivedCounter++);
            //this.activeDocument.Print();
            //logger.ErrorFormat("TotalRows = {0}", this.activeDocument.TotalRows);
        }

        private void VTSession_StatusChanged(object client, SessionStatusEnum status)
        {
            logger.InfoFormat("�Ự״̬�����ı�, {0}", status);
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
                    logger.ErrorFormat(string.Format("��Ⱦ����쳣, {0}", e));
                }
                finally
                {
                    Thread.Sleep(cursor.Interval);
                }
            }
        }


        /// <summary>
        /// ��������������ʱ�򴥷�
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="scrollValue">������������</param>
        private void TerminalScreen_ScrollChanged(ITerminalScreen arg1, int scrollValue)
        {
            this.ScrollToHistory(scrollValue);
        }

        private void TerminalScreen_VTMouseDown(ITerminalScreen canvasPanel, VTPoint mousePosition)
        {
            this.isMouseDown = true;
            this.mouseDownPos = mousePosition;
            this.surfaceRect = this.Surface.GetRectRelativeToDesktop();
        }

        private void TerminalScreen_VTMouseMove(ITerminalScreen arg1, VTPoint mousePosition)
        {
            if (!this.isMouseDown)
            {
                return;
            }

            if (!this.selectionState)
            {
                // ��ʱ˵����ʼѡ�в���
                this.selectionState = true;
                this.textSelection.Reset();
                this.Surface.Draw(this.textSelection);
                this.Surface.Arrange(this.textSelection);
            }

            // �����û�в�����ʼ�ַ�����ô������ʼ�ַ�
            if (this.textSelection.Start.CharacterIndex == -1)
            {
                if (!this.GetTextPointer(mousePosition, this.surfaceRect, this.textSelection.Start))
                {
                    // û��������ʼ�ַ�����ôֱ�ӷ���ɶ������
                    return;
                }
            }

            // ���ȼ������Ƿ���Surface�߽�������
            // �����Surface�����沢������������Surface������ʾ�������������ô������귽����й�����ÿ�ι���һ��
            OutsideScrollResult scrollResult = this.ScrollIfCursorOutsideSurface(mousePosition, this.surfaceRect);

            // ����˼·�������StartTextPointer��EndTextPointer֮��ļ���ͼ��
            // Ȼ����Ⱦ����ͼ�Σ�SelectionRange�����Ͼ���һ�Ѿ���
            VTextPointer startPointer = this.textSelection.Start;
            VTextPointer endPointer = this.textSelection.End;

            // �õ���ǰ����������Ϣ
            if (!this.GetTextPointer(mousePosition, this.surfaceRect, endPointer))
            {
                // ֻ����û��Outside������ʱ�򣬲ŷ���
                // Outside�����ᵼ��GetTextPointerʧ�ܣ���Ȼʧ�ܣ�����Ҫ����SelectionRange
                if (scrollResult == OutsideScrollResult.None)
                {
                    return;
                }
            }

            #region Selection����ʼ�ַ��ͽ����ַ���ͬһ���ַ���ɶ������

            if (startPointer.CharacterIndex > -1 && endPointer.CharacterIndex > -1)
            {
                if (startPointer.CharacterIndex == endPointer.CharacterIndex)
                {
                    return;
                }
            }

            #endregion

            // ����ѡ�����ݵļ���ͼ�Σ�����Ⱦ
            this.textSelection.BuildGeometry();
            this.uiSyncContext.Send((state) =>
            {
                this.Surface.Draw(this.textSelection);
            }, null);
        }

        private void TerminalScreen_VTMouseUp(ITerminalScreen arg1, VTPoint cursorPos)
        {
            this.isMouseDown = false;
            this.selectionState = false;
        }

        private void TerminalScreen_VTMouseWheel(ITerminalScreen screen, bool upper)
        {
            if (upper)
            {
                // ���Ϲ���

                // ���ж��ǲ����Ѿ�����������
                if (this.ScrollAtTop)
                {
                    // ��������ֱ�ӷ���
                    return;
                }

                if (this.scrollValue < this.mouseOptions.ScrollDelta)
                {
                    // һ�ο���ȫ�����겢�һ���ʣ��
                    this.ScrollToHistory(0);
                }
                else
                {
                    this.ScrollToHistory(this.scrollValue - this.mouseOptions.ScrollDelta);
                }
            }
            else
            {
                // ���¹���

                if (this.ScrollAtBottom)
                {
                    // ��������ֱ�ӷ���
                    return;
                }

                // ʣ��������¹���������
                int remainScroll = this.scrollMax - this.scrollValue;

                if (remainScroll >= this.mouseOptions.ScrollDelta)
                {
                    this.ScrollToHistory(this.scrollValue + this.mouseOptions.ScrollDelta);
                }
                else
                {
                    // ֱ�ӹ�������
                    this.ScrollToHistory(this.scrollMax);
                }
            }
        }

        #endregion
    }
}
