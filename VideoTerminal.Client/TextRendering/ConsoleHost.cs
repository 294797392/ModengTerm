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

namespace VideoTerminal.TextRendering
{
    public class ConsoleHost : FrameworkElement
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("ConsoleHost");

        #endregion

        #region 实例方法

        private VisualCollection visualList;
        private DrawingVisualText textDrawingVisual;
        private DrawingVisualCaret caretDrawingVisual;

        private Thread blinkThread;

        #endregion

        #region 属性

        // Provide a required override for the VisualChildrenCount property.
        protected override int VisualChildrenCount
        {
            get { return this.visualList.Count; }
        }

        #endregion

        #region 构造方法

        public ConsoleHost()
        {
            this.InitializeControl();
        }

        #endregion

        #region 实例方法

        private void InitializeControl()
        {
            this.textDrawingVisual = new DrawingVisualText();
            this.caretDrawingVisual = new DrawingVisualCaret();

            this.visualList = new VisualCollection(this);
            this.visualList.Add(this.textDrawingVisual);
            this.visualList.Add(this.caretDrawingVisual);

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                this.textDrawingVisual.Text = "asdkhaskjdh卡事件回调卡手动开机阿萨德很快就哈萨克接口和和框架ksjadhkjashdkjashdkjsahdkjashdkjsdad";
                this.textDrawingVisual.Render();

                this.InitializeBlinkThread();
            }
        }

        private void InitializeBlinkThread()
        {
            this.blinkThread = new Thread(this.BlinkThreadProc);
            this.blinkThread.IsBackground = true;
            this.blinkThread.Start();
        }

        #endregion

        #region 事件处理器

        private void BlinkThreadProc(object state)
        {
            while (true)
            {
                try
                {
                    this.Dispatcher.Invoke(this.caretDrawingVisual.Render);
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
            if (index < 0 || index >= this.visualList.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return this.visualList[index];
        }

        #endregion
    }
}
