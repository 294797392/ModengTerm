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

        private ScrollBar scorllbar;

        #endregion

        public override bool Visible
        {
            get
            {
                return base.Visible;
            }
            set
            {
                if (base.Visible != value) 
                {
                    if (value)
                    {
                        this.scorllbar.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this.scorllbar.Visibility = Visibility.Collapsed;
                    }

                    base.Visible = value;
                }
            }
        }

        public VTScrollbarImpl(ScrollBar scrollBar) 
        {
            this.scorllbar = scorllbar;
            this.scorllbar.LargeChange = 1;
            this.scorllbar.SmallChange = 1;
            //this.scorllbar.ValueChanged += Scorllbar_ValueChanged;
        }

        private void Scorllbar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //double newvalue = Math.Round(newValue, 0);
            //if (newvalue > this.Maximum)
            //{
            //    newvalue = this.Maximum;
            //}

            //// feel free to add code to test against the min, too. 
            //this.Value = newvalue;

            //// base.OnValueChanged会触发ValueChanged事件
            //// base.OnValueChanged使用传入的newValue作为ValueChanged事件的参数
            //base.OnValueChanged(oldValue, newvalue);
        }
    }
}
