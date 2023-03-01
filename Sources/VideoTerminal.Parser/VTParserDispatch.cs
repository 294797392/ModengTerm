using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminalParser
{
    /// <summary>
    /// 负责分发VTParser的事件
    /// </summary>
    public partial class VTParser
    {
        #region 类变量

        #endregion

        #region 公开事件

        #endregion

        #region 实例变量

        //private ICursorState cursorState;
        //private IPresentationDevice presentationDevice;     // 保存主屏幕

        #endregion

        #region 构造方法

        public VTParser()
        {
        }

        #endregion

        #region 公开接口

        private void ActionPrint(byte ch)
        {
            this.NotifyActionEvent(VTActions.Print, char.ToString((char)ch));
        }

        private void ActionPrint(string text)
        {
            this.NotifyActionEvent(VTActions.Print, text);
        }

        /// <summary>
        /// 执行C0字符集的控制字符
        /// </summary>
        /// <param name="ch"></param>
        private void ActionExecute(byte ch)
        {
            //logger.InfoFormat("执行CSI事件, {0}, {1}", ch, this.parameters.Count);

            switch (ch)
            {
                case ASCIITable.NUL:
                    {
                        // do nothing
                        break;
                    }

                case ASCIITable.BEL:
                    {
                        // 响铃
                        logger.DebugFormat("Action - BEL");
                        this.NotifyActionEvent(VTActions.PlayBell);
                        break;
                    }

                case ASCIITable.BS:
                    {
                        // Backspace，退格，光标向前移动一位

                        // BS causes the active data position to be moved one character position in the data component in the 
                        // direction opposite to that of the implicit movement.
                        // The direction of the implicit movement depends on the parameter value of SELECT IMPLICIT
                        // MOVEMENT DIRECTION (SIMD).

                        // 在Active Position（光标的位置）的位置向implicit movement相反的方向移动一个字符
                        // implicit movement的方向使用SIMD标志来指定
                        logger.DebugFormat("Action - Backspace");
                        this.NotifyActionEvent(VTActions.CursorBackward, 1);
                        break;
                    }

                case ASCIITable.TAB:
                    {
                        // tab键
                        logger.DebugFormat("Action - Tab");
                        this.NotifyActionEvent(VTActions.ForwardTab);
                        break;
                    }

                case ASCIITable.CR:
                    {
                        logger.DebugFormat("Action - CR");
                        this.NotifyActionEvent(VTActions.CarriageReturn);
                        break;
                    }

                case ASCIITable.LF:
                    {
                        logger.DebugFormat("Action - LF");
                        this.NotifyActionEvent(VTActions.LineFeed);
                        break;
                    }

                case ASCIITable.FF:
                    {
                        logger.DebugFormat("Action - LF");
                        this.NotifyActionEvent(VTActions.LineFeed);
                        break;
                    }

                case ASCIITable.VT:
                    {
                        // 这三个都是LF
                        logger.DebugFormat("Action - VT");
                        this.NotifyActionEvent(VTActions.LineFeed);
                        break;
                    }

                case ASCIITable.SI:
                case ASCIITable.SO:
                    {
                        // 这两个不知道是什么意思
                        logger.WarnFormat("未处理的SI和SI");
                        break;
                    }

                default:
                    {
                        throw new NotImplementedException(string.Format("未实现的控制字符:{0}", ch));
                        //this.NotifyActionEvent(VTActions.Print, char.ToString((char)ch));
                        break;
                    }
            }
        }

        /// <summary>
        /// CSI状态解析完毕，开始执行CSI对应的动作
        /// </summary>
        /// <param name="finalByte">Final Byte</param>
        private void ActionCSIDispatch(int finalByte, List<int> parameters)
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
                        logger.DebugFormat("CSIDispatch - DECRST_PrivateModeReset");
                        this.PerformDECPrivateMode(parameters, false);
                        break;
                    }

                case CSIActionCodes.DECSET_PrivateModeSet:
                    {
                        logger.DebugFormat("CSIDispatch - DECSET_PrivateModeSet");
                        this.PerformDECPrivateMode(parameters, true);
                        break;
                    }

                case CSIActionCodes.HVP_HorizontalVerticalPosition:
                    {
                        logger.DebugFormat("CSIDispatch - HVP_HorizontalVerticalPosition");
                        break;
                    }

                case CSIActionCodes.CUP_CursorPosition:
                    {
                        logger.DebugFormat("CSIDispatch - CUP_CursorPosition");
                        int row = parameters[0];
                        int col = parameters[1];
                        this.NotifyActionEvent(VTActions.CursorPosition, row, col);
                        break;
                    }

                case CSIActionCodes.CUF_CursorForward:
                    {
                        logger.DebugFormat("CSIDispatch - CUF_CursorForward");
                        this.NotifyActionEvent(VTActions.CursorForword, parameters[0]);
                        break;
                    }

                case CSIActionCodes.DTTERM_WindowManipulation:
                    {
                        logger.DebugFormat("CSIDispatch - DTTERM_WindowManipulation");
                        WindowManipulationType wmt = (WindowManipulationType)parameters[0];
                        this.PerformWindowManipulation(wmt, parameters[1], parameters[2]);
                        break;
                    }

                case CSIActionCodes.DECSTBM_SetScrollingRegion:
                    {
                        logger.DebugFormat("CSIDispatch - DECSTBM_SetScrollingRegion");
                        int topMargin = parameters[0];
                        int bottomMargin = parameters[1];
                        throw new NotImplementedException();
                        break;
                    }

                case CSIActionCodes.EL_EraseLine:
                    {
                        logger.DebugFormat("CSIDispatch - EL_EraseLine");
                        this.PerformEraseLine(parameters);
                        break;
                    }

                case CSIActionCodes.DCH_DeleteCharacter:
                    {
                        logger.DebugFormat("CSIDispatch - DCH_DeleteCharacter, {0}", parameters[0]);
                        this.NotifyActionEvent(VTActions.DeleteCharacters, parameters[0]);
                        break;
                    }

                default:
                    logger.WarnFormat("未实现CSIAction, {0}", (char)finalByte);
                    break;
            }
        }

        private DCSStringHandlerDlg ActionDCSDispatch(int id, List<int> parameters)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///  - Triggers the EscDispatch action to indicate that the listener should handle a simple escape sequence.
        ///   These sequences traditionally start with ESC and a simple letter. No complicated parameters.
        /// </summary>
        /// <param name="ch">Final Byte</param>
        private void ActionEscDispatch(byte ch)
        {
            EscActionCodes code = (EscActionCodes)ch;

            switch (code)
            {
                case EscActionCodes.DECKPAM_KeypadApplicationMode:
                    {
                        logger.DebugFormat("ESCDispatch - DECKPAM_KeypadApplicationMode");
                        this.NotifyActionEvent(VTActions.SetKeypadMode, VTKeypadMode.ApplicationMode);
                        break;
                    }

                case EscActionCodes.DECKPNM_KeypadNumericMode:
                    {
                        logger.DebugFormat("ESCDispatch - DECKPNM_KeypadNumericMode");
                        this.NotifyActionEvent(VTActions.SetKeypadMode, VTKeypadMode.NumericMode);
                        break;
                    }

                default:
                    logger.ErrorFormat("未实现EscAction, {0}", code);
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
        private void ActionVt52EscDispatch(byte ch, List<int> parameters)
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

            for (int i = 0; i < size; i++)
            {
                byte option = (byte)parameters[i];

                switch ((GraphicsOptions)option)
                {
                    case GraphicsOptions.Off:
                        {
                            // 关闭字体效果
                            this.NotifyActionEvent(VTActions.DefaultAttributes);
                            this.NotifyActionEvent(VTActions.DefaultBackground);
                            this.NotifyActionEvent(VTActions.DefaultForeground);
                            break;
                        }

                    case GraphicsOptions.ForegroundDefault: this.NotifyActionEvent(VTActions.DefaultForeground); break;
                    case GraphicsOptions.BackgroundDefault: this.NotifyActionEvent(VTActions.DefaultBackground); break;
                    case GraphicsOptions.BoldBright: this.NotifyActionEvent(VTActions.Bold); break;
                    case GraphicsOptions.RGBColorOrFaint: this.NotifyActionEvent(VTActions.Faint); break;// 降低颜色强度
                    case GraphicsOptions.NotBoldOrFaint:
                        {
                            // 还原颜色强度和粗细
                            this.NotifyActionEvent(VTActions.BoldUnset);
                            this.NotifyActionEvent(VTActions.FaintUnset);
                            break;
                        }

                    case GraphicsOptions.Italics: this.NotifyActionEvent(VTActions.Italics); break;
                    case GraphicsOptions.NotItalics: this.NotifyActionEvent(VTActions.ItalicsUnset); break;
                    case GraphicsOptions.BlinkOrXterm256Index:
                    case GraphicsOptions.RapidBlink: this.NotifyActionEvent(VTActions.Blink); break;
                    case GraphicsOptions.Steady: this.NotifyActionEvent(VTActions.BlinkUnset); break;
                    case GraphicsOptions.Invisible: this.NotifyActionEvent(VTActions.Invisible); break;
                    case GraphicsOptions.Visible: this.NotifyActionEvent(VTActions.InvisibleUnset); break;
                    case GraphicsOptions.CrossedOut: this.NotifyActionEvent(VTActions.CrossedOut); break;
                    case GraphicsOptions.NotCrossedOut: this.NotifyActionEvent(VTActions.CrossedOutUnset); break;
                    case GraphicsOptions.Negative: this.NotifyActionEvent(VTActions.ReverseVideo); break;
                    case GraphicsOptions.Positive: this.NotifyActionEvent(VTActions.ReverseVideoUnset); break;
                    case GraphicsOptions.Underline: this.NotifyActionEvent(VTActions.Underline); break;
                    case GraphicsOptions.DoublyUnderlined: this.NotifyActionEvent(VTActions.DoublyUnderlined); break;
                    case GraphicsOptions.NoUnderline:
                        {
                            this.NotifyActionEvent(VTActions.UnderlineUnset);
                            this.NotifyActionEvent(VTActions.DoublyUnderlinedUnset);
                            break;
                        }

                    case GraphicsOptions.Overline: this.NotifyActionEvent(VTActions.Overlined); break;
                    case GraphicsOptions.NoOverline: this.NotifyActionEvent(VTActions.OverlinedUnset); break;

                    case GraphicsOptions.ForegroundBlack: this.NotifyActionEvent(VTActions.Foreground, VTForeground.DarkBlack); break;
                    case GraphicsOptions.ForegroundBlue: this.NotifyActionEvent(VTActions.Foreground, VTForeground.DarkBlue); break;
                    case GraphicsOptions.ForegroundGreen: this.NotifyActionEvent(VTActions.Foreground, VTForeground.DarkGreen); break;
                    case GraphicsOptions.ForegroundCyan: this.NotifyActionEvent(VTActions.Foreground, VTForeground.DarkCyan); break;
                    case GraphicsOptions.ForegroundRed: this.NotifyActionEvent(VTActions.Foreground, VTForeground.DarkRed); break;
                    case GraphicsOptions.ForegroundMagenta: this.NotifyActionEvent(VTActions.Foreground, VTForeground.DarkMagenta); break;
                    case GraphicsOptions.ForegroundYellow: this.NotifyActionEvent(VTActions.Foreground, VTForeground.DarkYellow); break;
                    case GraphicsOptions.ForegroundWhite: this.NotifyActionEvent(VTActions.Foreground, VTForeground.DarkWhite); break;

                    case GraphicsOptions.BackgroundBlack: this.NotifyActionEvent(VTActions.Background, VTForeground.DarkWhite); break;
                    case GraphicsOptions.BackgroundBlue: this.NotifyActionEvent(VTActions.Background, VTForeground.DarkBlue); break;
                    case GraphicsOptions.BackgroundGreen: this.NotifyActionEvent(VTActions.Background, VTForeground.DarkGreen); break;
                    case GraphicsOptions.BackgroundCyan: this.NotifyActionEvent(VTActions.Background, VTForeground.DarkCyan); break;
                    case GraphicsOptions.BackgroundRed: this.NotifyActionEvent(VTActions.Background, VTForeground.DarkRed); break;
                    case GraphicsOptions.BackgroundMagenta: this.NotifyActionEvent(VTActions.Background, VTForeground.DarkMagenta); break;
                    case GraphicsOptions.BackgroundYellow: this.NotifyActionEvent(VTActions.Background, VTForeground.DarkYellow); break;
                    case GraphicsOptions.BackgroundWhite: this.NotifyActionEvent(VTActions.Background, VTForeground.DarkWhite); break;

                    case GraphicsOptions.BrightForegroundBlack: this.NotifyActionEvent(VTActions.Foreground, VTForeground.BrightWhite); break;
                    case GraphicsOptions.BrightForegroundBlue: this.NotifyActionEvent(VTActions.Foreground, VTForeground.BrightBlue); break;
                    case GraphicsOptions.BrightForegroundGreen: this.NotifyActionEvent(VTActions.Foreground, VTForeground.BrightGreen); break;
                    case GraphicsOptions.BrightForegroundCyan: this.NotifyActionEvent(VTActions.Foreground, VTForeground.BrightCyan); break;
                    case GraphicsOptions.BrightForegroundRed: this.NotifyActionEvent(VTActions.Foreground, VTForeground.BrightRed); break;
                    case GraphicsOptions.BrightForegroundMagenta: this.NotifyActionEvent(VTActions.Foreground, VTForeground.BrightMagenta); break;
                    case GraphicsOptions.BrightForegroundYellow: this.NotifyActionEvent(VTActions.Foreground, VTForeground.BrightYellow); break;
                    case GraphicsOptions.BrightForegroundWhite: this.NotifyActionEvent(VTActions.Foreground, VTForeground.BrightWhite); break;

                    case GraphicsOptions.BrightBackgroundBlack: this.NotifyActionEvent(VTActions.Background, VTForeground.BrightWhite); break;
                    case GraphicsOptions.BrightBackgroundBlue: this.NotifyActionEvent(VTActions.Background, VTForeground.BrightBlue); break;
                    case GraphicsOptions.BrightBackgroundGreen: this.NotifyActionEvent(VTActions.Background, VTForeground.BrightGreen); break;
                    case GraphicsOptions.BrightBackgroundCyan: this.NotifyActionEvent(VTActions.Background, VTForeground.BrightCyan); break;
                    case GraphicsOptions.BrightBackgroundRed: this.NotifyActionEvent(VTActions.Background, VTForeground.BrightRed); break;
                    case GraphicsOptions.BrightBackgroundMagenta: this.NotifyActionEvent(VTActions.Background, VTForeground.BrightMagenta); break;
                    case GraphicsOptions.BrightBackgroundYellow: this.NotifyActionEvent(VTActions.Background, VTForeground.BrightYellow); break;
                    case GraphicsOptions.BrightBackgroundWhite: this.NotifyActionEvent(VTActions.Background, VTForeground.BrightWhite); break;

                    case GraphicsOptions.ForegroundExtended:
                        {
                            byte r, g, b;
                            i += this.SetRgbColorsHelper(parameters.Skip(i + 1).ToList(), true, out r, out g, out b);
                            this.NotifyActionEvent(VTActions.ForegroundRGB, r, g, b);
                            break;
                        }

                    case GraphicsOptions.BackgroundExtended:
                        {
                            byte r, g, b;
                            i += this.SetRgbColorsHelper(parameters.Skip(i + 1).ToList(), false, out r, out g, out b);
                            this.NotifyActionEvent(VTActions.BackgroundRGB, r, g, b);
                            break;
                        }

                    default:
                        logger.WarnFormat("未实现的SGRCode = {0}", option);
                        break;
                }
            }
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
            //// 先获取子显示屏
            //IPresentationDevice subDevice = this.terminal.GetActivePresentationDevice();

            //// 切换主屏幕
            //if (!this.terminal.SwitchPresentationDevice(this.presentationDevice))
            //{
            //    logger.Error("切换主显示屏失败");
            //    return false;
            //}

            //// 还原鼠标状态
            //this.terminal.CursorRestoreState(this.cursorState);

            //// 删除子显示屏
            //this.terminal.DeletePresentationDevice(subDevice);

            //return true;
            throw new NotImplementedException();
        }

        private bool UseAlternateScreenBuffer()
        {
            //this.cursorState = this.terminal.CursorSaveState();

            //// 首先保存主屏幕设备
            //this.presentationDevice = this.terminal.GetActivePresentationDevice();

            //// 再创建一个新的屏幕设备
            //IPresentationDevice device = this.terminal.CreatePresentationDevice();
            //if (device == null)
            //{
            //    logger.Error("创建显示屏失败");
            //    return false;
            //}

            //return this.terminal.SwitchPresentationDevice(device);
            throw new NotImplementedException();
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
                    case DECPrivateMode.DECCKM_CursorKeysMode:
                        {
                            // set - Enable Application Mode, reset - Normal mode

                            // true表示ApplicationMode
                            // false表示NormalMode
                            this.isApplicationMode = enable;
                            this.NotifyActionEvent(VTActions.SetCursorKeyMode, enable ? VTCursorKeyMode.ApplicationMode : VTCursorKeyMode.NormalMode);
                            break;
                        }

                    case DECPrivateMode.DECANM_AnsiMode:
                        {
                            this.isAnsiMode = enable;
                            this.NotifyActionEvent(VTActions.SetVTMode, enable ? VTMode.AnsiMode : VTMode.VT52Mode);
                            break;
                        }

                    case DECPrivateMode.ASB_AlternateScreenBuffer:
                        {
                            success = enable ? this.UseAlternateScreenBuffer() : this.UseMainScreenBuffer();
                            break;
                        }

                    case DECPrivateMode.DECTCEM_TextCursorEnableMode:
                        {
                            if (enable)
                            {
                                this.NotifyActionEvent(VTActions.CursorVisible);
                            }
                            else
                            {
                                this.NotifyActionEvent(VTActions.CursorHiden);
                            }
                            break;
                        }

                    case DECPrivateMode.XTERM_BracketedPasteMode:
                        {
                            break;
                        }

                    case DECPrivateMode.ATT610_StartCursorBlink:
                        {
                            break;
                        }

                    default:
                        logger.WarnFormat("未实现DECSETPrivateMode, {0}", mode);
                        break;
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
            int parameter = parameters.Count > 0 ? parameters[0] : 0;
            this.NotifyActionEvent(VTActions.EraseLine, parameter);
        }

        #endregion
    }
}
