using ModengTerm.Base;
using ModengTerm.ViewModel.CreateSession.SessionOptions;
using System;
using System.Windows;
using System.Windows.Controls;
using WPFToolkit.MVVM;

namespace ModengTerm.UserControls.OptionsUserControls
{
    /// <summary>
    /// LoginScriptOptionsUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class LoginScriptOptionsUserControl : UserControl
    {
        public LoginScriptOptionsUserControl()
        {
            InitializeComponent();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            ScriptItemVM scriptItem = new ScriptItemVM()
            {
                ID = Guid.NewGuid().ToString()
            };
            BindableCollection<ScriptItemVM> scriptItems = DataGridScriptItems.DataContext as BindableCollection<ScriptItemVM>;
            scriptItems.Add(scriptItem);
            scriptItems.SelectedItem = scriptItem;
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            ScriptItemVM scriptItem = DataGridScriptItems.SelectedItem as ScriptItemVM;
            if (scriptItem == null)
            {
                MTMessageBox.Info("请选择要编辑的项");
                return;
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            ScriptItemVM scriptItem = DataGridScriptItems.SelectedItem as ScriptItemVM;
            if (scriptItem == null)
            {
                MTMessageBox.Info("请选择要删除的项");
                return;
            }

            if (!MTMessageBox.Confirm("确定删除吗?"))
            {
                return;
            }

            BindableCollection<ScriptItemVM> scriptItems = DataGridScriptItems.DataContext as BindableCollection<ScriptItemVM>;
            scriptItems.Remove(scriptItem);
        }

        private void ButtonMoveUp_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = DataGridScriptItems.SelectedIndex;
            if (selectedIndex == -1)
            {
                return;
            }

            if (selectedIndex == 0)
            {
                return;
            }

            BindableCollection<ScriptItemVM> scriptItems = DataGridScriptItems.DataContext as BindableCollection<ScriptItemVM>;
            scriptItems.Move(selectedIndex, selectedIndex - 1);
        }

        private void ButtonMoveDown_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = DataGridScriptItems.SelectedIndex;
            if (selectedIndex == -1)
            {
                return;
            }

            BindableCollection<ScriptItemVM> scriptItems = DataGridScriptItems.DataContext as BindableCollection<ScriptItemVM>;

            if (selectedIndex == scriptItems.Count - 1)
            {
                return;
            }

            scriptItems.Move(selectedIndex, selectedIndex + 1);
        }

        private void DataGridScriptItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
