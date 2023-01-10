using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using XTerminalParser;

namespace XTerminal.Controls
{
    public class TerminalVisualContainer : FrameworkElement
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("TerminalVisuals");

        #endregion

        #region 实例方法

        private VisualCollection visualList;

        /// <summary>
        /// 所有的行
        /// </summary>
        private List<TerminalLine> termLines;

        /// <summary>
        /// 闪烁的光标
        /// </summary>
        private TerminalCaret termCaret;

        /// <summary>
        /// 当前正在编辑的行
        /// </summary>
        private TerminalLine currentLine;

        /// <summary>
        /// 闪烁光标的线程
        /// </summary>
        private Thread blinkThread;

        /// <summary>
        /// 当前的所有行高
        /// </summary>
        private double linesHeight;

        private bool selectionState;

        #endregion

        #region 属性

        // Provide a required override for the VisualChildrenCount property.
        protected override int VisualChildrenCount
        {
            get { return this.visualList.Count; }
        }

        #endregion

        #region 构造方法

        public TerminalVisualContainer()
        {
            this.InitializeContainer();
        }

        #endregion

        #region 实例方法

        private void InitializeContainer()
        {
            this.termLines = new List<TerminalLine>();
            this.termCaret = new TerminalCaret();

            this.visualList = new VisualCollection(this);
            this.visualList.Add(this.termCaret);

            this.currentLine = this.RenderLine(string.Empty);

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                //this.InitializeBlinkThread();
            }
        }

        private void InitializeBlinkThread()
        {
            this.blinkThread = new Thread(this.BlinkThreadProc);
            this.blinkThread.IsBackground = true;
            this.blinkThread.Start();
        }

        /// <summary>
        /// 创建一个新的行，然后渲染，最后返回
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private TerminalLine RenderLine(string text)
        {
            TerminalLine line = TerminalLineFactory.Render(0, this.linesHeight, text);
            this.termLines.Add(line);
            this.visualList.Add(line);

            this.linesHeight += line.Height;

            return line;
        }

        #endregion

        #region 公开接口

        public void TestRender()
        {
            //this.RenderLine("123");
            //this.RenderLine("456");
            //this.RenderLine("789");
        }

        public void PerformAction(VTActions vtAction, params object[] param)
        {
            switch (vtAction)
            {
                case VTActions.Print:
                    {
                        string text = param[0].ToString();

                        this.currentLine.AppendText(text);
                        this.currentLine.PerformRender();

                        break;
                    }

                case VTActions.CarriageReturn:
                    {
                        break;
                    }

                case VTActions.LineFeed:
                    {
                        this.currentLine = this.RenderLine(string.Empty);
                        break;
                    }

                default:
                    logger.WarnFormat("未处理的VTAction, {0}", vtAction);
                    break;
            }
        }

        #endregion

        #region 事件处理器

        private void BlinkThreadProc(object state)
        {
            while (true)
            {
                try
                {
                    this.Dispatcher.Invoke(this.termCaret.Render);
                }
                catch (Exception ex)
                {
                    // 有可能是线程正在运行的时候然后关闭了客户端
                    logger.Error("BlinkThreadProc异常", ex);
                }

                Thread.Sleep(500);
            }
        }

        #endregion

        #region 重写方法

        // Provide a required override for the GetVisualChild method.
        protected override Visual GetVisualChild(int index)
        {
            //Console.WriteLine(string.Format("GetVisualChild, {0}", Guid.NewGuid().ToString()));
            if (index < 0 || index >= this.visualList.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return this.visualList[index];
        }

        #endregion
    }
}
