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
    [TemplatePart(Name = "PART_CloseButton", Type = typeof(Button))]
    public class MTermWindow : Window
    {
        public MTermWindow()
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Button closeButton = this.Template.FindName("PART_CloseButton", this) as Button;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            this.DragMove();
        }
    }
}
