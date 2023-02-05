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
using VideoTerminal.Utility;

namespace XTerminal.Render
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

        public event Action<IVideoTerminal, VTKeys, VTModifierKeys, string> InputEvent;

        #endregion

        #region 实例变量

        private TextCanvas textCanvas;
        private Typeface typeface;
        private double pixelPerDip;

        #endregion

        #region 属性

        #endregion

        #region 构造方法

        public Terminal()
        {
            this.typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
            this.pixelPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;

            this.textCanvas = new TextCanvas();
            this.textCanvas.Typeface = this.typeface;
            this.textCanvas.PixelsPerDip = this.pixelPerDip;
            this.Content = this.textCanvas;
        }

        #endregion

        #region 实例方法

        private void NotifyInputEvent(VTKeys key, VTModifierKeys mkey, string text)
        {
            if (this.InputEvent != null)
            {
                this.InputEvent(this, key, mkey, text);
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
                VTKeys vtKey = TerminalUtils.ConvertToVTKey(e.Key);
                this.NotifyInputEvent(vtKey, VTModifierKeys.None, string.Empty);
            }
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
            Dispatcher.Invoke(() =>
            {
                this.textCanvas.DrawText(textBlock);
            });
        }

        public VTextBlockMetrics MeasureText(VTextBlock textBlock)
        {
            FormattedText formattedText = TerminalUtils.CreateFormattedText(textBlock, this.typeface, this.pixelPerDip);
            TerminalUtils.UpdateTextMetrics(textBlock, formattedText);
            return textBlock.Metrics;
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
