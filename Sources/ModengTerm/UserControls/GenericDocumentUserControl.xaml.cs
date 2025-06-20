using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Document;
using ModengTerm.Terminal;
using ModengTerm.Terminal.Renderer;
using ModengTerm.UserControls.TerminalUserControls;
using System;
using System.Windows.Controls;

namespace ModengTerm.UserControls
{
    /// <summary>
    /// GenericDocumentUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class GenericDocumentUserControl : UserControl
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("GenericDocumentUserControl");

        #endregion

        #region 实例变量

        private TextRenderer stringRenderer;
        private VTDocument document;

        #endregion

        #region 构造方法

        public GenericDocumentUserControl()
        {
            InitializeComponent();
        }

        #endregion

        #region 公开接口

        public void Initialize(XTermSession session)
        {
            string background = session.GetOption<string>(OptionKeyEnum.THEME_BACKGROUND_COLOR);
            double padding = session.GetOption<double>(OptionKeyEnum.SSH_THEME_DOCUMENT_PADDING);
            double width = DocumentControl.ActualWidth - padding * 2;
            double height = DocumentControl.ActualHeight - padding * 2;

            DocumentControl.Padding = new System.Windows.Thickness(padding);
            DocumentControl.Background = DrawingUtils.GetBrush(background);

            VTDocumentOptions documentOptions = VTermUtils.CreateDocumentOptions(Guid.NewGuid().ToString(), width, height, session, DocumentControl);
            this.document = new VTDocument(documentOptions);
            this.document.Initialize();
        }

        public void Release()
        {
        }

        public void DrawText(string text)
        {
        }

        #endregion
    }
}
