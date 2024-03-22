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
        private bool mouseDown;
        private int oldValue;
        private int newValue;
        private ScrollChangedData scrollData;

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
            this.scrollData = new ScrollChangedData();

            this.scrollbar = scrollbar;
            this.scrollbar.LargeChange = 1;
            this.scrollbar.SmallChange = 1;
            this.scrollbar.PreviewMouseLeftButtonDown += Scrollbar_PreviewMouseLeftButtonDown;
            this.scrollbar.PreviewMouseLeftButtonUp += Scrollbar_PreviewMouseLeftButtonUp;
            this.scrollbar.MouseMove += Scrollbar_MouseMove;
        }

        private int GetScrollValue() 
        {
            var newvalue = (int)Math.Round(this.scrollbar.Value, 0);
            if (newvalue > this.scrollbar.Maximum)
            {
                newvalue = (int)Math.Round(this.scrollbar.Maximum, 0);
            }
            return newvalue;
        }

        private void Scrollbar_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!this.mouseDown)
            {
                return;
            }

            this.scrollData.NewScroll = this.GetScrollValue();
            this.Document.EventInput.OnScrollChanged(this.scrollData);
        }

        private void Scrollbar_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.mouseDown = false;
        }

        private void Scrollbar_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.scrollData.OldScroll = this.GetScrollValue();

            this.mouseDown = true;
        }
    }
}
