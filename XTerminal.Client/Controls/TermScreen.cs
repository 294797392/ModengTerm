using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace XTerminal.Controls
{
    public class TermScreen : Control
    {
        private ListBox listBoxLines;
        private ObservableCollection<TermLine> lines;

        public TermScreen()
        {
        }

        private void InitializeScreen()
        {
            this.lines = new ObservableCollection<TermLine>();
            this.listBoxLines.Items.Add(new Button() { Width = 100, Height = 100 });
            this.listBoxLines.Items.Add(new TextBlock() { Width = 100, Height = 100, Text = "asd" });
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.listBoxLines = base.Template.FindName("PART_VisualLines", this) as ListBox;

            this.InitializeScreen();
        }
    }
}