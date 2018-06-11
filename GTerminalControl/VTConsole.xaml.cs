using GTerminalCore;
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
        private IVideoTerminal vt;

        #endregion

        #region 属性

        public IVideoTerminal VT
        {
            get
            {
                return this.vt;
            }
            set
            {
                this.vt = value;
                this.vt.Action += this.VideoTerminal_Action;
            }
        }

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
        }

        #endregion

        #region 事件处理器

        private void RichTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            byte[] data;
            if (this.VT.HandleKeyDown(e, out data))
            {
                if (!this.vt.Stream.Write(data))
                {
                    logger.ErrorFormat("向终端发送数据失败");
                }
            }
        }

        private void VideoTerminal_Action(object sender, VTAction action, ParseState state)
        {
            switch (action)
            {

            }
        }

        #endregion
    }
}