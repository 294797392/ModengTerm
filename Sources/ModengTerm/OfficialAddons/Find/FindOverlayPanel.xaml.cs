using ModengTerm.Addon;
using ModengTerm.Addon.Controls;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addon.Service;
using ModengTerm.Base;
using ModengTerm.Base.Enumerations;
using ModengTerm.Document;
using ModengTerm.Document.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModengTerm.OfficialAddons.Find
{
    /// <summary>
    /// FindWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FindOverlayPanel : OverlayPanel
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("FindOverlayPanel");

        #endregion

        #region 实例变量

        private ClientFactory factory;
        private IClientEventRegistry eventRegistory;

        private IDrawingContext drawingContext;
        /// <summary>
        /// 用来高亮显示匹配结果的矩形
        /// </summary>
        private GraphicsObject highlightObject;
        private VTColor backColor;

        private Brush textBoxBorderBrush;

        #endregion

        #region 构造方法

        public FindOverlayPanel()
        {
            InitializeComponent();

            this.InitializePanel();
        }

        #endregion

        #region 实例方法

        private void InitializePanel()
        {
            this.factory = ClientFactory.GetFactory();
            this.eventRegistory = this.factory.GetEventRegistry();
            this.textBoxBorderBrush = TextBoxKeyword.BorderBrush;
            TextBoxKeyword.BorderBrush = Brushes.Red;
        }

        private void PerformFind()
        {
            FindOptions options = new FindOptions()
            {
                CaseSensitive = CheckBoxCaseSensitive.IsChecked.Value,
                Regexp = CheckBoxRegexp.IsChecked.Value,
                Keyword = TextBoxKeyword.Text
            };
            List<VTextRange> textRanges = this.OwnerTab.FindMatches(options);
            if (textRanges == null || textRanges.Count == 0)
            {
                this.highlightObject.Clear();
                TextBoxKeyword.BorderBrush = Brushes.Red;
                return;
            }

            TextBoxKeyword.BorderBrush = this.textBoxBorderBrush;

            List<VTRect> rectangles = textRanges.Select(v => v.GetVTRect()).ToList();
            this.highlightObject.DrawRectangles(rectangles, null, this.backColor);
        }

        #endregion

        #region 事件处理器

        private void ButtonFind_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ButtonFindAll_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ButtonNextMatches_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TextBoxKeyword_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.PerformFind();
        }

        private void CheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            this.PerformFind();
        }

        #endregion

        #region OverlayPanelHost

        public override void OnInitialize()
        {
            this.eventRegistory.SubscribeTabEvent(TabEvent.SHELL_RENDERED, this.OnShellRendered, this.OwnerTab);

            string backColorRgbKey = this.OwnerTab.GetOption<string>(OptionKeyEnum.THEME_FIND_HIGHLIGHT_BACKCOLOR, OptionDefaultValues.THEME_FIND_HIGHLIGHT_BACKCOLOR);
            this.backColor = VTColor.CreateFromRgbKey(backColorRgbKey);
            this.drawingContext = this.OwnerTab.DrawingContext;
            this.highlightObject = this.drawingContext.CreateGraphicsObject();
        }

        public override void OnRelease()
        {
            this.eventRegistory.UnsubscribeTabEvent(TabEvent.SHELL_RENDERED, this.OnShellRendered, this.OwnerTab);

            this.drawingContext.DeleteGraphicsObject(this.highlightObject);
        }

        public override void OnLoaded()
        {
            TextBoxKeyword.Focus();

            this.PerformFind();
        }

        public override void OnUnload()
        {
            this.highlightObject.Clear();
        }

        #endregion

        #region TabEvent

        private void OnShellRendered(TabEventArgs e, object userData)
        {
            throw new System.NotImplementedException();
            //if (this.OwnerPanel.IsOpened)
            //{
            //    this.PerformFind();
            //}
        }

        #endregion
    }
}
