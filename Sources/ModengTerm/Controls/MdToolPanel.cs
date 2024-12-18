﻿using ModengTerm.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WPFToolkit.MVVM;

namespace ModengTerm.Controls
{
    /// <summary>
    /// 侧边工具栏控件
    /// </summary>
    [TemplatePart(Name = "PART_ItemsHeader", Type = typeof(ListBox))]
    [TemplatePart(Name = "PART_Content", Type = typeof(ContentControl))]
    [TemplatePart(Name = "PART_CloseButton", Type = typeof(MdButton))]
    public class MdToolPanel : Control
    {
        private ContentControl contentControl;
        private ListBox listBox;
        private MdButton buttonClose;



        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(MdToolPanel), new PropertyMetadata(string.Empty));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(MdToolPanel), new PropertyMetadata(false));

        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(MdToolPanel), new PropertyMetadata(false));

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(MdToolPanel), new PropertyMetadata(null));

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(MdToolPanel), new PropertyMetadata(null));







        public ToolPanelVM Menu
        {
            get { return (ToolPanelVM)GetValue(MenuProperty); }
            set { SetValue(MenuProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Menu.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MenuProperty =
            DependencyProperty.Register("Menu", typeof(ToolPanelVM), typeof(MdToolPanel), new PropertyMetadata(null, MenuPropertyChangedCallback));


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.listBox = this.Template.FindName("PART_ItemsHeader", this) as ListBox;
            this.listBox.SelectionChanged += ListBox_SelectionChanged;

            this.buttonClose = this.Template.FindName("PART_CloseButton", this) as MdButton;
            this.buttonClose.Click += ButtonClose_Click;

            this.contentControl = this.Template.FindName("PART_Content", this) as ContentControl;
            // 如果默认不显示MdToolPanel，那么就不会触发ListBox.SelectionChanged事件，界面就无法显示
            // 所以这里重新触发一下SelectionChanged事件
            //if (this.Menu != null)
            //{
            //    this.Menu.InvokeWhenSelectionChanged();
            //}
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            base.SetValue(VisibilityProperty, System.Windows.Visibility.Collapsed);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.Menu.InvokeWhenSelectionChanged();
        }


        private void OnMenuPropertyChanged(ToolPanelVM oldValue, ToolPanelVM newValue)
        {
            if (this.contentControl == null)
            {
                return;
            }

            //this.contentControl.DataContext = newValue;
        }

        private static void MenuPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MdToolPanel me = d as MdToolPanel;
            me.OnMenuPropertyChanged(e.OldValue as ToolPanelVM, e.NewValue as ToolPanelVM);
        }
    }
}
