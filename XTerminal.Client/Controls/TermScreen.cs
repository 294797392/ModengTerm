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
        private ListBox listBoxLines;
        private ObservableCollection<TermLine> lines;

        public TermScreen()
        {
        }

        private void InitializeScreen()
        {
            this.lines = new ObservableCollection<TermLine>();

            this.listBoxLines.SelectionChanged += listBoxLines_SelectionChanged;

            TermLine line = new TermLine();
            ListBoxItem item = new ListBoxItem();
            item.Content = line;
            this.listBoxLines.Items.Add(item);
            line.Draw();

            TermLine line2 = new TermLine();
            ListBoxItem item2 = new ListBoxItem();
            item2.Content = line2;
            this.listBoxLines.Items.Add(item2);
            line2.Draw();
        }

        void listBoxLines_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("selected");
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.listBoxLines = base.Template.FindName("PART_VisualLines", this) as ListBox;

            this.InitializeScreen();
        }
    }
}