using ModengTerm.Addon;
using ModengTerm.Addon.Controls;
using ModengTerm.Addon.Interactive;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Document;
using ModengTerm.Document.Graphics;
using ModengTerm.OfficialAddons.Broadcast;
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
            this.textBoxBorderBrush = TextBoxKeyword.BorderBrush;
            TextBoxKeyword.BorderBrush = Brushes.Red;
        }

        private void PerformFind()
        {
            IClientShellTab shellTab = this.Tab as IClientShellTab;

            FindOptions options = new FindOptions()
            {
                CaseSensitive = CheckBoxCaseSensitive.IsChecked.Value,
                Regexp = CheckBoxRegexp.IsChecked.Value,
                Keyword = TextBoxKeyword.Text
            };
            List<VTextRange> textRanges = shellTab.FindMatches(options);
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

        #region OverlayPanel

        public override void Initialize()
        {
            this.eventRegistry.SubscribeTabEvent(TabEvent.SHELL_RENDERED, this.OnShellRendered, this.Tab);

            string backColorRgbKey = this.Tab.GetOption<string>(PredefinedOptions.THEME_SELECTION_BACK_COLOR);
            this.backColor = VTColor.CreateFromRgbKey(backColorRgbKey);
            IClientShellTab shellTab = this.Tab as IClientShellTab;
            this.drawingContext = shellTab.DrawingContext;
            this.highlightObject = this.drawingContext.CreateGraphicsObject();
        }

        public override void Release()
        {
            this.eventRegistry.UnsubscribeTabEvent(TabEvent.SHELL_RENDERED, this.OnShellRendered, this.Tab);

            this.drawingContext.DeleteGraphicsObject(this.highlightObject);
        }

        public override void Load()
        {
            TextBoxKeyword.Focus();

            this.PerformFind();
        }

        public override void Unload()
        {
            this.highlightObject.Clear();
        }

        #endregion

        #region TabEvent

        private void OnShellRendered(TabEventArgs e, object userData)
        {
            if (this.Container.IsOpened)
            {
                this.PerformFind();
            }
        }

        #endregion
    }
}
