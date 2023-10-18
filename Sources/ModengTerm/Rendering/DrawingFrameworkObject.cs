using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModengTerm.Rendering
{
    /// <summary>
    /// 该类比普通的DrawingObject功能更多
    /// 1. 当宽高被设置的时候，会自动更新改元素在父元素里的空间区域
    /// </summary>
    public abstract class DrawingFrameworkObject : DrawingObject
    {
        private double width;
        private double height;

        public double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set
            {
                if (this.width != value)
                {
                    SetValue(WidthProperty, value);
                    this.width = value;
                }
            }
        }

        // Using a DependencyProperty as the backing store for Width.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(DrawingScrollbar), new FrameworkPropertyMetadata(0.0D, FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        public double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set 
            {
                if (this.height != value)
                {
                    SetValue(HeightProperty, value);
                    this.height = value;
                }
            }
        }

        // Using a DependencyProperty as the backing store for Height.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(DrawingScrollbar), new FrameworkPropertyMetadata(0.0D, FrameworkPropertyMetadataOptions.AffectsParentMeasure));
    }
}
