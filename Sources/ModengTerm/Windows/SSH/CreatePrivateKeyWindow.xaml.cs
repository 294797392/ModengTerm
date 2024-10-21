using Microsoft.Win32;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.ServiceAgents;
using ModengTerm.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ModengTerm.Windows.SSH
{
    /// <summary>
    /// CreatePrivateKeyWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CreatePrivateKeyWindow : MdWindow
    {
        private string privateKeyPath;
        private ServiceAgent serviceAgent;

        /// <summary>
        /// 用户新建成功的密钥
        /// </summary>
        public PrivateKey PrivateKey { get; private set; }

        public CreatePrivateKeyWindow()
        {
            InitializeComponent();

            this.InitializeWindow();
        }

        private void InitializeWindow() 
        {
            this.serviceAgent = MTermApp.Context.ServiceAgent;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            base.DialogResult = false;
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            string name = TextBoxName.Text;
            if (string.IsNullOrEmpty(name))
            {
                MTMessageBox.Info("请输入密钥名称");
                return;
            }

            if (string.IsNullOrEmpty(this.privateKeyPath))
            {
                MTMessageBox.Info("请选择密钥文件");
                return;
            }

            if (!File.Exists(this.privateKeyPath)) 
            {
                MTMessageBox.Info("密钥文件不存在");
                return;
            }

            PrivateKey privateKey = new PrivateKey()
            {
                ID = Guid.NewGuid().ToString(),
                Name = name,
                Content = File.ReadAllText(this.privateKeyPath)
            };

            int code = this.serviceAgent.AddPrivateKey(privateKey);
            if (code != ResponseCode.SUCCESS) 
            {
                MTMessageBox.Info("新建密钥失败, {0}", code);
                return;
            }

            this.PrivateKey = privateKey;

            base.DialogResult = true;
        }

        private void ButtonBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if ((bool)openFileDialog.ShowDialog())
            {
                string fileName = System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                TextBoxPrivateKey.Text = fileName;
                TextBoxName.Text = fileName;

                this.privateKeyPath = openFileDialog.FileName;
            }
        }
    }
}
