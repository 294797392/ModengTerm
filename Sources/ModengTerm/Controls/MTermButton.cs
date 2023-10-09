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
    public class MTermButton : Button
    {


        public PathFigureCollection SVGPath
        {
            get { return (PathFigureCollection)GetValue(SVGPathProperty); }
            set { SetValue(SVGPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SVGPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SVGPathProperty =
            DependencyProperty.Register("SVGPath", typeof(PathFigureCollection), typeof(MTermButton), new PropertyMetadata(null));

    }
}
