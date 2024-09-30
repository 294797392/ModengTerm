using ModengTerm.Document;
using ModengTerm.Document.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ModengTerm.Document
{
    /// <summary>
    /// 渲染光标的定时器
    /// </summary>
    public class VTCursorTimer : DotNEToolkit.SingletonObject<VTCursorTimer>
    {
        private DispatcherTimer dispatcherTimer;
        private VTCursor cursor;

        /// <summary>
        /// 设置要闪烁的光标
        /// </summary>
        /// <param name="cursor"></param>
        public void SetCursor(VTCursor cursor)
        {
            if (this.cursor == cursor)
            {
                return;
            }

            if (this.dispatcherTimer == null)
            {
                this.dispatcherTimer = new DispatcherTimer();
                this.dispatcherTimer.Interval = TimeSpan.FromMilliseconds(cursor.Interval);
                this.dispatcherTimer.Tick += DispatcherTimer_Tick;
                this.dispatcherTimer.Start();
            }
            else
            {
                this.dispatcherTimer.Interval = TimeSpan.FromMilliseconds(cursor.Interval);
            }

            this.cursor = cursor;
        }

        private void DispatcherTimer_Tick(object? sender, EventArgs e)
        {
            VTCursor cursor = this.cursor;

            cursor.Flash();
        }
    }
}
