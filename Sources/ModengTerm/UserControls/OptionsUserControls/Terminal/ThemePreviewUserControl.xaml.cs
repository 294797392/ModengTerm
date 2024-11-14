using ModengTerm.Document;
using ModengTerm.Terminal.DataModels;
using ModengTerm.Terminal.Parsing;
using ModengTerm.UserControls.TerminalUserControls.Rendering;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPFToolkit.MVVM;

namespace ModengTerm.UserControls.OptionsUserControl.Terminal
{
    /// <summary>
    /// ThemePreviewUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class ThemePreviewUserControl : UserControl
    {
        private static readonly List<VTColorIndex> BrightColors = new List<VTColorIndex>() 
        {
            VTColorIndex.BrightBlack, VTColorIndex.BrightBlue, VTColorIndex.BrightCyan, VTColorIndex.BrightGreen, 
            VTColorIndex.BrightMagenta, VTColorIndex.BrightRed, VTColorIndex.BrightWhite, VTColorIndex.BrightYellow
        };

        private static readonly List<VTColorIndex> DarkColors = new List<VTColorIndex>() 
        {
            VTColorIndex.DarkBlack, VTColorIndex.DarkBlue, VTColorIndex.DarkCyan, VTColorIndex.DarkGreen,
            VTColorIndex.DarkMagenta, VTColorIndex.DarkRed, VTColorIndex.DarkWhite, VTColorIndex.DarkYellow
        };

        private class ColorItem
        {
            public string Name { get; set; }

            public Brush Brush { get; set; }

            public string Character { get; set; }
        }

        public ThemePackage Theme
        {
            get { return (ThemePackage)GetValue(ThemeProperty); }
            set { SetValue(ThemeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Theme.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThemeProperty =
            DependencyProperty.Register("Theme", typeof(ThemePackage), typeof(ThemePreviewUserControl), new PropertyMetadata(null, ThemePropertyChangedCallback));




        public Color BackgroundColor
        {
            get { return (Color)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackgroundColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register("BackgroundColor", typeof(Color), typeof(ThemePreviewUserControl), new PropertyMetadata(Colors.Transparent, BackgroundColorPropertyChangedCallback));





        public int PreviewFontSize
        {
            get { return (int)GetValue(PreviewFontSizeProperty); }
            set { SetValue(PreviewFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PreviewFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PreviewFontSizeProperty =
            DependencyProperty.Register("PreviewFontSize", typeof(int), typeof(ThemePreviewUserControl), new PropertyMetadata(12, PreviewFontSizePropertyChangedCallback));




        public string PreviewFontFamily
        {
            get { return (string)GetValue(PreviewFontFamilyProperty); }
            set { SetValue(PreviewFontFamilyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PreviewFontFamily.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PreviewFontFamilyProperty =
            DependencyProperty.Register("PreviewFontFamily", typeof(string), typeof(ThemePreviewUserControl), new PropertyMetadata(string.Empty, PreviewFontFamilyPropertyChangedCallback));




        public Color PreviewFontColor
        {
            get { return (Color)GetValue(PreviewFontColorProperty); }
            set { SetValue(PreviewFontColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PreviewFontColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PreviewFontColorProperty =
            DependencyProperty.Register("PreviewFontColor", typeof(Color), typeof(ThemePreviewUserControl), new PropertyMetadata(Colors.Transparent, PreviewFontColorPropertyChangedCallback));














        public ThemePreviewUserControl()
        {
            InitializeComponent();

            this.InitializeUserControl();
        }

        private void InitializeUserControl()
        {

        }



        private BindableCollection<ColorItem> InitializeColorList(ThemePackage theme, List<VTColorIndex> colorIndexs)
        {
            BindableCollection<ColorItem> colorList = new BindableCollection<ColorItem>();

            char firstChar = 'A';

            foreach (VTColorIndex colorIndex in colorIndexs)
            {
                VTColor vtColor = theme.ColorTable.GetColor(colorIndex);
                Brush brush = DrawingUtils.GetBrush(vtColor);

                ColorItem colorItem = new ColorItem() 
                {
                    Name = colorIndex.ToString(),
                    Brush = brush,
                    Character = firstChar.ToString()
                };
                colorList.Add(colorItem);

                firstChar++;
            }

            return colorList;
        }

        private void OnThemePropertyChanged(ThemePackage oldValue, ThemePackage newValue)
        {
            if (newValue != null) 
            {
                ListBoxBrightColorsFont.DataContext = ListBoxBrightColors.DataContext = this.InitializeColorList(newValue, BrightColors);
                ListBoxDarkColorsFont.DataContext = ListBoxDarkColors.DataContext = this.InitializeColorList(newValue, DarkColors);
            }
        }

        private static void ThemePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ThemePreviewUserControl me = d as ThemePreviewUserControl;
            me.OnThemePropertyChanged(e.OldValue as ThemePackage, e.NewValue as ThemePackage);
        }




        private void OnBackgroundColorPropertyChanged(object oldValue, object newValue)
        {
            if (newValue != null)
            {
                BorderBackground.Background = new SolidColorBrush((Color)newValue);
            }
        }

        private static void BackgroundColorPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ThemePreviewUserControl me = d as ThemePreviewUserControl;
            me.OnBackgroundColorPropertyChanged(e.OldValue, e.NewValue);
        }


        private void OnPreviewFontColorPropertyChanged(object oldValue, object newValue)
        {
            if (newValue != null)
            {
                TextBlockFontPreview.Foreground = new SolidColorBrush((Color)newValue);
            }
        }

        private static void PreviewFontColorPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ThemePreviewUserControl me = d as ThemePreviewUserControl;
            me.OnPreviewFontColorPropertyChanged(e.OldValue, e.NewValue);
        }



        private void OnPreviewFontSizePropertyChanged(object oldValue, object newValue)
        {
            if (newValue != null)
            {
                TextBlockFontPreview.FontSize = (int)newValue;
            }
        }

        private static void PreviewFontSizePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ThemePreviewUserControl me = d as ThemePreviewUserControl;
            me.OnPreviewFontSizePropertyChanged(e.OldValue, e.NewValue);
        }


        private void OnPreviewFontFamilyPropertyChanged(object oldValue, object newValue)
        {
            if (newValue != null)
            {
                TextBlockFontPreview.FontFamily = new FontFamily(newValue.ToString());
            }
        }

        private static void PreviewFontFamilyPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ThemePreviewUserControl me = d as ThemePreviewUserControl;
            me.OnPreviewFontFamilyPropertyChanged(e.OldValue, e.NewValue);
        }

    }
}
