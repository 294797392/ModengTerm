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
using XTerminalController;
using XTerminalParser;

namespace XTerminal.VideoTerminal
{
    /// <summary>
    /// 终端控件
    /// </summary>
    public class Terminal : ContentControl, IVideoTerminal
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("Terminal");

        #endregion

        #region 公开事件

        public event Action<IVideoTerminal, VTInputEventArgs> InputEvent;

        #endregion

        #region 实例变量

        private TextCanvas textCanvas;
        private VTKeyboard keyboard;
        private Typeface typeface;
        private double fontSize;
        private Brush fontColor;
        private double pixelPerDip;

        #endregion

        #region 构造方法

        public Terminal()
        {
            this.keyboard = new VTKeyboard();
            this.typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
            this.fontSize = 12;
            this.fontColor = Brushes.Black;
            this.pixelPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;

            this.textCanvas = new TextCanvas();
            this.textCanvas.Typeface = this.typeface;
            this.textCanvas.FontSize = this.fontSize;
            this.textCanvas.Foreground = this.fontColor;
            this.textCanvas.PixelsPerDip = this.pixelPerDip;
            this.Content = this.textCanvas;
        }

        #endregion

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

        public void DrawText(List<VTextBlock> textBlocks)
        {
            foreach (VTextBlock textBlock in textBlocks)
            {
                this.DrawText(textBlock);
            }
        }

        public void DrawText(VTextBlock textBlock)
        {
            this.textCanvas.DrawText(textBlock);
        }

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
                            //this.textCanvas.PerformAction(vtAction, param);
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
