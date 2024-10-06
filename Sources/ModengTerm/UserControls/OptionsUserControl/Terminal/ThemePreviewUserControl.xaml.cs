using ModengTerm.Controls;
using ModengTerm.Document;
using ModengTerm.Document.Rendering;
using ModengTerm.Terminal.DataModels;
using ModengTerm.Terminal.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        }

        public ThemePackage Theme
        {
            get { return (ThemePackage)GetValue(ThemeProperty); }
            set { SetValue(ThemeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Theme.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThemeProperty =
            DependencyProperty.Register("Theme", typeof(ThemePackage), typeof(ThemePreviewUserControl), new PropertyMetadata(null, ThemePropertyChangedCallback));





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

            foreach (VTColorIndex colorIndex in colorIndexs)
            {
                VTColor vtColor = theme.ColorTable.GetColor(colorIndex);
                Brush brush = DrawingUtils.GetBrush(vtColor);

                ColorItem colorItem = new ColorItem() 
                {
                    Name = colorIndex.ToString(),
                    Brush = brush,
                };
                colorList.Add(colorItem);
            }

            return colorList;
        }

        private void OnThemePropertyChanged(ThemePackage oldValue, ThemePackage newValue)
        {
            if (newValue != null) 
            {
                ListBoxBrightColors.DataContext = this.InitializeColorList(newValue, BrightColors);
                ListBoxDarkColors.DataContext = this.InitializeColorList(newValue, DarkColors);
            }
        }

        private static void ThemePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ThemePreviewUserControl me = d as ThemePreviewUserControl;
            me.OnThemePropertyChanged(e.OldValue as ThemePackage, e.NewValue as ThemePackage);
        }
    }
}
