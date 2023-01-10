using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using XTerminalApplication;
using XTerminalBase;
using XTerminalParser;

namespace XTerminal.Controls
{
    /// <summary>
    /// 终端控件
    /// </summary>
    public class Terminal : ContentControl, IVideoTerminal
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("ConsoleHostInput");

        #endregion

        #region 公开事件

        public event Action<IVideoTerminal, VTInputEventArgs> InputEvent;

        #endregion

        #region 实例方法

        private TerminalVisualContainer visualContainer;
        private VTKeyboard keyboard;

        #endregion

        #region 构造方法

        public Terminal()
        {
            this.visualContainer = new TerminalVisualContainer();
            this.Content = this.visualContainer;
            this.keyboard = new VTKeyboard();
        }

        #endregion

        public void TestRender()
        {
            this.visualContainer.TestRender();
        }

        #region 事件处理器

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);

            Console.WriteLine(e.Text);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            //if (e.Key == Key.ImeProcessed)
            //{
            //    // 这些字符交给输入法处理了
            //}
            //else
            //{
            //    e.Handled = true;
            //}
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

        #region IVideoTerminal

        public void PerformAction(VTActions vtAction, params object[] param)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                switch (vtAction)
                {
                    case VTActions.PlayBell:
                        {
                            // 播放响铃
                            break;
                        }

                    default:
                        {
                            this.visualContainer.PerformAction(vtAction, param);
                            break;
                        }
                }
            }));
        }

        public ICursorState CursorSaveState()
        {
            logger.WarnFormat("CursorSaveState");
            return null;
        }

        public void CursorRestoreState(ICursorState state)
        {
            logger.WarnFormat("CursorRestoreState");
        }

        public IPresentationDevice CreatePresentationDevice()
        {
            logger.WarnFormat("CreatePresentationDevice");
            return null;
        }

        public void DeletePresentationDevice(IPresentationDevice device)
        {
            logger.WarnFormat("DeletePresentationDevice");
        }

        public bool SwitchPresentationDevice(IPresentationDevice activeDevice)
        {
            logger.WarnFormat("SwitchPresentationDevice");
            return true;
        }

        public IPresentationDevice GetActivePresentationDevice()
        {
            logger.WarnFormat("GetActivePresentationDevice");
            return null;
        }

        #endregion
    }
}
