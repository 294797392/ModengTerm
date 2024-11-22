using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModengTerm.Controls
{
    /// <summary>
    /// 可以把SGV图像当背景的按钮
    /// </summary>
    public class MdButton : Button
    {




        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(MdButton), new PropertyMetadata(null));




        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof(double), typeof(MdButton), new PropertyMetadata(18.0D));



        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof(double), typeof(MdButton), new PropertyMetadata(18.0D));










        public Brush MouseOverBorder
        {
            get { return (Brush)GetValue(MouseOverBorderProperty); }
            set { SetValue(MouseOverBorderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MouseOverBorder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseOverBorderProperty =
            DependencyProperty.Register("MouseOverBorder", typeof(Brush), typeof(MdButton), new PropertyMetadata(Brushes.Transparent));



        public Brush MouseOverBrush
        {
            get { return (Brush)GetValue(MouseOverBrushProperty); }
            set { SetValue(MouseOverBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MouseOverBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseOverBrushProperty =
            DependencyProperty.Register("MouseOverBrush", typeof(Brush), typeof(MdButton), new PropertyMetadata(Brushes.Transparent));

        public Brush MouseDownBrush
        {
            get { return (Brush)GetValue(MouseDownBrushProperty); }
            set { SetValue(MouseDownBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MouseDownBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseDownBrushProperty =
            DependencyProperty.Register("MouseDownBrush", typeof(Brush), typeof(MdButton), new PropertyMetadata(Brushes.Transparent));





        public PathFigureCollection SVGPath
        {
            get { return (PathFigureCollection)GetValue(SVGPathProperty); }
            set { SetValue(SVGPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SVGPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SVGPathProperty =
            DependencyProperty.Register("SVGPath", typeof(PathFigureCollection), typeof(MdButton), new PropertyMetadata(null));



        public Brush SVGColor
        {
            get { return (Brush)GetValue(SVGColorProperty); }
            set { SetValue(SVGColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SVGColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SVGColorProperty =
            DependencyProperty.Register("SVGColor", typeof(Brush), typeof(MdButton), new PropertyMetadata(Brushes.Black));




        public Brush SVGMouseOverColor
        {
            get { return (Brush)GetValue(SVGMouseOverColorProperty); }
            set { SetValue(SVGMouseOverColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SVGMouseOverColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SVGMouseOverColorProperty =
            DependencyProperty.Register("SVGMouseOverColor", typeof(Brush), typeof(MdButton), new PropertyMetadata(Brushes.Transparent));




        public Brush SVGMouseDownColor
        {
            get { return (Brush)GetValue(SVGMouseDownColorProperty); }
            set { SetValue(SVGMouseDownColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SVGMouseDownColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SVGMouseDownColorProperty =
            DependencyProperty.Register("SVGMouseDownColor", typeof(Brush), typeof(MdButton), new PropertyMetadata(Brushes.Transparent));









        /// <summary>
        /// SVG图标距离按钮的边距
        /// </summary>
        public Thickness SVGMargin
        {
            get { return (Thickness)GetValue(SVGMarginProperty); }
            set { SetValue(SVGMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SVGMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SVGMarginProperty =
            DependencyProperty.Register("SVGMargin", typeof(Thickness), typeof(MdButton), new PropertyMetadata(new Thickness(0)));






        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(MdButton), new PropertyMetadata(new CornerRadius(0)));





    }
}
