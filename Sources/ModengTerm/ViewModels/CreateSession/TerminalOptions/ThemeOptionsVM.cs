using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using ModengTerm.Document;
using ModengTerm.Document.Enumerations;
using ModengTerm.Terminal;
using ModengTerm.Terminal.DataModels;
using ModengTerm.UserControls.TerminalUserControls.Rendering;
using System;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Windows.Media;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels.CreateSession.TerminalOptions
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
            get { return this.cursorColor; }
            set
            {
                if (this.cursorColor != value)
                {
                    this.cursorColor = value;
                    this.NotifyPropertyChanged("CursorColor");
                }
            }
        }

        /// <summary>
        /// 前景色
        /// </summary>
        public Color FontColor
        {
            get { return this.fontColor; }
            set
            {
                if (this.fontColor != value)
                {
                    this.fontColor = value;
                    this.NotifyPropertyChanged("FontColor");
                }
            }
        }

        /// <summary>
        /// 背景色
        /// </summary>
        public Color BackColor
        {
            get { return this.backColor; }
            set
            {
                if (this.backColor != value)
                {
                    this.backColor = value;
                    this.NotifyPropertyChanged("BackColor");
                }
            }
        }

        public Color HighlightBackColor
        {
            get { return this.highlightBackColor; }
            set
            {
                if (this.highlightBackColor != value)
                {
                    this.highlightBackColor = value;
                    this.NotifyPropertyChanged("HighlightBackColor");
                }
            }
        }

        public Color HighlightFontColor
        {
            get { return this.highlightFontColor; }
            set
            {
                if (this.highlightFontColor != value)
                {
                    this.highlightFontColor = value;
                    this.NotifyPropertyChanged("HighlightFontColor");
                }
            }
        }

        /// <summary>
        /// 选中颜色
        /// </summary>
        public Color SelectionColor
        {
            get { return this.selectionColor; }
            set
            {
                if (this.selectionColor != value)
                {
                    this.selectionColor = value;
                    this.NotifyPropertyChanged("SelectionColor");
                }
            }
        }

        public string BackImageUri
        {
            get { return this.backImageUri; }
            set 
            {
                if (this.backImageUri != value) 
                {
                    this.backImageUri = value;
                    this.NotifyPropertyChanged("BackImageUri");
                }
            }
        }

        public double BackImageOpacity
        {
            get { return this.backImageOpacity; }
            set
            {
                if (this.backImageOpacity != value)
                {
                    this.backImageOpacity = value;
                    this.NotifyPropertyChanged("BackImageOpacity");
                }
            }
        }

        public string BackImageName
        {
            get { return this.backImageName; }
            set
            {
                if (this.backImageName != value)
                {
                    this.backImageName = value;
                    this.NotifyPropertyChanged("BackImageName");
                }
            }
        }

        #endregion

        #region OptionContentVM

        public override void OnInitialize()
        {
            MTermManifest manifest = MTermApp.Context.Manifest;

            this.ThemeList = new BindableCollection<ThemePackage>();
            this.ThemeList.AddRange(manifest.DefaultThemes);
            this.ThemeList.SelectionChanged += ThemeList_SelectionChanged;

            this.FontFamilyList = new BindableCollection<FontFamilyDefinition>();
            this.FontFamilyList.AddRange(manifest.FontFamilyList);

            this.FontSizeList = new BindableCollection<FontSizeDefinition>();
            this.FontSizeList.AddRange(manifest.FontSizeList);

            this.CursorSpeeds = new BindableCollection<VTCursorSpeeds>();
            this.CursorSpeeds.AddRange(VTBaseUtils.GetEnumValues<VTCursorSpeeds>());

            this.CursorStyles = new BindableCollection<VTCursorStyles>();
            this.CursorStyles.AddRange(VTBaseUtils.GetEnumValues<VTCursorStyles>());
        }

        public override void OnLoaded()
        {
            this.ThemeList.SelectedItem = this.ThemeList.FirstOrDefault();
        }

        public override void OnUnload()
        {
        }

        public override void OnRelease()
        {
            this.ThemeList.SelectionChanged -= this.ThemeList_SelectionChanged;
        }

        public override void LoadOptions(XTermSession session)
        {
        }

        public override bool SaveOptions(XTermSession session)
        {
            if (this.FontFamilyList.SelectedItem == null)
            {
                MTMessageBox.Info("请选择字体");
                return false;
            }

            if (this.FontSizeList.SelectedItem == null)
            {
                MTMessageBox.Info("请选择字号");
                return false;
            }

            if (this.ThemeList.SelectedItem == null)
            {
                MTMessageBox.Info("请选择主题");
                return false;
            }

            session.SetOption<string>(OptionKeyEnum.THEME_ID, this.ThemeList.SelectedItem.ID);
            session.SetOption<string>(OptionKeyEnum.THEME_FONT_FAMILY, this.FontFamilyList.SelectedItem.Value);
            session.SetOption<int>(OptionKeyEnum.THEME_FONT_SIZE, this.FontSizeList.SelectedItem.Value);

            // 背景颜色和图片
            byte[] imageBytes = File.ReadAllBytes(this.backImageUri);
            string imageBase64 = Convert.ToBase64String(imageBytes);
            session.SetOption<string>(OptionKeyEnum.THEME_BACKGROUND_IMAGE_DATA, imageBase64);
            session.SetOption<double>(OptionKeyEnum.THEME_BACKGROUND_IMAGE_OPACITY, this.backImageOpacity);
            session.SetOption<string>(OptionKeyEnum.THEME_BACKGROUND_COLOR, DrawingUtils.GetRgbKey(this.BackColor));

            session.SetOption<string>(OptionKeyEnum.THEME_FONT_COLOR, DrawingUtils.GetRgbKey(this.FontColor));
            session.SetOption<int>(OptionKeyEnum.THEME_CURSOR_STYLE, (int)this.CursorStyles.SelectedItem);
            session.SetOption<int>(OptionKeyEnum.THEME_CURSOR_SPEED, (int)this.CursorSpeeds.SelectedItem);
            session.SetOption<string>(OptionKeyEnum.THEME_CURSOR_COLOR, DrawingUtils.GetRgbKey(this.CursorColor));
            session.SetOption<VTColorTable>(OptionKeyEnum.TEHEM_COLOR_TABLE, this.ThemeList.SelectedItem.ColorTable);
            session.SetOption<string>(OptionKeyEnum.THEME_FIND_HIGHLIGHT_BACKCOLOR, DrawingUtils.GetRgbKey(this.HighlightBackColor));
            session.SetOption<string>(OptionKeyEnum.THEME_FIND_HIGHLIGHT_FONTCOLOR, DrawingUtils.GetRgbKey(this.highlightFontColor));

            session.SetOption<string>(OptionKeyEnum.THEME_BOOKMARK_COLOR, this.ThemeList.SelectedItem.BookmarkColor);

            session.SetOption<string>(OptionKeyEnum.THEME_SELECTION_COLOR, DrawingUtils.GetRgbKey(this.SelectionColor));

            return true;
        }

        #endregion

        #region 实例方法

        private void SwitchTheme(ThemePackage theme)
        {
            // 加载系统已安装的所有字体
            this.FontFamilyList.Clear();
            InstalledFontCollection installedFont = new InstalledFontCollection();
            foreach (FontFamilyDefinition ffd in MTermApp.Context.Manifest.FontFamilyList)
            {
                if (installedFont.Families.FirstOrDefault(v => v.Name == ffd.Value) != null)
                {
                    this.FontFamilyList.Add(ffd);
                }
            }
            // 如果所有的预定义字体都不存在于当前系统里安装的字体，那么把当前系统里的所有字体加进去
            if (this.FontFamilyList.Count == 0)
            {
                this.FontFamilyList.AddRange(installedFont.Families.Select(v => new FontFamilyDefinition() { Name = v.Name, Value = v.Name }));
            }
            this.FontFamilyList.SelectedItem = this.FontFamilyList.FirstOrDefault();
            this.FontSizeList.SelectedItem = this.FontSizeList.FirstOrDefault(v => v.Value == theme.FontSize);

            this.CursorSpeeds.SelectedItem = MTermConsts.DefaultCursorBlinkSpeed;

            this.CursorStyles.SelectedItem = MTermConsts.DefaultCursorStyle;

            this.CursorColor = DrawingUtils.GetColor(theme.CursorColor);
            this.FontColor = DrawingUtils.GetColor(theme.FontColor);
            
            this.BackColor = DrawingUtils.GetColor(theme.BackColor);

            this.HighlightBackColor = DrawingUtils.GetColor(theme.HighlightBackColor);
            this.HighlightFontColor = DrawingUtils.GetColor(theme.HighlightFontColor);

            this.SelectionColor = DrawingUtils.GetColor(theme.SelectionColor);

            this.BackImageUri = theme.BackImageUri;
            this.BackImageOpacity = theme.BackImageOpacity;
        }

        #endregion

        #region 事件处理器

        private void ThemeList_SelectionChanged(ThemePackage oldValue, ThemePackage newValue)
        {
            if (newValue != null)
            {
                this.SwitchTheme(newValue);
            }
        }

        #endregion
    }
}
