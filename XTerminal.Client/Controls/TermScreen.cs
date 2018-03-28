using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace XTerminal.Controls
{
    public class TermScreen : Control
    {
        private TermLineList termLines;

        public TermScreen()
        {
        }

        private void InitializeScreen()
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.termLines = base.Template.FindName("PART_VisualLines", this) as TermLineList;

            this.InitializeScreen();
        }

        public void ExecInvocation()
        {
        }
    }
}