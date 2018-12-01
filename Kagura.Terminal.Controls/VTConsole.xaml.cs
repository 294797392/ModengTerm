using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Kagura.Terminal.Controls
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class VTConsole : UserControl, VTScreen
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTConsole");

        #endregion

        #region 实例变量

        private VisualParagraph _paragraph;
        private VideoTerminal vt;
        private int cursorRow = 0;
        private int cursorColumn = 0;

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
            }
        }

        public TextPointer CurrentCaretPosition
        {
            get
            {
                return RichTextBox.CaretPosition;
            }
        }

        public int CursorRow { get { return this.cursorRow; } set { this.cursorRow = value; } }

        public int CursorColumn { get { return this.cursorColumn; } set { this.cursorColumn = value; } }

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
            this._paragraph = new VisualParagraph();
            this._paragraph.CreateVisualLine();
            RichTextBox.IsUndoEnabled = false;
            RichTextBox.IsReadOnlyCaretVisible = false;
            RichTextBox.IsReadOnly = true;
            RichTextBox.Document.Blocks.Clear();
            RichTextBox.Document.Blocks.Add(this._paragraph);
            RichTextBox.Document.IsEnabled = false;
            RichTextBox.PreviewKeyDown += RichTextBox_PreviewKeyDown;
            RichTextBox.PreviewTextInput += RichTextBox_PreviewTextInput;
            Task.Factory.StartNew(this.CursorThreadProcess);

            //System.Windows.Documents.List lis;
            //ListItem item;

            Run run;
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

        private void InsertTextAtPosition(string text, int column, int row)
        {
            this._paragraph.InsertTextAtPosition(text, column, row);
            //TextPointer textPosition = this._paragraph.ContentStart;
            //textPosition = textPosition.GetLineStartPosition(row);
            //textPosition = textPosition.GetPositionAtOffset(column, LogicalDirection.Forward);
            //textPosition.InsertTextInRun(text);
        }

        private void CursorThreadProcess()
        {
            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        private void EraseText(int row, int startCol, int endCol)
        {

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

        #endregion

        #region VTScreen

        public void PrintText(string text)
        {
            if (text == "\r")
            {
                this.CursorColumn = 0;
                return;
            }
            if (text == "\n")
            {
                this.CursorRow += 1;
                this._paragraph.CreateVisualLine();
            }
            else
            {
                this.InsertTextAtPosition(text, this.CursorColumn, this.CursorRow);
                this.CursorColumn += text.Length;
            }
        }

        public void MoveCursor(int col, int row)
        {
            this.CursorColumn += col;
            this.CursorRow += row;
        }

        public void EraseLine(int startCol, int count)
        {
            if (count == 0)
            {
                throw new NotImplementedException();
                //TextPointer lineStart = position.GetLineStartPosition(0);
            }
            else
            {
                //position.DeleteTextInRun(count);
            }
        }

        #endregion
    }
}