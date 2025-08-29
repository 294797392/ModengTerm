using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.DataModels.Terminal;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using ModengTerm.Document;
using ModengTerm.Document.Enumerations;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using WPFToolkit.MVVM;
using Color = System.Windows.Media.Color;

namespace ModengTerm.ViewModel.CreateSession.TerminalOptions
{
    public class ThemeOptionsVM : OptionContentVM
    {
        #region 实例变量

        private Color backColor;
        private Color fontColor;
        private Color highlightFontColor;
        private Color highlightBackColor;
        private Color cursorColor;
        private Color selectionColor;
        private string backImageUri;
        private double backImageOpacity;
        private string backImageName;

        #endregion

        #region 属性

        /// <summary>
        /// 支持的主题列表
        /// </summary>
        public BindableCollection<ThemePackage> ThemeList { get; private set; }

        public BindableCollection<FontFamilyDefinition> FontFamilyList { get; private set; }
        public BindableCollection<FontSizeDefinition> FontSizeList { get; private set; }

        public BindableCollection<VTCursorStyles> CursorStyles { get; private set; }

        public BindableCollection<VTCursorSpeeds> CursorSpeeds { get; private set; }

        /// <summary>
        /// 光标颜色
        /// </summary>
        public Color CursorColor
        {
            get { return cursorColor; }
            set
            {
                if (cursorColor != value)
                {
                    cursorColor = value;
                    this.NotifyPropertyChanged("CursorColor");
                }
            }
        }

        /// <summary>
        /// 前景色
        /// </summary>
        public Color FontColor
        {
            get { return fontColor; }
            set
            {
                if (fontColor != value)
                {
                    fontColor = value;
                    this.NotifyPropertyChanged("FontColor");
                }
            }
        }

        /// <summary>
        /// 背景色
        /// </summary>
        public Color BackColor
        {
            get { return backColor; }
            set
            {
                if (backColor != value)
                {
                    backColor = value;
                    this.NotifyPropertyChanged("BackColor");
                }
            }
        }

        public Color HighlightBackColor
        {
            get { return highlightBackColor; }
            set
            {
                if (highlightBackColor != value)
                {
                    highlightBackColor = value;
                    this.NotifyPropertyChanged("HighlightBackColor");
                }
            }
        }

        public Color HighlightFontColor
        {
            get { return highlightFontColor; }
            set
            {
                if (highlightFontColor != value)
                {
                    highlightFontColor = value;
                    this.NotifyPropertyChanged("HighlightFontColor");
                }
            }
        }

        /// <summary>
        /// 选中颜色
        /// </summary>
        public Color SelectionColor
        {
            get { return selectionColor; }
            set
            {
                if (selectionColor != value)
                {
                    selectionColor = value;
                    this.NotifyPropertyChanged("SelectionColor");
                }
            }
        }

        public string BackImageUri
        {
            get { return backImageUri; }
            set
            {
                if (backImageUri != value)
                {
                    backImageUri = value;
                    this.NotifyPropertyChanged("BackImageUri");
                }
            }
        }

        public double BackImageOpacity
        {
            get { return backImageOpacity; }
            set
            {
                if (backImageOpacity != value)
                {
                    backImageOpacity = value;
                    this.NotifyPropertyChanged("BackImageOpacity");
                }
            }
        }

        public string BackImageName
        {
            get { return backImageName; }
            set
            {
                if (backImageName != value)
                {
                    backImageName = value;
                    this.NotifyPropertyChanged("BackImageName");
                }
            }
        }

        #endregion

        #region OptionContentVM

        public override void OnInitialize()
        {
            ClientManifest manifest = ClientContext.Context.Manifest;

            this.FontFamilyList = new BindableCollection<FontFamilyDefinition>();
            InstalledFontCollection installedFont = new InstalledFontCollection();
            this.FontFamilyList.AddRange(installedFont.Families.Select(v => new FontFamilyDefinition() { Name = v.Name, Value = v.Name }));

            this.FontSizeList = new BindableCollection<FontSizeDefinition>();
            this.FontSizeList.AddRange(manifest.FontSizeList);

            this.CursorSpeeds = new BindableCollection<VTCursorSpeeds>();
            this.CursorSpeeds.AddRange(VTBaseUtils.GetEnumValues<VTCursorSpeeds>());

            this.CursorStyles = new BindableCollection<VTCursorStyles>();
            this.CursorStyles.AddRange(VTBaseUtils.GetEnumValues<VTCursorStyles>());

            this.ThemeList = new BindableCollection<ThemePackage>();
            this.ThemeList.AddRange(manifest.DefaultThemes);
            this.ThemeList.SelectionChanged += ThemeList_SelectionChanged;
            this.ThemeList.SelectedItem = ThemeList.FirstOrDefault();
        }

        public override void OnLoaded()
        {
        }

        public override void OnUnload()
        {
        }

        public override void OnRelease()
        {
            ThemeList.SelectionChanged -= this.ThemeList_SelectionChanged;
        }

        public override void LoadOptions(XTermSession session)
        {
        }

        public override bool SaveOptions(XTermSession session)
        {
            if (FontFamilyList.SelectedItem == null)
            {
                MTMessageBox.Info("请选择字体");
                return false;
            }

            if (FontSizeList.SelectedItem == null)
            {
                MTMessageBox.Info("请选择字号");
                return false;
            }

            if (ThemeList.SelectedItem == null)
            {
                MTMessageBox.Info("请选择主题");
                return false;
            }

            session.SetOption<string>(OptionKeyEnum.THEME_ID, ThemeList.SelectedItem.ID);
            session.SetOption<string>(OptionKeyEnum.THEME_FONT_FAMILY, FontFamilyList.SelectedItem.Value);
            session.SetOption<int>(OptionKeyEnum.THEME_FONT_SIZE, FontSizeList.SelectedItem.Value);

            // 背景颜色和图片
            if (!string.IsNullOrEmpty(backImageUri))
            {
                byte[] imageBytes = File.ReadAllBytes(backImageUri);
                string imageBase64 = Convert.ToBase64String(imageBytes);
                session.SetOption<string>(OptionKeyEnum.THEME_BACKGROUND_IMAGE_DATA, imageBase64);
            }
            session.SetOption<double>(OptionKeyEnum.THEME_BACKGROUND_IMAGE_OPACITY, backImageOpacity);
            session.SetOption<string>(OptionKeyEnum.THEME_BACK_COLOR, VTBaseUtils.Color2RgbKey(BackColor));

            session.SetOption<string>(OptionKeyEnum.THEME_FONT_COLOR, VTBaseUtils.Color2RgbKey(FontColor));
            session.SetOption<int>(OptionKeyEnum.THEME_CURSOR_STYLE, (int)CursorStyles.SelectedItem);
            session.SetOption<int>(OptionKeyEnum.THEME_CURSOR_SPEED, (int)CursorSpeeds.SelectedItem);
            session.SetOption<string>(OptionKeyEnum.THEME_CURSOR_COLOR, VTBaseUtils.Color2RgbKey(CursorColor));
            session.SetOption<VTColorTable>(OptionKeyEnum.TEHEM_COLOR_TABLE, ThemeList.SelectedItem.ColorTable);
            session.SetOption<string>(OptionKeyEnum.THEME_FIND_HIGHLIGHT_BACKCOLOR, VTBaseUtils.Color2RgbKey(HighlightBackColor));
            session.SetOption<string>(OptionKeyEnum.THEME_FIND_HIGHLIGHT_FORECOLOR, VTBaseUtils.Color2RgbKey(highlightFontColor));

            session.SetOption<string>(OptionKeyEnum.THEME_SELECTION_COLOR, VTBaseUtils.Color2RgbKey(SelectionColor));

            return true;
        }

        #endregion

        #region 实例方法

        private void SwitchTheme(ThemePackage theme)
        {
            FontFamilyList.SelectedItem = FontFamilyList.FirstOrDefault();
            FontSizeList.SelectedItem = FontSizeList.FirstOrDefault(v => v.Value == theme.FontSize);

            CursorSpeeds.SelectedItem = VTBaseConsts.DefaultCursorBlinkSpeed;

            CursorStyles.SelectedItem = VTBaseConsts.DefaultCursorStyle;

            CursorColor = VTBaseUtils.RgbKey2Color(theme.CursorColor);
            FontColor = VTBaseUtils.RgbKey2Color(theme.FontColor);

            BackColor = VTBaseUtils.RgbKey2Color(theme.BackColor);

            HighlightBackColor = VTBaseUtils.RgbKey2Color(theme.HighlightBackColor);
            HighlightFontColor = VTBaseUtils.RgbKey2Color(theme.HighlightFontColor);

            SelectionColor = VTBaseUtils.RgbKey2Color(theme.SelectionColor);

            BackImageUri = theme.BackImageUri;
            BackImageOpacity = theme.BackImageOpacity;
        }

        #endregion

        #region 事件处理器

        private void ThemeList_SelectionChanged(ThemePackage oldValue, ThemePackage newValue)
        {
            if (newValue != null)
            {
                SwitchTheme(newValue);
            }
        }

        #endregion
    }
}
