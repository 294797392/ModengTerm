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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModengTerm.UserControls
{
    /// <summary>
    /// GenericDocumentUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class GenericDocumentUserControl : UserControl
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("GenericDocumentUserControl");

        #endregion

        #region 实例变量

        private VTDocument document;

        #endregion

        #region 构造方法

        public GenericDocumentUserControl()
        {
            InitializeComponent();
        }

        #endregion

        #region 公开接口

        public void Initialize()
        {
            VTDocumentOptions documentOptions = new VTDocumentOptions()
            {
            };

            this.document = new VTDocument(documentOptions);
            this.document.Initialize();
        }

        public void AppendText(string text) 
        {
        }

        #endregion
    }
}
