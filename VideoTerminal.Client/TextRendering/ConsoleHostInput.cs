using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace VideoTerminal.TextRendering
{
    /// <summary>
    /// 专门用来处理用户的输入
    /// </summary>
    public class ConsoleHostInput : ContentControl
    {
        #region 实例方法

        private ConsoleHost consoleHost;

        #endregion

        #region 构造方法

        public ConsoleHostInput()
        {
            this.consoleHost = new ConsoleHost();
            this.Content = this.consoleHost;
        }

        #endregion

        #region 事件处理器

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);

            Console.WriteLine(e.Text);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            base.Focus();
        }

        /// <summary>
        /// 参考AvalonEdit
        /// 重写了这个事件后，就会触发鼠标相关的事件
        /// </summary>
        /// <param name="hitTestParameters"></param>
        /// <returns></returns>
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }

        #endregion
    }
}
