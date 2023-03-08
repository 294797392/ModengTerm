using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VideoTerminal.Parser;
using VideoTerminal.Sockets;

namespace VideoTerminal
{
    public class TextArea : FlowDocumentScrollViewer
    {
        public TextArea()
        {
            this.Background = Brushes.Transparent;
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);

            Console.WriteLine(e.Text);

            e.Handled = true;
        }

        /// <inheritdoc/>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            base.Focus();
        }

        //protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        //{
        //    // accept clicks even where the text area draws no background
        //    return new PointHitTestResult(this, hitTestParameters.HitPoint);
        //}
    }
}

