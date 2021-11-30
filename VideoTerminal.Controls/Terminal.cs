using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VideoTerminal.Parser;
using VTInterface;

namespace VideoTerminal.Controls
{
    /// <summary>
    /// 终端控件
    /// </summary>
    public class Terminal : ContentControl, IVideoTerminal
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("ConsoleHostInput");

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

        public void PerformAction(List<VTAction> vtActions)
        {
            //this.Dispatcher.Invoke(new Action(() =>
            //{
            //    this.lastInline = this.CreateRun();

            //    foreach (VTAction vtAction in vtActions)
            //    {
            //        switch (vtAction.Type)
            //        {
            //            case VTActions.Foreground:
            //                {
            //                    this.lastInline.Foreground = this.TextColor2Brush((TextColor)vtAction.Data);
            //                    break;
            //                }

            //            case VTActions.Background:
            //                {
            //                    this.lastInline.Background = this.TextColor2Brush((TextColor)vtAction.Data);
            //                    break;
            //                }

            //            case VTActions.ForegroundRGB:
            //                {
            //                    this.lastInline.Foreground = this.CreateBrush(vtAction.R, vtAction.G, vtAction.B);
            //                    break;
            //                }

            //            case VTActions.BackgroundRGB:
            //                {
            //                    this.lastInline.Background = this.CreateBrush(vtAction.R, vtAction.G, vtAction.B);
            //                    break;
            //                }

            //            case VTActions.Blink:
            //                {
            //                    break;
            //                }

            //            case VTActions.BlinkUnset:
            //                {
            //                    break;
            //                }

            //            case VTActions.Bold:
            //                {
            //                    this.lastInline.FontWeight = FontWeights.Bold;
            //                    break;
            //                }

            //            case VTActions.BoldUnset:
            //                {
            //                    this.lastInline.FontWeight = FontWeights.Normal;
            //                    break;
            //                }

            //            case VTActions.CrossedOut:
            //                {
            //                    break;
            //                }

            //            case VTActions.CrossedOutUnset:
            //                {
            //                    break;
            //                }

            //            case VTActions.DefaultAttributes:
            //                {
            //                    break;
            //                }

            //            case VTActions.DefaultBackground:
            //                {
            //                    break;
            //                }

            //            case VTActions.DefaultForeground:
            //                {
            //                    break;
            //                }

            //            case VTActions.DoublyUnderlined:
            //                {
            //                    break;
            //                }

            //            case VTActions.DoublyUnderlinedUnset:
            //                {
            //                    break;
            //                }

            //            case VTActions.Faint:
            //                {
            //                    break;
            //                }

            //            case VTActions.FaintUnset:
            //                {
            //                    break;
            //                }

            //            case VTActions.Invisible:
            //                {
            //                    break;
            //                }

            //            case VTActions.InvisibleUnset:
            //                {
            //                    break;
            //                }

            //            case VTActions.Italics:
            //                {
            //                    this.lastInline.FontStyle = FontStyles.Italic;
            //                    break;
            //                }

            //            case VTActions.ItalicsUnset:
            //                {
            //                    this.lastInline.FontStyle = FontStyles.Normal;
            //                    break;
            //                }

            //            case VTActions.Overlined:
            //                {
            //                    this.lastInline.TextDecorations = TextDecorations.OverLine;
            //                    break;
            //                }

            //            case VTActions.OverlinedUnset:
            //                {
            //                    this.lastInline.TextDecorations = null;
            //                    break;
            //                }

            //            case VTActions.ReverseVideo:
            //                {
            //                    break;
            //                }

            //            case VTActions.ReverseVideoUnset:
            //                {
            //                    break;
            //                }

            //            case VTActions.Underline:
            //                {
            //                    this.lastInline.TextDecorations = TextDecorations.Underline;
            //                    break;
            //                }

            //            case VTActions.UnderlineUnset:
            //                {
            //                    this.lastInline.TextDecorations = null;
            //                    break;
            //                }

            //            default:
            //                logger.WarnFormat("未实现VTAction, {0}", vtAction.Type);
            //                break;
            //        }
            //    }
            //}));
        }

        public void PerformAction(VTAction vtAction)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                switch (vtAction.Type)
                {
                    case VTActions.PlayBell:
                        {
                            // 播放响铃
                            break;
                        }

                    default:
                        {
                            this.visualContainer.PerformAction(vtAction);
                            break;
                        }
                }
            }));
        }

        public void CursorBackward(int distance)
        {
            logger.WarnFormat("CursorBackward");
        }

        public void CursorForward(int distance)
        {
            logger.WarnFormat("CursorForward");
        }

        public void ForwardTab()
        {
            logger.WarnFormat("ForwardTab");
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

        public bool CursorVisibility(bool visible)
        {
            logger.WarnFormat("CursorVisibility");
            return false;
        }

        public void CursorPosition(int row, int column)
        {
            logger.WarnFormat("CursorPosition");
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
