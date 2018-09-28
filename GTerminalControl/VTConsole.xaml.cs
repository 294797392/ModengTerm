using GardeniaTerminalCore;
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
        private VideoTerminal vt;

        #endregion

        #region 属性

        public VideoTerminal VT
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
            _paragraph = new Paragraph()
            {
            };
            RichTextBox.IsUndoEnabled = false;
            RichTextBox.Document = new FlowDocument(_paragraph);
            RichTextBox.PreviewKeyDown += RichTextBox_PreviewKeyDown;
            RichTextBox.PreviewTextInput += RichTextBox_PreviewTextInput;
        }

        ///// <summary>
        ///// TextPointer
        ///// </summary>
        ///// <param name="column"></param>
        ///// <returns></returns>
        //private TextPointer GetTextPointer(int row, int column)
        //{
        //    RichTextBox.CaretPosition.GetLineStartPosition
        //    RichTextBox.CaretPosition.GetPositionAtOffset(column);
        //}

        private void AppendText(string text)
        {
            Inline lastInline = this._paragraph.Inlines.LastInline;
            if (lastInline == null)
            {
                Run runInline = new Run();
                this._paragraph.Inlines.Add(runInline);
            }

            RichTextBox.CaretPosition.InsertTextInRun(text);
            RichTextBox.CaretPosition = RichTextBox.CaretPosition.GetPositionAtOffset(text.Length, LogicalDirection.Forward);
        }

        #endregion

        #region 事件处理器

        private void RichTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            byte[] data;
            if (this.VT.HandleInputWideChar(e.Text, out data))
            {
                if (!this.vt.Socket.Write(data))
                {
                    logger.ErrorFormat("向终端发送数据失败");
                }
            }
        }

        private void RichTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.ImeProcessed)
            {
            }
            else
            {
                byte[] data;
                if (this.VT.HandleInputChar(e, out data))
                {
                    if (!this.vt.Socket.Write(data))
                    {
                        logger.ErrorFormat("向终端发送数据失败");
                    }
                }

                e.Handled = true;
            }
        }

        private void VideoTerminal_Action(object sender, VTAction action, ParseState state)
        {
            switch (action)
            {
                case VTAction.Print:
                    {
                        base.Dispatcher.Invoke(new Action(() =>
                        {
                            this.AppendText(state.Text);
                        }));
                    }
                    break;

                case VTAction.MoveCursor:
                    {
                    }
                    break;

                case VTAction.NewLine:
                    {
                        base.Dispatcher.Invoke(new Action(() =>
                        {

                        }));
                    }
                    break;
            }
        }

        #endregion
    }
}