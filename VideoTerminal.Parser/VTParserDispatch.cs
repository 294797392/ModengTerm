using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoTerminal.Base;
using VTInterface;

namespace VideoTerminal.Parser
{
    internal class VTParserDispatch
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTParserDispatch");

        #endregion

        #region 公开事件

        /// <summary>
        /// VTParserDispatch：触发事件的对象
        /// DECSETPrivateModeSet：要设置的DECPrivateMode
        /// bool：是否启用该设置
        /// </summary>
        public event Action<VTParserDispatch, DECPrivateMode, bool> DECPrivateModeSet;

        #endregion

        #region 实例变量

        private ICursorState cursorState;
        private IPresentationDevice presentationDevice;     // 保存主屏幕

        protected IVideoTerminal terminal;

        #endregion

        #region 构造方法

        public VTParserDispatch(IVideoTerminal terminal)
        {
            this.terminal = terminal;
        }

        #endregion

        #region 公开接口

        public void ActionPrint(byte ch)
        {
            this.ActionPrint(char.ToString((char)ch));
        }

        public void ActionPrint(string text)
        {
            VTAction.PrintAction.Data = text;
            this.terminal.PerformAction(VTAction.PrintAction);
        }

        public void ActionExecute(byte ch)
        {
            switch (ch)
            {
                case ASCIIChars.NUL:
                    {
                        // do nothing
                        break;
                    }

                case ASCIIChars.BEL:
                    {
                        // 响铃
                        this.terminal.PerformAction(VTAction.PlayBellAction);
                        break;
                    }

                case ASCIIChars.BS:
                    {
                        // Backspace，退格，光标向前移动一位
                        this.terminal.CursorBackward(1);
                        break;
                    }

                case ASCIIChars.TAB:
                    {
                        // tab键
                        this.terminal.ForwardTab();
                        break;
                    }

                case ASCIIChars.CR:
                    {
                        this.terminal.PerformAction(VTAction.CarriageReturnAction);
                        break;
                    }

                case ASCIIChars.LF:
                case ASCIIChars.FF:
                case ASCIIChars.VT:
                    {
                        // 这三个都是LF
                        this.terminal.PerformAction(VTAction.LineFeedAction);
                        break;
                    }

                case ASCIIChars.SI:
                case ASCIIChars.SO:
                    {
                        // 这两个不知道是什么意思
                        logger.WarnFormat("未处理的SI和SI");
                        break;
                    }

                default:
                    {
                        //throw new NotImplementedException(string.Format("未实现的控制字符:{0}", ch));
                        VTAction.PrintAction.Data = char.ToString((char)ch);
                        this.terminal.PerformAction(VTAction.PrintAction);
                        break;
                    }
            }
        }

        public void ActionCSIDispatch(int finalByte, List<int> parameters)
        {
            CSIActionCodes code = (CSIActionCodes)finalByte;

            switch (code)
            {
                case CSIActionCodes.SGR_SetGraphicsRendition:
                    {
                        // Modifies the graphical rendering options applied to the next characters written into the buffer.
                        // Options include colors, invert, underlines, and other "font style" type options.
                        this.PerformSetGraphicsRendition(parameters);
                        break;
                    }

                case CSIActionCodes.DECRST_PrivateModeReset:
                    {
                        this.PerformDECPrivateMode(parameters, false);
                        break;
                    }

                case CSIActionCodes.DECSET_PrivateModeSet:
                    {
                        this.PerformDECPrivateMode(parameters, true);
                        break;
                    }

                case CSIActionCodes.HVP_HorizontalVerticalPosition:
                case CSIActionCodes.CUP_CursorPosition:
                    {
                        int row = parameters[0];
                        int col = parameters[1];
                        this.terminal.CursorPosition(row, col);
                        break;
                    }

                case CSIActionCodes.CUF_CursorForward:
                    {
                        this.terminal.CursorForward(parameters[0]);
                        break;
                    }

                case CSIActionCodes.DTTERM_WindowManipulation:
                    {
                        WindowManipulationType wmt = (WindowManipulationType)parameters[0];
                        this.PerformWindowManipulation(wmt, parameters[1], parameters[2]);
                        break;
                    }

                case CSIActionCodes.DECSTBM_SetScrollingRegion:
                    {
                        int topMargin = parameters[0];
                        int bottomMargin = parameters[1];
                        break;
                    }

                case CSIActionCodes.EL_EraseLine:
                    {
                        this.PerformEraseLine(parameters);
                        break;
                    }

                default:
                    logger.WarnFormat("未实现CSIAction, {0}", finalByte);
                    break;
            }
        }

        public DCSStringHandlerDlg ActionDCSDispatch(int id, List<int> parameters)
        {
            throw new NotImplementedException();
        }

        public void ActionEscDispatch(byte ch)
        {
            EscActionCodes code = (EscActionCodes)ch;

            switch (code)
            {
                case EscActionCodes.DECKPAM_KeypadApplicationMode:
                    {
                        // TODO：实现
                        break;
                    }

                default:
                    logger.WarnFormat("未实现EscAction, {0}", code);
                    break;
            }
        }

        /// <summary>
        /// - Triggers the Vt52EscDispatch action to indicate that the listener should handle
        ///      a VT52 escape sequence. These sequences start with ESC and a single letter,
        ///      sometimes followed by parameters.
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="parameters"></param>
        public void ActionVt52EscDispatch(byte ch, List<int> parameters)
        {
            VT52ActionCodes code = (VT52ActionCodes)ch;

            switch (code)
            {
                default:
                    logger.WarnFormat("未实现VT52ActionCodes:{0}", code);
                    break;
            }
        }

        #endregion

        #region 实例方法

        /// <summary>
        /// 代码从terminal里复制
        /// AdaptDispatch::SetGraphicsRendition
        /// </summary>
        /// <param name="parameters"></param>
        private void PerformSetGraphicsRendition(List<int> parameters)
        {
            int size = parameters.Count;

            List<VTAction> actions = new List<VTAction>();

            for (int i = 0; i < size; i++)
            {
                byte option = (byte)parameters[i];

                switch ((GraphicsOptions)option)
                {
                    case GraphicsOptions.Off:
                        {
                            // 关闭字体效果
                            actions.Add(VTAction.DefaultAttributeAction);
                            actions.Add(VTAction.DefaultBackgroundAction);
                            actions.Add(VTAction.DefaultForegroundAction);
                            break;
                        }

                    case GraphicsOptions.ForegroundDefault: actions.Add(VTAction.DefaultForegroundAction); break;
                    case GraphicsOptions.BackgroundDefault: actions.Add(VTAction.DefaultBackgroundAction); break;
                    case GraphicsOptions.BoldBright: actions.Add(VTAction.BoldAction); break;
                    case GraphicsOptions.RGBColorOrFaint: actions.Add(VTAction.FaintAction); break;// 降低颜色强度
                    case GraphicsOptions.NotBoldOrFaint:
                        {
                            // 还原颜色强度和粗细
                            actions.Add(VTAction.BoldUnsetAction);
                            actions.Add(VTAction.FaintUnsetAction);
                            break;
                        }

                    case GraphicsOptions.Italics: actions.Add(VTAction.ItalicsAction); break;
                    case GraphicsOptions.NotItalics: actions.Add(VTAction.ItalicsUnsetAction); break;
                    case GraphicsOptions.BlinkOrXterm256Index:
                    case GraphicsOptions.RapidBlink: actions.Add(VTAction.BlinkAction); break;
                    case GraphicsOptions.Steady: actions.Add(VTAction.BlinkUnsetAction); break;
                    case GraphicsOptions.Invisible: actions.Add(VTAction.InvisibleAction); break;
                    case GraphicsOptions.Visible: actions.Add(VTAction.InvisibleUnsetAction); break;
                    case GraphicsOptions.CrossedOut: actions.Add(VTAction.CrossedOutAction); break;
                    case GraphicsOptions.NotCrossedOut: actions.Add(VTAction.CrossedOutUnsetAction); break;
                    case GraphicsOptions.Negative: actions.Add(VTAction.ReverseVideoAction); break;
                    case GraphicsOptions.Positive: actions.Add(VTAction.ReverseVideoUnsetAction); break;
                    case GraphicsOptions.Underline: actions.Add(VTAction.UnderlineAction); break;
                    case GraphicsOptions.DoublyUnderlined: actions.Add(VTAction.DoublyUnderlinedAction); break;
                    case GraphicsOptions.NoUnderline:
                        {
                            actions.Add(VTAction.UnderlineUnsetAction);
                            actions.Add(VTAction.DoublyUnderlinedUnsetAction);
                            break;
                        }

                    case GraphicsOptions.Overline: actions.Add(VTAction.OverlinedAction); break;
                    case GraphicsOptions.NoOverline: actions.Add(VTAction.OverlinedUnsetAction); break;

                    case GraphicsOptions.ForegroundBlack: actions.Add(VTAction.ForegroundDarkBlackAction); break;
                    case GraphicsOptions.ForegroundBlue: actions.Add(VTAction.ForegroundDarkBlueAction); break;
                    case GraphicsOptions.ForegroundGreen: actions.Add(VTAction.ForegroundDarkGreenAction); break;
                    case GraphicsOptions.ForegroundCyan: actions.Add(VTAction.ForegroundDarkCyanAction); break;
                    case GraphicsOptions.ForegroundRed: actions.Add(VTAction.ForegroundDarkRedAction); break;
                    case GraphicsOptions.ForegroundMagenta: actions.Add(VTAction.ForegroundDarkMagentaAction); break;
                    case GraphicsOptions.ForegroundYellow: actions.Add(VTAction.ForegroundDarkYellowAction); break;
                    case GraphicsOptions.ForegroundWhite: actions.Add(VTAction.ForegroundDarkWhiteAction); break;

                    case GraphicsOptions.BackgroundBlack: actions.Add(VTAction.BackgroundDarkBlackAction); break;
                    case GraphicsOptions.BackgroundBlue: actions.Add(VTAction.BackgroundDarkBlueAction); break;
                    case GraphicsOptions.BackgroundGreen: actions.Add(VTAction.BackgroundDarkGreenAction); break;
                    case GraphicsOptions.BackgroundCyan: actions.Add(VTAction.BackgroundDarkCyanAction); break;
                    case GraphicsOptions.BackgroundRed: actions.Add(VTAction.BackgroundDarkRedAction); break;
                    case GraphicsOptions.BackgroundMagenta: actions.Add(VTAction.BackgroundDarkMagentaAction); break;
                    case GraphicsOptions.BackgroundYellow: actions.Add(VTAction.BackgroundDarkYellowAction); break;
                    case GraphicsOptions.BackgroundWhite: actions.Add(VTAction.BackgroundDarkWhiteAction); break;

                    case GraphicsOptions.BrightForegroundBlack: actions.Add(VTAction.ForegroundLightBlackAction); break;
                    case GraphicsOptions.BrightForegroundBlue: actions.Add(VTAction.ForegroundLightBlueAction); break;
                    case GraphicsOptions.BrightForegroundGreen: actions.Add(VTAction.ForegroundLightGreenAction); break;
                    case GraphicsOptions.BrightForegroundCyan: actions.Add(VTAction.ForegroundLightCyanAction); break;
                    case GraphicsOptions.BrightForegroundRed: actions.Add(VTAction.ForegroundLightRedAction); break;
                    case GraphicsOptions.BrightForegroundMagenta: actions.Add(VTAction.ForegroundLightMagentaAction); break;
                    case GraphicsOptions.BrightForegroundYellow: actions.Add(VTAction.ForegroundLightYellowAction); break;
                    case GraphicsOptions.BrightForegroundWhite: actions.Add(VTAction.ForegroundLightWhiteAction); break;

                    case GraphicsOptions.BrightBackgroundBlack: actions.Add(VTAction.BackgroundLightBlackAction); break;
                    case GraphicsOptions.BrightBackgroundBlue: actions.Add(VTAction.BackgroundLightBlueAction); break;
                    case GraphicsOptions.BrightBackgroundGreen: actions.Add(VTAction.BackgroundLightGreenAction); break;
                    case GraphicsOptions.BrightBackgroundCyan: actions.Add(VTAction.BackgroundLightCyanAction); break;
                    case GraphicsOptions.BrightBackgroundRed: actions.Add(VTAction.BackgroundLightRedAction); break;
                    case GraphicsOptions.BrightBackgroundMagenta: actions.Add(VTAction.BackgroundLightMagentaAction); break;
                    case GraphicsOptions.BrightBackgroundYellow: actions.Add(VTAction.BackgroundLightYellowAction); break;
                    case GraphicsOptions.BrightBackgroundWhite: actions.Add(VTAction.BackgroundLightWhiteAction); break;

                    case GraphicsOptions.ForegroundExtended:
                        {
                            byte r, g, b;
                            i += this.SetRgbColorsHelper(parameters.Skip(i + 1).ToList(), true, out r, out g, out b);

                            VTAction.ForegroundRGBAction.R = r;
                            VTAction.ForegroundRGBAction.G = g;
                            VTAction.ForegroundRGBAction.B = b;

                            actions.Add(VTAction.ForegroundRGBAction);
                            break;
                        }

                    case GraphicsOptions.BackgroundExtended:
                        {
                            byte r, g, b;
                            i += this.SetRgbColorsHelper(parameters.Skip(i + 1).ToList(), false, out r, out g, out b);

                            VTAction.BackgroundRGBAction.R = r;
                            VTAction.BackgroundRGBAction.G = g;
                            VTAction.BackgroundRGBAction.B = b;

                            actions.Add(VTAction.BackgroundRGBAction);
                            break;
                        }

                    default:
                        //logger.WarnFormat("未实现的SGRCode = {0}", option);
                        break;
                }
            }

            this.terminal.PerformAction(actions);
        }

        /// <summary>
        /// - Helper to parse extended graphics options, which start with 38 (FG) or 48 (BG)
        ///     These options are followed by either a 2 (RGB) or 5 (xterm index)
        ///      RGB sequences then take 3 MORE params to designate the R, G, B parts of the color
        ///      Xterm index will use the param that follows to use a color from the preset 256 color xterm color table.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="foreground"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private int SetRgbColorsHelper(List<int> parameters, bool foreground, out byte r, out byte g, out byte b)
        {
            r = 0; b = 0; g = 0;

            int optionsConsumed = 1;
            GraphicsOptions options = (GraphicsOptions)parameters[0];
            if (options == GraphicsOptions.RGBColorOrFaint)
            {
                // 这里使用RGB颜色
                optionsConsumed = 4;
                r = (byte)parameters[1];
                g = (byte)parameters[2];
                b = (byte)parameters[3];
            }
            else if (options == GraphicsOptions.BlinkOrXterm256Index)
            {
                // 这里使用xterm颜色表里的颜色
                optionsConsumed = 2;
                int tableIndex = parameters[1];
                if (tableIndex <= 255)
                {
                    byte index = (byte)tableIndex;
                    Xterm256Color.ConvertRGB(index, out r, out g, out b);
                }
            }
            return optionsConsumed;
        }

        private bool UseMainScreenBuffer()
        {
            // 先获取子显示屏
            IPresentationDevice subDevice = this.terminal.GetActivePresentationDevice();

            // 切换主屏幕
            if (!this.terminal.SwitchPresentationDevice(this.presentationDevice))
            {
                logger.Error("切换主显示屏失败");
                return false;
            }

            // 还原鼠标状态
            this.terminal.CursorRestoreState(this.cursorState);

            // 删除子显示屏
            this.terminal.DeletePresentationDevice(subDevice);

            return true;
        }

        private bool UseAlternateScreenBuffer()
        {
            this.cursorState = this.terminal.CursorSaveState();

            // 首先保存主屏幕设备
            this.presentationDevice = this.terminal.GetActivePresentationDevice();

            // 再创建一个新的屏幕设备
            IPresentationDevice device = this.terminal.CreatePresentationDevice();
            if (device == null)
            {
                logger.Error("创建显示屏失败");
                return false;
            }

            return this.terminal.SwitchPresentationDevice(device);
        }

        /// <summary>
        /// 设置DECPrivateMode模式
        /// </summary>
        /// <param name="privateModes">要设置的模式列表</param>
        /// <param name="enable">启用或者禁用</param>
        private bool PerformDECPrivateMode(List<int> privateModes, bool enable)
        {
            bool success = false;

            foreach (int mode in privateModes)
            {
                switch ((DECPrivateMode)mode)
                {
                    case Parser.DECPrivateMode.DECANM_AnsiMode:
                        {
                            break;
                        }

                    case Parser.DECPrivateMode.DECCKM_CursorKeysMode:
                        {
                            // set - Enable Application Mode, reset - Normal mode
                            break;
                        }

                    case Parser.DECPrivateMode.ASB_AlternateScreenBuffer:
                        {
                            success = enable ? this.UseAlternateScreenBuffer() : this.UseMainScreenBuffer();
                            break;
                        }

                    case Parser.DECPrivateMode.DECTCEM_TextCursorEnableMode:
                        {
                            success = this.terminal.CursorVisibility(enable);
                            break;
                        }

                    case Parser.DECPrivateMode.XTERM_BracketedPasteMode:
                        {
                            break;
                        }

                    case Parser.DECPrivateMode.ATT610_StartCursorBlink:
                        {
                            break;
                        }

                    default:
                        logger.WarnFormat("未实现DECSETPrivateMode, {0}", mode);
                        break;
                }

                if (this.DECPrivateModeSet != null)
                {
                    this.DECPrivateModeSet(this, (DECPrivateMode)mode, enable);
                }
            }

            return success;
        }

        /// <summary>
        /// Window Manipulation - Performs a variety of actions relating to the window,
        ///      such as moving the window position, resizing the window, querying
        ///      window state, forcing the window to repaint, etc.
        ///  This is kept separate from the input version, as there may be
        ///      codes that are supported in one direction but not the other.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        private void PerformWindowManipulation(WindowManipulationType type, int parameter1, int parameter2)
        {
        }

        /// <summary>
        /// 执行删除操作
        /// </summary>
        /// <param name="parameters"></param>
        private void PerformEraseLine(List<int> parameters)
        {
            //logger.InfoFormat("PerformEraseLine, num paramters = {0}", parameters.Count);
            foreach (int eraseType in parameters)
            {

            }
        }

        #endregion
    }
}
