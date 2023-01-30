using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using XTerminalController;
using XTerminalParser;

namespace XTerminal.Controls
{
    public class TerminalTextSurface : FrameworkElement
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("TerminalVisuals");

        #endregion

        #region 实例方法

        private VisualCollection textList;

        #endregion

        #region 属性

        // Provide a required override for the VisualChildrenCount property.
        protected override int VisualChildrenCount
        {
            get { return this.textList.Count; }
        }

        #endregion

        #region 构造方法

        public TerminalTextSurface()
        {
            this.InitializeCanvas();
        }

        #endregion

        #region 实例方法

        private void InitializeCanvas()
        {
            this.textList = new VisualCollection(this);
        }

        /// <summary>
        /// 创建一个新的行，然后渲染，最后返回
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private TerminalText RenderLine(string text)
        {
            //TerminalText line = TerminalLineFactory.Render(0, this.linesHeight, text);
            //this.termLines.Add(line);
            //this.visualList.Add(line);

            //this.linesHeight += line.Height;

            //return line;
            return new TerminalText();
        }

        private TerminalText CreateText()
        {
            //TerminalTextSource textSource = new TerminalTextSource();
            //TerminalTextRunProperties textProperties = new TerminalTextRunProperties();

            return new TerminalText();
        }

        #endregion

        #region 公开接口

        public void DrawText(VTextBlock textBlock)
        {
            FormattedText text = new FormattedText(textBlock.Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, this.typeface, this.fontSize, this.fontColor, this.pixelPerDip);
            this.textList.Add(text);
        }

        #endregion

        #region 事件处理器

        private void BlinkThreadProc(object state)
        {
            //while (true)
            //{
            //    try
            //    {
            //        this.Dispatcher.Invoke(this.termCaret.Render);
            //    }
            //    catch (Exception ex)
            //    {
            //        // 有可能是线程正在运行的时候然后关闭了客户端
            //        logger.Error("BlinkThreadProc异常", ex);
            //    }

            //    Thread.Sleep(500);
            //}
        }

        #endregion

        #region 重写方法

        // Provide a required override for the GetVisualChild method.
        protected override Visual GetVisualChild(int index)
        {
            //Console.WriteLine(string.Format("GetVisualChild, {0}", Guid.NewGuid().ToString()));
            if (index < 0 || index >= this.textList.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return this.textList[index];
        }

        #endregion
    }
}
