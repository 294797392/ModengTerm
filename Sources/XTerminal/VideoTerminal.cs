using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using XTerminal.Base;
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
        private static readonly byte[] DA_DeviceAttributesResponse = new byte[7] { 0x1b, (byte)'[', (byte)'?', (byte)'1', (byte)':', (byte)'0', (byte)'c' };

        #endregion

        #region �����¼�

        public event Action<VideoTerminal, SessionStatusEnum> SessionStatusChanged;

        #endregion

        #region ʵ������

        /// <summary>
        /// ���ն˽���ͨ�ŵ��ŵ�
        /// </summary>
        private SessionBase session;

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

        private VTInitialOptions initialOptions;

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
        /// Ҳ�����ĵ���ĵ�һ����ʷ��¼��Row��ֵ
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
        /// <param name="options"></param>
        public void Initialize(VTInitialOptions options)
        {
            this.initialOptions = options;
            this.uiSyncContext = SynchronizationContext.Current;

            // DECAWM
            this.autoWrapMode = this.initialOptions.TerminalProperties.DECPrivateAutoWrapMode;

            // ��ʼ������
            this.historyLines = new Dictionary<int, VTHistoryLine>();
            this.TextOptions = new VTextOptions();
            this.textSelection = new VTextSelection();

            this.isRunning = true;

            #region ��ʼ������

            this.Keyboard = new VTKeyboard();
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

            #endregion

            #region ��ʼ���ĵ�ģ��

            VTDocumentOptions documentOptions = new VTDocumentOptions()
            {
                ColumnSize = initialOptions.TerminalProperties.Columns,
                RowSize = initialOptions.TerminalProperties.Rows,
                DECPrivateAutoWrapMode = initialOptions.TerminalProperties.DECPrivateAutoWrapMode,
                CursorStyle = initialOptions.CursorOption.Style,
                Interval = initialOptions.CursorOption.Interval,
                CanvasCreator = this.TerminalScreen
            };
            this.mainDocument = new VTDocument(documentOptions) { Name = "MainDocument" };
            this.alternateDocument = new VTDocument(documentOptions) { Name = "AlternateDocument" };
            this.activeDocument = this.mainDocument;
            this.firstHistoryLine = VTHistoryLine.Create(0, null, this.ActiveLine);
            this.historyLines[0] = this.firstHistoryLine;
            this.lastHistoryLine = this.firstHistoryLine;
            this.TerminalScreen.AddSurface(this.activeDocument.Surface);

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

            SessionBase session = SessionFactory.Create(options);
            session.StatusChanged += this.VTSession_StatusChanged;
            session.DataReceived += this.VTSession_DataReceived;
            session.Connect();
            this.session = session;

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

            this.cursorBlinkingThread.Join();

            this.session.StatusChanged -= this.VTSession_StatusChanged;
            this.session.DataReceived -= this.VTSession_DataReceived;
            this.session.Disconnect();

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

            string text = this.textSelection.GetText();

            // ���ü�����API���Ƶ�������
            Clipboard.SetText(text);
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

            this.textSelection.Start.LineHit = this.firstHistoryLine;
            this.textSelection.Start.CharacterIndex = 0;

            this.textSelection.End.LineHit = this.lastHistoryLine;
            this.textSelection.End.CharacterIndex = this.lastHistoryLine.Text.Length - 1;
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
            string text = this.textSelection.GetText();
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
                        this.session.Write(OS_OperationStatusResponse);
                        break;
                    }

                case StatusType.CPR_CursorPositionReport:
                    {
                        // Result is CSI r ; c R
                        int cursorRow = this.CursorRow;
                        int cursorCol = this.CursorCol;
                        CPR_CursorPositionReportResponse[2] = (byte)cursorRow;
                        CPR_CursorPositionReportResponse[4] = (byte)cursorCol;
                        this.session.Write(CPR_CursorPositionReportResponse);
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
        /// ע������ֻ�Ǹ���UI�ϵĹ�����λ�ã�������ʵ�ʵ�ȥ����
        /// </param>
        private void DrawDocument(VTDocument document, int scrollValue = -1)
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

                    if (arrangeDirty)
                    {
                        this.Surface.Arrange(next, next.OffsetX, next.OffsetY);
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
                this.Surface.Arrange(this.Cursor, this.Cursor.OffsetX, this.Cursor.OffsetY);

                #endregion

                #region �ƶ�������

                if (scrollValue != -1)
                {
                    this.TerminalScreen.ScrollTo(scrollValue);
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
        /// ��Ⱦ�û�ѡ�е�����
        /// </summary>
        /// <param name="selection"></param>
        private void DrawTextSelection(VTextSelection selection)
        {
            //VTextPointer startPointer = selection.Start;
            //VTextPointer endPointer = selection.End;

            //// �ж���ʼλ�û��߽���λ���Ƿ���Surface��

            //// ���������ƶ�����
            //TextPointerPositions pointerPosition = this.GetTextPointerPosition(startPointer, endPointer);

            //switch (pointerPosition)
            //{
            //    case TextPointerPositions.Original:
            //        {
            //            break;
            //        }

            //    // �������������ͬһ�����ƶ�
            //    case TextPointerPositions.Right:
            //    case TextPointerPositions.Left:
            //        {
            //            VTRect rect1 = startPointer.CharacterBounds;
            //            VTRect rect2 = endPointer.CharacterBounds;

            //            double xmin = Math.Min(rect1.Left, rect2.Left);
            //            double xmax = Math.Max(rect1.Right, rect2.Right);
            //            double x = xmin;
            //            double y = rect1.Y;
            //            double width = xmax - xmin;
            //            double height = rect1.Height;

            //            VTRect bounds = new VTRect(x, y, width, height);
            //            this.textSelection.Ranges.Clear();
            //            this.textSelection.Ranges.Add(bounds);
            //            break;
            //        }

            //    // ����������������ƶ�
            //    default:
            //        {
            //            this.textSelection.Ranges.Clear();

            //            // �����ϱߺ��±ߵľ���
            //            VTextPointer topPointer = startPointer.Row < endPointer.Row ? startPointer : endPointer;
            //            VTextPointer bottomPointer = startPointer.Row < endPointer.Row ? endPointer : startPointer;

            //            //logger.FatalFormat("top = {0}, bottom = {1}", topPointer.Row, bottomPointer.Row);

            //            // �����Panel����ʼѡ�б߽��ͽ���ѡ�еı߽��
            //            VTRect topBounds = topPointer.CharacterBounds;
            //            VTRect bottomBounds = bottomPointer.CharacterBounds;

            //            // �������Panel���ʱ��Ҳ����һ���ڹ�����һ����ѡ�У�����Ҫ�����⴦��
            //            if (scrollResult == OutsideScrollResult.ScrollTop)
            //            {
            //                // ������Ϲ���

            //                // ����Ѿ��ƶ������������ˣ�Ҫ�޸�topBoundsλ��Ϊ��һ��
            //                topBounds.Y = 0;

            //                //logger.FatalFormat("bottom:{0}, top:{1}", bottomPointer.Row, topPointer.Row);

            //                if (!this.UpdateTextPointerBounds(bottomPointer))
            //                {
            //                    // Pointer����ʧ�ܣ�˵��Pointer��ָ������Ѿ��������ĵ����ˣ���ô��bottom���ַ�λ�øĳ����½�
            //                    bottomBounds.X = this.surfaceRect.Width;
            //                    bottomBounds.Y = this.surfaceRect.Height;
            //                    //logger.FatalFormat("����ʧ��");
            //                }
            //                else
            //                {
            //                    // ���³ɹ���ˢ��bottomBounds
            //                    bottomBounds = bottomPointer.CharacterBounds;
            //                }
            //            }
            //            else if (scrollResult == OutsideScrollResult.ScrollDown)
            //            {
            //                // ������¹���

            //                // ����Ѿ��ƶ������������ˣ�Ҫ�޸�bottomBoundsλ��Ϊ���һ��
            //                bottomBounds.Y = this.surfaceRect.Height;

            //                if (!this.UpdateTextPointerBounds(topPointer))
            //                {
            //                    topBounds.X = 0;
            //                    topBounds.Y = 0;
            //                }
            //                else
            //                {
            //                    topBounds = topPointer.CharacterBounds;
            //                }
            //            }

            //            this.textSelection.Ranges.Add(new VTRect(topBounds.X, topBounds.Y, 9999, topBounds.Height));
            //            this.textSelection.Ranges.Add(new VTRect(0, bottomBounds.Y, bottomBounds.X + bottomBounds.Width, bottomBounds.Height));

            //            // �����м�ļ���ͼ��
            //            VTRect middleBounds = new VTRect(0, topBounds.Y + topBounds.Height, 9999, bottomBounds.Y - topBounds.Bottom);
            //            this.textSelection.Ranges.Add(middleBounds);
            //            break;
            //        }
            //}

            //this.uiSyncContext.Send((state) =>
            //{
            //    this.Surface.Draw(this.textSelection);
            //}, null);
        }

        /// <summary>
        /// ������ָ������ʷ��¼
        /// ������UI�ϵĹ�����λ��
        /// </summary>
        /// <param name="scrollValue">Ҫ��ʾ�ĵ�һ����ʷ��¼</param>
        private void ScrollToHistory(int scrollValue)
        {
            this.scrollValue = scrollValue;

            // �ն˿�����ʾ��������
            int terminalRows = this.initialOptions.TerminalProperties.Rows;

            VTHistoryLine historyLine;
            if (!this.historyLines.TryGetValue(scrollValue, out historyLine))
            {
                logger.ErrorFormat("ScrollToʧ��, �Ҳ�����Ӧ��VTHistoryLine, scrollValue = {0}", scrollValue);
                return;
            }

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

            this.activeDocument.SetArrangeDirty(true);
            this.DrawDocument(this.activeDocument, scrollValue);
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
                logger.ErrorFormat("GetTextPointerʧ��, ��������ʷ�м�¼, currentScroll = {0}", this.scrollValue);
                return false;
            }

            double lineOffsetY;
            VTHistoryLine lineHit = VTextSelectionHelper.HitTestVTextLine(topHistoryLine, mouseY, out lineOffsetY);
            if (lineHit == null)
            {
                // ����˵�����û�����κ�һ����
                logger.ErrorFormat("û���ҵ����λ�ö�Ӧ����, cursorY = {0}", mouseY);
                return false;
            }

            pointer.LineHit = lineHit;

            #endregion

            #region �ټ�������������ĸ��ַ���

            int characterIndex;
            VTRect characterBounds;
            if (!VTextSelectionHelper.HitTestVTCharacter(this.Surface, lineHit, mouseX, out characterIndex, out characterBounds))
            {
                return false;
            }

            pointer.CharacterIndex = characterIndex;

            #endregion

            return true;
        }

        /// <summary>
        /// ��ȡpointer2�����pointer1�ķ���
        /// </summary>
        /// <param name="pointer1">��һ��pointer</param>
        /// <param name="pointer2">�ڶ���pointer</param>
        /// <returns></returns>
        private TextPointerPositions GetTextPointerPosition(VTextPointer pointer1, VTextPointer pointer2)
        {
            double p1x = pointer1.CharacterIndex;
            double p2x = pointer2.CharacterIndex;
            int row1 = pointer1.Row;
            int row2 = pointer2.Row;

            if (p2x == p1x && row2 < row1)
            {
                return TextPointerPositions.Top;
            }
            else if (p2x > p1x && row2 < row1)
            {
                return TextPointerPositions.RightTop;
            }
            else if (p2x > p1x && row2 == row1)
            {
                return TextPointerPositions.Right;
            }
            else if (p2x > p1x && row2 > row1)
            {
                return TextPointerPositions.RightBottom;
            }
            else if (p2x == p1x && row2 > row1)
            {
                return TextPointerPositions.Bottom;
            }
            else if (p2x < p1x && row2 > row1)
            {
                return TextPointerPositions.LeftBottom;
            }
            else if (p2x < p1x && row2 == row1)
            {
                return TextPointerPositions.Left;
            }
            else if (p2x < p1x && row2 < row1)
            {
                return TextPointerPositions.LeftTop;
            }
            else
            {
                return TextPointerPositions.Original;
            }
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

            // ��������Ķ��Ǽ��̰���
            int code = this.session.Write(bytes);
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
                        logger.DebugFormat("Print:{0}, cursorRow = {1}, cursorCol = {2}", ch, this.CursorRow, this.CursorCol);
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
                        logger.DebugFormat("CarriageReturn, cursorRow = {0}, cursorCol = {1}", this.CursorRow, this.CursorCol);
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
                        logger.DebugFormat("LineFeed, cursorRow = {0}, cursorCol = {1}, {2}", this.CursorRow, this.CursorCol, action);

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
                            int historyIndex = this.lastHistoryLine.Row + 1;
                            VTHistoryLine historyLine = VTHistoryLine.Create(historyIndex, this.lastHistoryLine, this.ActiveLine);
                            this.historyLines[historyIndex] = historyLine;
                            this.lastHistoryLine = historyLine;

                            // ��������������
                            int terminalRows = this.initialOptions.TerminalProperties.Rows;
                            int scrollMax = historyIndex - terminalRows + 1;
                            if (scrollMax > 0)
                            {
                                this.scrollMax = scrollMax;
                                this.scrollValue = scrollMax;
                                logger.DebugFormat("scrollMax = {0}", scrollMax);
                                this.uiSyncContext.Send((state) =>
                                {
                                    this.TerminalScreen.UpdateScrollInfo(scrollMax);
                                    this.TerminalScreen.ScrollToEnd(ScrollOrientation.Down);
                                }, null);
                            }
                        }

                        break;
                    }

                case VTActions.RI_ReverseLineFeed:
                    {
                        // ��LineFeed�෴��Ҳ���ǰѹ��������һ��λ��
                        // ����man�����ʱ��ᴥ�����ָ��
                        // ������ �C ִ��\n�ķ������������������ƶ�һ�У�ά��ˮƽλ�ã����б�Ҫ������������ *
                        this.activeDocument.ReverseLineFeed();
                        logger.DebugFormat("ReverseLineFeed");
                        break;
                    }

                #region Erase

                case VTActions.EL_EraseLine:
                    {
                        List<int> parameters = parameter as List<int>;
                        EraseType eraseType = (EraseType)VTParameter.GetParameter(parameters, 0, 0);
                        logger.DebugFormat("EL_EraseLine, eraseType = {0}, cursorRow = {1}, cursorCol = {2}", eraseType, this.CursorRow, this.CursorCol);
                        this.activeDocument.EraseLine(this.ActiveLine, this.CursorCol, eraseType);
                        break;
                    }

                case VTActions.ED_EraseDisplay:
                    {
                        List<int> parameters = parameter as List<int>;
                        EraseType eraseType = (EraseType)VTParameter.GetParameter(parameters, 0, 0);
                        logger.DebugFormat("ED_EraseDisplay, eraseType = {0}, cursorRow = {1}, cursorCol = {2}", eraseType, this.CursorRow, this.CursorCol);
                        this.activeDocument.EraseDisplay(this.ActiveLine, this.CursorCol, eraseType);
                        break;
                    }

                #endregion

                #region ����ƶ�

                // ����Ĺ���ƶ�ָ��ܽ���VTDocument�Ĺ���
                // �����ƶ�����������ڿ��������ڵ�����

                case VTActions.BS:
                    {
                        this.activeDocument.SetCursor(this.CursorRow, this.CursorCol - 1);
                        logger.DebugFormat("CursorBackward, cursorRow = {0}, cursorCol = {1}", this.CursorRow, this.CursorCol);
                        break;
                    }

                case VTActions.CursorBackward:
                    {
                        List<int> parameters = parameter as List<int>;
                        int n = VTParameter.GetParameter(parameters, 0, 1);
                        this.activeDocument.SetCursor(this.CursorRow, this.CursorCol - n);
                        logger.DebugFormat("CursorBackward, cursorRow = {0}, cursorCol = {1}", this.CursorRow, this.CursorCol);
                        break;
                    }

                case VTActions.CUF_CursorForward:
                    {
                        List<int> parameters = parameter as List<int>;
                        int n = VTParameter.GetParameter(parameters, 0, 1);
                        this.activeDocument.SetCursor(this.CursorRow, this.CursorCol + n);
                        break;
                    }

                case VTActions.CUU_CursorUp:
                    {
                        List<int> parameters = parameter as List<int>;
                        int n = VTParameter.GetParameter(parameters, 0, 1);
                        this.activeDocument.SetCursor(this.CursorRow - n, this.CursorCol);
                        break;
                    }

                case VTActions.CUD_CursorDown:
                    {
                        List<int> parameters = parameter as List<int>;
                        int n = VTParameter.GetParameter(parameters, 0, 1);
                        this.activeDocument.SetCursor(this.CursorRow + n, this.CursorCol);
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

                        logger.DebugFormat("CUP_CursorPosition, row = {0}, col = {1}", row, col);
                        this.activeDocument.SetCursor(row, col);
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
                        logger.DebugFormat("DECANM_AnsiMode, enable = {0}", enable);
                        this.Keyboard.SetAnsiMode(enable);
                        break;
                    }

                case VTActions.DECCKM_CursorKeysMode:
                    {
                        bool enable = Convert.ToBoolean(parameter);
                        logger.DebugFormat("DECCKM_CursorKeysMode, enable = {0}", enable);
                        this.Keyboard.SetCursorKeyMode(enable);
                        break;
                    }

                case VTActions.DECKPAM_KeypadApplicationMode:
                    {
                        logger.DebugFormat("DECKPAM_KeypadApplicationMode");
                        this.Keyboard.SetKeypadMode(true);
                        break;
                    }

                case VTActions.DECKPNM_KeypadNumericMode:
                    {
                        logger.DebugFormat("DECKPNM_KeypadNumericMode");
                        this.Keyboard.SetKeypadMode(false);
                        break;
                    }

                case VTActions.DECAWM_AutoWrapMode:
                    {
                        bool enable = Convert.ToBoolean(parameter);
                        this.autoWrapMode = enable;
                        logger.DebugFormat("DECAWM_AutoWrapMode, enable = {0}", enable);
                        this.activeDocument.DECPrivateAutoWrapMode = enable;
                        break;
                    }

                case VTActions.XTERM_BracketedPasteMode:
                    {
                        this.xtermBracketedPasteMode = Convert.ToBoolean(parameter);
                        logger.ErrorFormat("δʵ��XTERM_BracketedPasteMode");
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
                        logger.ErrorFormat("DCH_DeleteCharacter, {0}, cursorPos = {1}", count, this.CursorCol);
                        this.activeDocument.DeleteCharacter(this.ActiveLine, this.CursorCol, count);
                        break;
                    }

                case VTActions.ICH_InsertCharacter:
                    {
                        // �ڵ�ǰ��괦����N���հ��ַ�,��Ὣ���������ı��Ƶ��Ҳࡣ ���������Ļ���ı��ᱻɾ��
                        // Ŀǰû��������������ն���ʾ��ʲôӰ�죬������ʱ��ʵ��
                        List<int> parameters = parameter as List<int>;
                        int count = VTParameter.GetParameter(parameters, 0, 1);
                        logger.ErrorFormat("δʵ��InsertCharacters, {0}, cursorPos = {1}", count, this.CursorCol);
                        break;
                    }

                #endregion

                #region ���¹���

                #endregion

                case VTActions.UseAlternateScreenBuffer:
                    {
                        logger.DebugFormat("UseAlternateScreenBuffer");

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
                        logger.DebugFormat("UseMainScreenBuffer");

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
                        logger.DebugFormat("DSR_DeviceStatusReport, statusType = {0}", statusType);
                        this.PerformDeviceStatusReport(statusType);
                        break;
                    }

                case VTActions.DA_DeviceAttributes:
                    {
                        logger.DebugFormat("DA_DeviceAttributes");
                        this.session.Write(DA_DeviceAttributesResponse);
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
                        int lines = this.initialOptions.TerminalProperties.Rows;

                        List<int> parameters = parameter as List<int>;
                        int topMargin = VTParameter.GetParameter(parameters, 0, 1);
                        int bottomMargin = VTParameter.GetParameter(parameters, 1, lines);

                        if (bottomMargin < 0 || topMargin < 0)
                        {
                            logger.ErrorFormat("DECSTBM_SetScrollingRegion��������ȷ�����Ա�������, topMargin = {0}, bottomMargin = {1}", topMargin, bottomMargin);
                            return;
                        }
                        if (topMargin >= bottomMargin)
                        {
                            logger.ErrorFormat("DECSTBM_SetScrollingRegion��������ȷ��topMargin���ڵ�bottomMargin�����Ա�������, topMargin = {0}, bottomMargin = {1}", topMargin, bottomMargin);
                            return;
                        }
                        if (bottomMargin > lines)
                        {
                            logger.DebugFormat("DECSTBM_SetScrollingRegion��������ȷ��bottomMargin���ڵ�ǰ��Ļ������, bottomMargin = {0}, lines = {1}", bottomMargin, lines);
                            return;
                        }

                        // ���topMargin����1����ô�ͱ�ʾʹ��Ĭ��ֵ��Ҳ����û��marginTop�����Ե�topMargin == 1��ʱ��marginTop��Ϊ0
                        int marginTop = topMargin == 1 ? 0 : topMargin;
                        // ���bottomMargin���ڿ���̨�߶ȣ���ô�ͱ�ʾʹ��Ĭ��ֵ��Ҳ����û��marginBottom�����Ե�bottomMargin == ����̨�߶ȵ�ʱ��marginBottom��Ϊ0
                        int marginBottom = lines - bottomMargin;
                        logger.DebugFormat("SetScrollingRegion, topMargin = {0}, bottomMargin = {1}", marginTop, marginBottom);
                        this.activeDocument.SetScrollMargin(marginTop, marginBottom);
                        break;
                    }

                case VTActions.IL_InsertLine:
                    {
                        // �� <n> �в�����λ�õĻ������� ������ڵ��м����·����н������ƶ���
                        List<int> parameters = parameter as List<int>;
                        int lines = VTParameter.GetParameter(parameters, 0, 1);
                        logger.DebugFormat("IL_InsertLine, lines = {0}", lines);
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
                        logger.DebugFormat("DL_DeleteLine, lines = {0}", lines);
                        if (lines > 0)
                        {
                            this.activeDocument.DeleteLines(this.ActiveLine, lines);
                        }
                        break;
                    }

                default:
                    {
                        logger.WarnFormat("δִ�е�VTAction, {0}", action);
                        break;
                    }
            }
        }

        private void VTSession_DataReceived(SessionBase client, byte[] bytes)
        {
            //string str = string.Join(",", bytes.Select(v => v.ToString()).ToList());
            //logger.InfoFormat("Received, {0}", str);
            this.vtParser.ProcessCharacters(bytes);

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
                this.selectionState = true;
                this.textSelection.Reset();
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

            // ���ȼ������Ƿ��������߽�������
            OutsideScrollResult scrollResult = this.ScrollIfCursorOutsideSurface(mousePosition, this.surfaceRect);

            // ����˼·�������StartTextPointer��EndTextPointer֮��ļ���ͼ��
            // Ȼ����Ⱦ����ͼ�Σ�SelectionRange�����Ͼ���һ�Ѿ���
            VTextPointer startPointer = this.textSelection.Start;
            VTextPointer endPointer = this.textSelection.End;

            // ��갴�µ�ʱ�������ϣ�Ȼ������ƶ���������
            // ��Ҫˢ����ʼ�����������Ϣ
            if (startPointer.LineHit == null)
            {
                if (!this.GetTextPointer(mousePosition, this.surfaceRect, startPointer))
                {
                    return;
                }
            }

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

            this.DrawTextSelection(this.textSelection);
        }

        private void TerminalScreen_VTMouseUp(ITerminalScreen arg1, VTPoint cursorPos)
        {
            this.isMouseDown = false;
            this.selectionState = false;
        }

        #endregion
    }
}
