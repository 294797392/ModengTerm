using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ModengTerm.Document.Rendering
{
    public class VTScrollbarImpl : VTScrollbar
    {
        #region 实例变量

        private ScrollBar scrollbar;

        #endregion

        public override double Maximum
        {
            get { return this.scrollbar.Maximum; }
            set
            {
                if (this.scrollbar.Maximum != value)
                {
                    this.scrollbar.Maximum = value;
                }
            }
        }

        public override double Value
        {
            get { return this.scrollbar.Value; }
            set
            {
                if (this.scrollbar.Value != value)
                {
                    this.scrollbar.Value = value;
                }
            }
        }

        public override int ViewportRow
        {
            get { return (int)this.scrollbar.ViewportSize; }
            set
            {
                if (this.scrollbar.ViewportSize != value)
                {
                    this.scrollbar.ViewportSize = value;
                }
            }
        }

        public override bool Visible
        {
            get
            {
                return this.scrollbar.Visibility == Visibility.Visible;
            }
            set
            {
                bool visible = this.scrollbar.Visibility == Visibility.Visible;

                if (visible != value)
                {
                    if (value)
                    {
                        this.scrollbar.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this.scrollbar.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        public VTScrollbarImpl(ScrollBar scrollbar)
        {
            this.scrollbar = scrollbar;
            this.scrollbar.LargeChange = 1;
            this.scrollbar.SmallChange = 1;
            this.scrollbar.ValueChanged += Scorllbar_ValueChanged;
        }

        private void Scorllbar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var newvalue = Math.Round(e.NewValue, 0);
            if (newvalue > this.scrollbar.Maximum)
            {
                newvalue = this.scrollbar.Maximum;
            }

            this.Document.EventInput.OnScrollChanged((int)newvalue);
        }
    }
}
