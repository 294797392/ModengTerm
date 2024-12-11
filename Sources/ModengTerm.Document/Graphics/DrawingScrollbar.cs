using ModengTerm.Terminal.Document;
using ModengTerm.Terminal.Rendering;
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
    public class DrawingScrollbar : ScrollBar, IDrawingScrollbar
    {
        #region 实例变量

        private Brush trackBrush;
        private Brush thumbBrush;
        private Brush buttonBrush;

        #endregion

        #region IDrawingScrollbar

        public int ViewportRow
        {
            get { return (int)ViewportSize; }
            set
            {
                if (ViewportSize != value)
                {
                    ViewportSize = value;
                }
            }
        }

        public new double Maximum
        {
            get { return base.Maximum; }
            set
            {
                if (base.Maximum != value)
                {
                    base.Maximum = value;
                }
            }
        }

        #endregion

        public DrawingScrollbar()
        {
        }

        public void Initialize()
        {
            LargeChange = 1;
            SmallChange = 1;
        }

        public void Release()
        {
        }

        public void SetOpacity(double opacity)
        {
            Opacity = opacity;
        }

        public void Arrange(double x, double y)
        {
            throw new NotImplementedException();
        }

        public void Draw()
        {

        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            double newvalue = Math.Round(newValue, 0);
            if (newvalue > Maximum)
            {
                newvalue = Maximum;
            }

            // feel free to add code to test against the min, too. 
            Value = newvalue;

            // base.OnValueChanged会触发ValueChanged事件
            // base.OnValueChanged使用传入的newValue作为ValueChanged事件的参数
            base.OnValueChanged(oldValue, newvalue);
        }
    }
}
