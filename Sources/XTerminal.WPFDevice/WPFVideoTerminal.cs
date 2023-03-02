using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.TextFormatting;
using XTerminalBase;
using XTerminalParser;
using XTerminalBase.IVideoTerminal;

namespace XTerminal.WPFRenderer
{
    /// <summary>
    /// 终端控件
    /// </summary>
    public class WPFVideoTerminal : Grid, IInputDevice, IVTController
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("WPFVideoTerminal");

        #endregion

        #region 公开事件

        public event Action<IInputDevice, VTInputEvent> InputEvent;

        #endregion

        #region 实例变量

        private ScrollViewer scrollViewer;
        private VTInputEvent inputEvent;

        #endregion

        #region 属性

        #endregion

        #region 构造方法

        public WPFVideoTerminal()
        {
            this.inputEvent = new VTInputEvent();
            this.Background = Brushes.Transparent;
            this.Focusable = true;
        }

        #endregion

        #region 实例方法

        private void NotifyInputEvent(VTInputEvent evt)
        {
            if (this.InputEvent != null)
            {
                this.InputEvent(this, evt);
            }
        }

        #endregion

        #region 事件处理器

        /// <summary>
        /// 输入中文的时候会触发该事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);

            //Console.WriteLine(e.Text);
        }

        /// <summary>
        /// 从键盘上按下按键的时候会触发
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (e.Key == Key.ImeProcessed)
            {
                // 这些字符交给输入法处理了
            }
            else
            {
                switch (e.Key)
                {
                    case Key.Tab:
                    case Key.Up:
                    case Key.Down:
                    case Key.Left:
                    case Key.Right:
                    case Key.Space:
                        {
                            // 防止焦点移动到其他控件上了
                            e.Handled = true;
                            break;
                        }
                }

                VTKeys vtKey = TerminalUtils.ConvertToVTKey(e.Key);
                this.inputEvent.CapsLock = Console.CapsLock;
                this.inputEvent.Key = vtKey;
                this.inputEvent.Text = null;
                this.inputEvent.Modifiers = VTModifierKeys.None;
                this.NotifyInputEvent(this.inputEvent);
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            this.Focus();
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

        #region IVTController

        /// <summary>
        /// 获取输入设备
        /// </summary>
        /// <returns></returns>
        public IInputDevice GetInputDevice()
        {
            return this;
        }

        /// <summary>
        /// 创建一个新的显示设备
        /// </summary>
        /// <returns></returns>
        public IPresentationDevice CreatePresentationDevice()
        {
            WPFPresentaionDevice device = new WPFPresentaionDevice();
            return device;
        }

        /// <summary>
        /// 释放显示设备占用的资源
        /// </summary>
        /// <param name="device"></param>
        public void ReleasePresentationDevice(IPresentationDevice device)
        {

        }

        /// <summary>
        /// 切换显示设备
        /// </summary>
        /// <param name="device"></param>
        public void SwitchPresentaionDevice(IPresentationDevice toRemove, IPresentationDevice toAdd)
        {
            if (this.scrollViewer == null)
            {
                this.scrollViewer = new ScrollViewer()
                {
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto
                };
                this.Children.Add(this.scrollViewer);
            }

            this.scrollViewer.Content = toAdd;
        }

        #endregion
    }
}
