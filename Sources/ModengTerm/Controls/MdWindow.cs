using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ModengTerm.Controls
{
    [TemplatePart(Name = "PART_MinButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_MaxButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_CloseButton", Type = typeof(Button))]
    public class MdWindow : Window
    {



        /// <summary>
        /// CanResize替换ResizeMode
        /// </summary>
        public bool CanResize
        {
            get { return (bool)GetValue(CanResizeProperty); }
            set { SetValue(CanResizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanResize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanResizeProperty =
            DependencyProperty.Register("CanResize", typeof(bool), typeof(MdWindow), new PropertyMetadata(false));







        public MdWindow()
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Grid grid = this.Template.FindName("PART_Title", this) as Grid;
            grid.MouseLeftButtonDown += Grid_MouseLeftButtonDown;

            Button closeButton = this.Template.FindName("PART_CloseButton", this) as Button;
            closeButton.Click += CloseButton_Click;

            Button minButton = this.Template.FindName("PART_MinButton", this) as Button;
            minButton.Click += MinButton_Click;

            Button maxButton = this.Template.FindName("PART_MaxButton", this) as Button;
            maxButton.Click += MaxButton_Click;
        }

        private void MaxButton_Click(object sender, RoutedEventArgs e)
        {
            if (base.WindowState == WindowState.Normal)
            {
                base.WindowState = WindowState.Maximized;
            }
            else
            {
                base.WindowState = WindowState.Normal;
            }
        }

        private void MinButton_Click(object sender, RoutedEventArgs e)
        {
            base.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            base.Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
