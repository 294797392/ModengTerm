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
using XTerminal.Base;
using XTerminal.Base.Definitions;

namespace VideoTerminal.Windows
{
    /// <summary>
    /// CreateSessionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CreateSessionWindow : Window
    {
        /// <summary>
        /// 当前新建Session的步骤
        /// </summary>
        private enum StepEnum
        {
            SessionList,
            UpdateProperties
        }

        #region 实例变量

        private BindableCollection<SessionDefinition> sessionList;

        #endregion

        #region 构造方法

        public CreateSessionWindow()
        {
            InitializeComponent();

            this.InitializeWindow();
        }

        #endregion

        #region 实例方法

        private void InitializeWindow()
        {
            this.sessionList = new BindableCollection<SessionDefinition>();
            this.sessionList.AddRange(XTermApp.Context.ServiceAgent.GetSessionDefinitions());
            ListBoxSessionList.DataContext = this.sessionList;
        }

        #endregion
    }
}
