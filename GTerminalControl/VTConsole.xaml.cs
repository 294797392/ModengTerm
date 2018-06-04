using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GTerminalControl
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class VTConsole : UserControl
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTConsole");

        #endregion

        #region 实例变量

        private Paragraph _paragraph;
        private Run _promptInline;

        #endregion

        #region 属性

        public IRemoteHost Host { get; set; }

        public IVideoTerminal VT { get; set; }

        #endregion

        #region 构造方法

        public VTConsole()
        {
            InitializeComponent();

            this.InitializeConsole();
        }

        #endregion

        #region 实例方法

        private void InitializeConsole()
        {
            _paragraph = new Paragraph
            {
            };
            RichTextBox.IsUndoEnabled = false;
            RichTextBox.Document = new FlowDocument(_paragraph);
            RichTextBox.PreviewKeyDown += RichTextBox_PreviewKeyDown;

            this.Host.DataReceived += Host_DataReceived;

            this.VT.Action += VT_Action;
        }

        #endregion

        #region 事件处理器

        private void RichTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            byte[] data;
            if (this.VT.OnKeyDown(e, out data))
            {
                if (!this.Host.SendData(data))
                {
                    logger.ErrorFormat("发送数据失败");
                }
            }
        }

        private void Host_DataReceived(object sender, byte[] stream)
        {
            this.VT.ParseTerminalStream(stream);
        }

        private void VT_Action(object sender, byte controlFunc, byte[] data)
        {
        }

        #endregion
    }
}