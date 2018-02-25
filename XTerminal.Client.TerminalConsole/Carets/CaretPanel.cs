using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace XTerminal.Client.TerminalConsole.Carets
{
    public class CaretPanel : Canvas
    {
        private CaretElement caret;

        public CaretPanel()
        {
            this.caret = new CaretElement();
            base.Children.Add(this.caret);
        }
    }
}