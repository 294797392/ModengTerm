using Microsoft.Win32;
using ModengTerm.Addon.Controls;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.DataModels.Terminal;
using ModengTerm.Document;
using ModengTerm.Document.Utility;
using ModengTerm.Terminal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Formats.Asn1;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModengTerm.UserControls.OptionsUserControls.Terminal
{
    /// <summary>
    /// ThemeOptionsUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class ThemeOptionsUserControl : UserControl, IPreferencePanel
    {
        private List<ThemePackage> themes;

        public ThemeOptionsUserControl()
        {
            InitializeComponent();

            this.InitializeUserControl();
        }

        private void InitializeUserControl()
        {
            this.themes = new List<ThemePackage>();
            this.themes.AddRange(VTBaseConsts.TerminalThemes);

            ComboBoxThemeList.ItemsSource = this.themes;
            ComboBoxThemeList.SelectedIndex = 0;
            ComboBoxFontFamily.ItemsSource = VTBaseUtils.GetAllFontFamilies();
            ComboBoxFontFamily.SelectedIndex = 0;
            ComboBoxFontSize.ItemsSource = VTBaseConsts.FontSizes;
            ComboBoxFontSize.SelectedIndex = 0;
        }

        private void ButtonBrowseBackgroundImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "图片文件|*.jpg;*.png;*.jpeg;*.bmp|All files(*.*)|*.*";
            if ((bool)openFileDialog.ShowDialog())
            {
                string fullPath = openFileDialog.FileName;
                string fileName = System.IO.Path.GetFileName(fullPath);
                byte[] fileBytes = File.ReadAllBytes(fullPath);
                string imageData = Convert.ToBase64String(fileBytes);

                TextBoxBackgroundImage.Text = fileName;
                TextBoxBackgroundImage.Tag = imageData;
            }
        }

        public Dictionary<string, object> GetOptions()
        {
            ThemePackage selectedTheme = ComboBoxThemeList.SelectedItem as ThemePackage;
            if (selectedTheme == null)
            {
                MTMessageBox.Info("请选择主题");
                return null;
            }

            uint padding;
            if (!uint.TryParse(TextBoxPadding.Text, out padding))
            {
                MTMessageBox.Info("请输入正确的内边距");
                return null;
            }

            Dictionary<string, object> options = new Dictionary<string, object>()
            {
                { PredefinedOptions.THEME_FONT_FAMILY, ((System.Drawing.FontFamily)ComboBoxFontFamily.SelectedItem).Name },
                { PredefinedOptions.THEME_FONT_SIZE, (int)ComboBoxFontSize.SelectedItem },
                { PredefinedOptions.THEME_FONT_COLOR, VTBaseUtils.Color2RgbKey(ColorPickerForeground.SelectedColor.Value) },
                { PredefinedOptions.THEME_BACK_COLOR, VTBaseUtils.Color2RgbKey(ColorPickerBackground.SelectedColor.Value) },
                { PredefinedOptions.THEME_CURSOR_COLOR, VTBaseUtils.Color2RgbKey(ColorPickerCursorColor.SelectedColor.Value) },
                { PredefinedOptions.THEME_BACK_IMAGE_NAME, TextBoxBackgroundImage.Text },
                { PredefinedOptions.THEME_BACK_IMAGE_DATA, TextBoxBackgroundImage.Tag },
                { PredefinedOptions.THEME_BACK_IMAGE_OPACITY, SliderBackgroundOpacity.Value },
                { PredefinedOptions.TEHEM_COLOR_TABLE, JsonConvert.SerializeObject(selectedTheme.ColorTable) },
                { PredefinedOptions.THEME_ID, selectedTheme.ID },
                { PredefinedOptions.THEME_PADDING, padding },
                { PredefinedOptions.THEME_SELECTION_BACK_COLOR, VTBaseUtils.Color2RgbKey(ColorPickerSelectionBackColor.SelectedColor.Value) }
            };

            return options;
        }

        public void SetOptions(Dictionary<string, object> options)
        {
            ComboBoxThemeList.SelectionChanged -= this.ComboBoxThemeList_SelectionChanged;

            ComboBoxThemeList.SelectedItem = this.themes.FirstOrDefault(v => v.ID == options.GetOptions<string>(PredefinedOptions.THEME_ID));
            ComboBoxFontFamily.SelectedItem = VTBaseUtils.GetAllFontFamilies().FirstOrDefault(v => v.Name == options.GetOptions<string>(PredefinedOptions.THEME_FONT_FAMILY));
            ComboBoxFontSize.SelectedItem = options.GetOptions<int>(PredefinedOptions.THEME_FONT_SIZE);
            ColorPickerForeground.SelectedColor = VTBaseUtils.RgbKey2Color(options.GetOptions<string>(PredefinedOptions.THEME_FONT_COLOR));
            ColorPickerBackground.SelectedColor = VTBaseUtils.RgbKey2Color(options.GetOptions<string>(PredefinedOptions.THEME_BACK_COLOR));
            ColorPickerCursorColor.SelectedColor = VTBaseUtils.RgbKey2Color(options.GetOptions<string>(PredefinedOptions.THEME_CURSOR_COLOR));
            TextBoxBackgroundImage.Text = options.GetOptions<string>(PredefinedOptions.THEME_BACK_IMAGE_NAME);
            TextBoxBackgroundImage.Tag = options.GetOptions<string>(PredefinedOptions.THEME_BACK_IMAGE_DATA);
            SliderBackgroundOpacity.Value = options.GetOptions<double>(PredefinedOptions.THEME_BACK_IMAGE_OPACITY);
            TextBoxPadding.Text = options.GetOptions<string>(PredefinedOptions.THEME_PADDING);
            ColorPickerSelectionBackColor.SelectedColor = VTBaseUtils.RgbKey2Color(options.GetOptions<string>(PredefinedOptions.THEME_SELECTION_BACK_COLOR));

            ComboBoxThemeList.SelectionChanged += this.ComboBoxThemeList_SelectionChanged;
        }

        private void ComboBoxThemeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ThemePackage package = ComboBoxThemeList.SelectedItem as ThemePackage;
            if (package == null)
            {
                return;
            }

            ComboBoxFontSize.SelectedItem = package.FontSize;
            ColorPickerForeground.SelectedColor = VTBaseUtils.RgbKey2Color(package.FontColor);
            ColorPickerBackground.SelectedColor = VTBaseUtils.RgbKey2Color(package.BackColor);
            ColorPickerCursorColor.SelectedColor = VTBaseUtils.RgbKey2Color(package.CursorColor);
        }
    }
}
