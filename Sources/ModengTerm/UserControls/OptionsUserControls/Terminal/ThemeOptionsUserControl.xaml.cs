using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace ModengTerm.UserControls.OptionsUserControls.Terminal
{
    /// <summary>
    /// ThemeOptionsUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class ThemeOptionsUserControl : UserControl
    {
        public ThemeOptionsUserControl()
        {
            InitializeComponent();

            this.InitializeUserControl();
        }

        private void InitializeUserControl()
        {
        }

        private void ButtonBrowseBackgroundImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "图片文件|*.jpg;*.png;*.jpeg;*.bmp|All files(*.*)|*.*";
            if ((bool)openFileDialog.ShowDialog()) 
            {
                string fullPath = openFileDialog.FileName;
                string fileName = System.IO.Path.GetFileName(fullPath);

                TextBoxBackgroundImage.Text = fileName;
                TextBoxBackgroundImage.Tag = fullPath;
            }
        }
    }
}
