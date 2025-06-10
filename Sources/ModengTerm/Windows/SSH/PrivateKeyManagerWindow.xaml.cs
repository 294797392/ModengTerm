using log4net.Util;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.ServiceAgents;
using ModengTerm.Controls;
using ModengTerm.Document;
using System;
using System.Collections.Generic;
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
using WPFToolkit.MVVM;

namespace ModengTerm.Windows.SSH
{
    /// <summary>
    /// PrivateKeyManagerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PrivateKeyManagerWindow : MdWindow
    {
        #region 实例变量

        private ServiceAgent serviceAgent;
        private BindableCollection<PrivateKey> privateKeys;

        #endregion

        /// <summary>
        /// 用户选中的密钥
        /// </summary>
        public PrivateKey SelectedPrivateKey { get; private set; }

        #region 构造方法

        public PrivateKeyManagerWindow()
        {
            InitializeComponent();

            this.InitializeWindow();
        }

        #endregion

        #region 实例方法

        private void InitializeWindow()
        {
            this.privateKeys = new BindableCollection<PrivateKey>();
            this.serviceAgent = VTApp.Context.ServiceAgent;

            this.privateKeys.AddRange(this.serviceAgent.GetAllPrivateKey());
            DataGridPrivateKeys.DataContext = this.privateKeys;
        }

        #endregion

        #region 事件处理器

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            base.DialogResult = false;
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            PrivateKey privateKey = DataGridPrivateKeys.SelectedItem as PrivateKey;
            if (privateKey == null) 
            {
                MTMessageBox.Info("请选择要使用的密钥");
                return;
            }

            this.SelectedPrivateKey = privateKey;

            base.DialogResult = true;
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            PrivateKey pk = DataGridPrivateKeys.SelectedItem as PrivateKey;
            if (pk == null)
            {
                MTMessageBox.Info("请选择要删除的密钥");
                return;
            }

            if (!MTMessageBox.Confirm("确定要删除{0}吗?", pk.Name))
            {
                return;
            }

            int code = this.serviceAgent.DeletePrivateKey(pk.ID);
            if (code != ResponseCode.SUCCESS)
            {
                MTMessageBox.Info("删除失败, {0}", code);
                return;
            }

            this.privateKeys.Remove(pk);
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            CreatePrivateKeyWindow createPrivateKeyWindow= new CreatePrivateKeyWindow();
            createPrivateKeyWindow.Owner = this;
            createPrivateKeyWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if ((bool)createPrivateKeyWindow.ShowDialog())
            {
                this.privateKeys.Add(createPrivateKeyWindow.PrivateKey);
            }
        }

        private void DataGridPrivateKeys_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ButtonOK_Click(null, null);
        }

        #endregion
    }
}
