using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Document;
using ModengTerm.Document.Drawing;
using ModengTerm.Document.Enumerations;
using ModengTerm.Document.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base.Definitions;

namespace ModengTerm.Terminal
{
    /// <summary>
    /// 维护屏幕缓冲区
    /// </summary>
    public abstract class VTScreen
    {
        #region 实例变量

        protected VTDocument document;
        private bool canScroll;
        private VTDocumentOptions documentOptions;
        private VTOptions vtOptions;

        #endregion

        #region 属性

        public abstract bool IsAlternate { get; }

        public int ViewportRow { get { return this.document.ViewportRow; } }

        public int ViewportColumn { get { return this.document.ViewportColumn; } }

        public VTextLine ActiveLine { get { return this.document.ActiveLine; } }

        public VTCursor Cursor { get { return this.document.Cursor; } }

        public VTScrollInfo ScrollInfo { get { return this.document.Scrollbar; } }

        public VTextLine FirstLine { get { return this.document.FirstLine; } }

        public VTypeface Typeface { get { return this.document.Typeface; } }

        /// <summary>
        /// 可以滚动的行数
        /// </summary>
        public int ScrollbackMax { get { return this.document.Scrollbar.ScrollbackMax; } set { this.document.Scrollbar.ScrollbackMax = value; } }

        #endregion

        #region 构造方法

        public VTScreen(VTOptions options)
        {
            this.vtOptions = options;
        }

        #endregion

        #region 公开接口

        public void Initialize()
        {
            string name = this.IsAlternate ? "AlternateDocument" : "MainDocument";
            IDrawingDocument drawingDocument = this.IsAlternate ? this.vtOptions.AlternateDocument : this.vtOptions.MainDocument;

            this.documentOptions = this.CreateDocumentOptions(name, this.vtOptions.Session, drawingDocument);
            this.document = new VTDocument(this.documentOptions);
            this.document.Initialize();

            this.OnInitialize();
        }

        public void Release()
        {
            // 先释放子类资源
            this.OnRelease();

            // TODO：最后释放父类资源
            this.document.Release();
        }

        public void SetVisible(bool visible)
        {
            this.document.SetVisible(visible);
        }

        public void Render()
        {
            this.document.RequestInvalidate();
        }

        #endregion

        #region 抽象方法

        protected abstract void OnInitialize();

        protected abstract void OnRelease();

        #endregion

        #region 实例方法

        private VTDocumentOptions CreateDocumentOptions(string name, XTermSession sessionInfo, IDrawingDocument drawingDocument)
        {
            string fontFamily = sessionInfo.GetOption<string>(OptionKeyEnum.THEME_FONT_FAMILY);
            double fontSize = sessionInfo.GetOption<double>(OptionKeyEnum.THEME_FONT_SIZE);

            VTypeface typeface = drawingDocument.GetTypeface(fontSize, fontFamily);
            typeface.BackgroundColor = sessionInfo.GetOption<string>(OptionKeyEnum.THEME_BACKGROUND_COLOR);
            typeface.ForegroundColor = sessionInfo.GetOption<string>(OptionKeyEnum.THEME_FONT_COLOR);

            VTSize terminalSize = drawingDocument.Size;
            double contentMargin = sessionInfo.GetOption<double>(OptionKeyEnum.SSH_THEME_CONTENT_MARGIN);
            VTSize contentSize = new VTSize(terminalSize.Width - 15, terminalSize.Height).Offset(-contentMargin * 2);
            TerminalSizeModeEnum sizeMode = sessionInfo.GetOption<TerminalSizeModeEnum>(OptionKeyEnum.SSH_TERM_SIZE_MODE);

            int viewportRow = sessionInfo.GetOption<int>(OptionKeyEnum.SSH_TERM_ROW);
            int viewportColumn = sessionInfo.GetOption<int>(OptionKeyEnum.SSH_TERM_COL);
            if (sizeMode == TerminalSizeModeEnum.AutoFit)
            {
                /// 如果SizeMode等于Fixed，那么就使用DefaultViewportRow和DefaultViewportColumn
                /// 如果SizeMode等于AutoFit，那么动态计算行和列
                VTUtils.CalculateAutoFitSize(contentSize, typeface, out viewportRow, out viewportColumn);
            }

            VTDocumentOptions documentOptions = new VTDocumentOptions()
            {
                Name = name,
                ViewportRow = viewportRow,
                ViewportColumn = viewportColumn,
                AutoWrapMode = false,
                CursorStyle = sessionInfo.GetOption<VTCursorStyles>(OptionKeyEnum.THEME_CURSOR_STYLE),
                CursorColor = sessionInfo.GetOption<string>(OptionKeyEnum.THEME_CURSOR_COLOR),
                CursorSpeed = sessionInfo.GetOption<VTCursorSpeeds>(OptionKeyEnum.THEME_CURSOR_SPEED),
                ScrollDelta = sessionInfo.GetOption<int>(OptionKeyEnum.MOUSE_SCROLL_DELTA),
                ScrollbackMax = sessionInfo.GetOption<int>(OptionKeyEnum.TERM_MAX_SCROLLBACK),
                Typeface = typeface,
                DrawingObject = drawingDocument,
                SelectionColor = sessionInfo.GetOption<string>(OptionKeyEnum.TERM_SELECTION_COLOR),
            };

            return documentOptions;
        }

        #endregion
    }
}
