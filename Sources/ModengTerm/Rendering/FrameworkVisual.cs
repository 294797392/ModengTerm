using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ModengTerm.Rendering
{
    /// <summary>
    /// 提供显示，隐藏，移动位置的功能
    /// </summary>
    public class FrameworkVisual : DrawingVisual
    {
        private bool visible = true;

        public void SetOpacity(double opacity)
        {
            this.Opacity = opacity;
        }

        public void Arrange(double x, double y)
        {
            this.Offset = new Vector(x, y);
        }

        public void SetVisible(bool visible)
        {
            if (this.visible == visible)
            {
                return;
            }

            if (visible)
            {
                this.Opacity = 1;
            }
            else
            {
                this.Opacity = 0;
            }

            this.visible = visible;
        }
    }
}
