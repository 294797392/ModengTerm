using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Globalization;
using System.Windows.Media.TextFormatting;
using System.Collections.ObjectModel;

namespace XTerminal.Client.TerminalConsole.Rendering
{
    /// <summary>
    /// 存储一行里的信息
    /// </summary>
    public class TerminalLine
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("TerminalLine");

        #endregion

        #region 实例变量

        private TextLine textLine;
        private TerminalLineTextSource textSource;
        private DrawingVisual textVisual;
        private List<TerminalLineElement> elements;
        private TerminalLineElement currentElement;

        #endregion

        #region 属性

        public DrawingVisual TextVisual
        {
            get
            {
                return this.textVisual;
            }
        }

        #endregion

        #region 构造方法

        public TerminalLine(TextLine textLine, TerminalLineTextSource textSource)
        {
            this.textSource = textSource;
            this.textLine = textLine;
            this.elements = this.textSource.lineElements;
            this.textVisual = new DrawingVisual();
        }

        #endregion

        #region 实例方法

        private void Render()
        {
            var drawingCtx = this.textVisual.RenderOpen();
            this.textLine.Draw(drawingCtx, new Point(0, 0), InvertAxes.None);
            drawingCtx.Close();
        }

        #endregion

        #region 公开接口

        public void InputChar(char c)
        {
            if (char.IsWhiteSpace(c) || this.currentElement == null)
            {
                TerminalLineTextElement textElement = new TerminalLineTextElement();
                textElement.ColumnIndex = 0;
                this.elements.Add(textElement);
                this.currentElement = textElement;
            }

            if (!char.IsWhiteSpace(c))
            {
                this.currentElement.InputChar(c);
            }

            this.Render();
        }

        #endregion
    }
}