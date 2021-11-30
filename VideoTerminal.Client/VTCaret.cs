using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace VideoTerminal
{
    public class VTCaret : UIElement
    {
        #region 实例方法

        private Thread blinkThread;
        private bool blink;

        #endregion

        #region 构造方法

        public VTCaret()
        {
            this.blinkThread = new Thread(this.BlinkThreadProc);
            this.blinkThread.IsBackground = true;
            this.blinkThread.Start();
        }

        #endregion

        #region 实例方法

        private void BlinkThreadProc(object state)
        {
            while (true)
            {
                this.Dispatcher.Invoke(this.InvalidateVisual);

                Thread.Sleep(500);
            }
        }

        #endregion

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (blink)
            {
                drawingContext.DrawLine(new Pen(Brushes.White, 2), new Point(10, 10), new Point(10, 20));
            }
            blink = !blink;
        }
    }
}
