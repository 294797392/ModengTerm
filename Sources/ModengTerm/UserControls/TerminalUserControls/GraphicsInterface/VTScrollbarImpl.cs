using ModengTerm.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ModengTerm.UserControls.TerminalUserControls
{
    public class VTScrollbarImpl : VTScrollbar
    {
        #region 实例变量

        private ScrollBar scrollbar;
        private int oldValue;
        private int newValue;
        private ScrollChangedData scrollData;

        #endregion

        #region VTScrollbar

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

        #endregion

        #region 构造方法

        public VTScrollbarImpl(ScrollBar scrollbar)
        {
            this.scrollData = new ScrollChangedData();

            this.scrollbar = scrollbar;
            this.scrollbar.LargeChange = 1;
            this.scrollbar.SmallChange = 1;
        }

        #endregion

        #region 实例方法

        #endregion
    }
}
