using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XTerminal.Base;

namespace XTerminal.Parser
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
            this.NotifyActionEvent(VTActions.Print, (char)ch);
        }

        private void ActionPrint(char ch)
        {
            this.NotifyActionEvent(VTActions.Print, ch);
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
                        logger.ErrorFormat("Action - NUL");
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
                        //logger.DebugFormat("Action - CR");
                        this.NotifyActionEvent(VTActions.CarriageReturn);
                        break;
                    }

                case ASCIITable.LF:
                    {
                        //logger.DebugFormat("Action - LF");
                        this.NotifyActionEvent(VTActions.LF);
                        break;
                    }

                case ASCIITable.FF:
                    {
                        //logger.DebugFormat("Action - LF");
                        this.NotifyActionEvent(VTActions.FF);
                        break;
                    }

                case ASCIITable.VT:
                    {
                        // 这三个都是LF
                        //logger.DebugFormat("Action - VT");
                        this.NotifyActionEvent(VTActions.VT);
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
                        this.PerformDECPrivateMode(parameters, false);
                        break;
                    }

                case CSIActionCodes.DECSET_PrivateModeSet:
                    {
                        this.PerformDECPrivateMode(parameters, true);
                        break;
                    }

                case CSIActionCodes.ED_EraseDisplay:
                    {
                        /// <summary>
                        /// This sequence erases some or all of the characters in the display according to the parameter. Any complete line erased by this sequence will return that line to single width mode. Editor Function
                        /// 0	Erase from the active position to the end of the screen, inclusive (default)
                        /// 1	Erase from start of the screen to the active position, inclusive
                        /// 2	Erase all of the display -- all lines are erased, changed to single-width, and the cursor does not move.
                        /// 
                        /// 触发场景：VIM
                        /// </summary>

                        int parameter = Convert.ToInt32(parameters[0]);
                        logger.DebugFormat("CSIDispatch - ED_EraseDisplay, parameter = {0}", parameter);
                        this.NotifyActionEvent(VTActions.ED_EraseDisplay, parameter);
                        break;
                    }

                case CSIActionCodes.HVP_HorizontalVerticalPosition:
                    {
                        logger.ErrorFormat("CSIDispatch - HVP_HorizontalVerticalPosition");
                        break;
                    }

                case CSIActionCodes.CUP_CursorPosition:
                    {
                        int row = 0, col = 0;
                        if (parameters.Count == 2)
                        {
                            // VT的光标原点是(1,1)，我们程序里的是(0,0)，所以要减1
                            row = parameters[0] - 1;
                            col = parameters[1] - 1;
                        }
                        else 
                        {
                            // 如果没有参数，那么说明就是定位到原点(0,0)
                        }
                        this.NotifyActionEvent(VTActions.CUP_CursorPosition, row, col);
                        break;
                    }

                case CSIActionCodes.CUF_CursorForward:
                    {
                        int n = parameters.Count > 0 ? Convert.ToInt32(parameters[0]) : 1;
                        logger.DebugFormat("CSIDispatch - CUF_CursorForward, {0}", n);
                        this.NotifyActionEvent(VTActions.CUF_CursorForward, n);
                        break;
                    }

                case CSIActionCodes.CUU_CursorUp:
                    {
                        int n = parameters.Count > 0 ? Convert.ToInt32(parameters[0]) : 1;
                        logger.DebugFormat("CSIDispatch - CUU_CursorUp, {0}", n);
                        this.NotifyActionEvent(VTActions.CUU_CursorUp, n);
                        break;
                    }

                case CSIActionCodes.CUD_CursorDown:
                    {
                        int n = parameters.Count > 0 ? Convert.ToInt32(parameters[0]) : 1;
                        logger.DebugFormat("CSIDispatch - CUD_CursorDown, {0}", n);
                        this.NotifyActionEvent(VTActions.CUD_CursorDown, n);
                        break;
                    }

                case CSIActionCodes.DTTERM_WindowManipulation:
                    {
                        /// <summary>
                        /// Set Top and Bottom Margins
                        /// This sequence sets the top and bottom margins to define the scrolling region.The first parameter is the line number of the first line in the scrolling region; the second parameter is the line number of the bottom line in the scrolling region.Default is the entire screen(no margins).The minimum size of the scrolling region allowed is two lines, i.e., the top margin must be less than the bottom margin. The cursor is placed in the home position(see Origin Mode DECOM).
                        /// 
                        /// 触发场景：VIM
                        /// </summary>

                        this.PerformWindowManipulation(parameters);
                        break;
                    }

                case CSIActionCodes.DECSTBM_SetScrollingRegion:
                    {
                        /// <summary>
                        /// Set Top and Bottom Margins
                        /// This sequence sets the top and bottom margins to define the scrolling region.The first parameter is the line number of the first line in the scrolling region; the second parameter is the line number of the bottom line in the scrolling region.Default is the entire screen(no margins).The minimum size of the scrolling region allowed is two lines, i.e., the top margin must be less than the bottom margin. The cursor is placed in the home position(see Origin Mode DECOM).
                        /// 
                        /// 触发场景：VIM
                        /// </summary>

                        // topMargin：is the line number for the top margin.
                        // Default: Pt = 1
                        // bottomMargin：is the line number for the bottom margin.
                        // Default: Pb = current number of lines per screen
                        int topMargin = this.GetParameter(parameters, 0, 1);
                        int bottomMargin = this.GetParameter(parameters, 0, 0);
                        logger.ErrorFormat("CSIDispatch - DECSTBM_SetScrollingRegion, topMargin = {0}, bottomMargin = {1}", topMargin, bottomMargin);
                        this.NotifyActionEvent(VTActions.DECSTBM_SetScrollingRegion, topMargin, bottomMargin);
                        //throw new NotImplementedException();
                        break;
                    }

                case CSIActionCodes.EL_EraseLine:
                    {
                        EraseType eraseType = parameters.Count > 0 ? (EraseType)Convert.ToInt32(parameters[0]) : EraseType.ToEnd;
                        this.NotifyActionEvent(VTActions.EL_EraseLine, eraseType);
                        break;
                    }

                case CSIActionCodes.DCH_DeleteCharacter:
                    {
                        logger.DebugFormat("CSIDispatch - DCH_DeleteCharacter, {0}", parameters[0]);
                        this.NotifyActionEvent(VTActions.DCH_DeleteCharacter, parameters[0]);
                        break;
                    }

                case CSIActionCodes.ICH_InsertCharacter:
                    {
                        logger.ErrorFormat("未实现CSIDispatch - ICH_InsertCharacter, {0}", parameters[0]);
                        this.NotifyActionEvent(VTActions.ICH_InsertCharacter, parameters[0]);
                        break;
                    }

                case CSIActionCodes.DSR_DeviceStatusReport:
                    {
                        StatusType statusType = (StatusType)Convert.ToInt32(parameters[0]);
                        logger.DebugFormat("CSIActionCodes - DSR_DeviceStatusReport, statusType = {0}", statusType);
                        this.NotifyActionEvent(VTActions.DSR_DeviceStatusReport, statusType);
                        break;
                    }

                case CSIActionCodes.DA_DeviceAttributes:
                    {
                        /*****************************************************************
                         *    CSI Ps c Send Device Attributes(Primary DA).
                                Ps = 0  or omitted ⇒  request attributes from terminal.The response depends on the decTerminalID resource setting.
                                ⇒  CSI ? 1 ; 2 c("VT100 with Advanced Video Option")
                                ⇒  CSI ? 1 ; 0 c("VT101 with No Options")
                                ⇒  CSI ? 4 ; 6 c("VT132 with Advanced Video and Graphics")
                                ⇒  CSI ? 6 c("VT102")
                                ⇒  CSI ? 7 c("VT131")
                                ⇒  CSI ? 1 2; Ps c("VT125")
                                ⇒  CSI ? 6 2; Ps c("VT220")
                                ⇒  CSI ? 6 3; Ps c("VT320")
                                ⇒  CSI ? 6 4; Ps c("VT420")

                              The VT100-style response parameters do not mean anything by themselves.VT220(and higher) parameters do, telling the host what features the terminal supports:
                                Ps = 1  ⇒  132 - columns.
                                Ps = 2  ⇒  Printer.
                                Ps = 3  ⇒  ReGIS graphics.
                                Ps = 4  ⇒  Sixel graphics.
                                Ps = 6  ⇒  Selective erase.
                                Ps = 8  ⇒  User - defined keys.
                                  Ps = 9  ⇒  National Replacement Character sets.
                                Ps = 1 5  ⇒  Technical characters.
                                Ps = 1 6  ⇒  Locator port.
                                Ps = 1 7  ⇒  Terminal state interrogation.
                                Ps = 1 8  ⇒  User windows.
                                Ps = 2 1  ⇒  Horizontal scrolling.
                                Ps = 2 2  ⇒  ANSI color, e.g., VT525.
                                Ps = 2 8  ⇒  Rectangular editing.
                                Ps = 2 9  ⇒  ANSI text locator(i.e., DEC Locator mode).

                              XTerm supports part of the User windows feature, providing a
                              single page(which corresponds to its visible window).Rather
                              than resizing the font to change the number of lines/ columns
                              in a fixed-size display, xterm uses the window extension
                              controls(DECSNLS, DECSCPP, DECSLPP) to adjust its visible
                              window's size.  The "cursor coupling" controls (DECHCCM,
                              DECPCCM, DECVCCM) are ignored.
                        ********************************************************************/

                        logger.DebugFormat("CSIActionCodes - DA_DeviceAttributes");
                        this.NotifyActionEvent(VTActions.DA_DeviceAttributes);
                        break;
                    }

                default:
                    logger.ErrorFormat("未实现CSIAction, {0}", (char)finalByte);
                    throw new NotImplementedException();
            }
        }

        private int GetParameter(List<int> parameters, int index, int defaultParameter)
        {
            if (parameters.Count > index)
            {
                return parameters[index];
            }
            else
            {
                return defaultParameter;
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

                case EscActionCodes.RI_ReverseLineFeed:
                    {
                        // Performs a "Reverse line feed", essentially, the opposite of '\n'.
                        //    Moves the cursor up one line, and tries to keep its position in the line
                        logger.DebugFormat("ESCDispatch - RI_ReverseLineFeed");
                        this.NotifyActionEvent(VTActions.RI_ReverseLineFeed);
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
                            logger.DebugFormat("DECPrivateMode - DECCKM_CursorKeysMode, enable = {0}", enable);
                            // set - Enable Application Mode, reset - Normal mode

                            // true表示ApplicationMode
                            // false表示NormalMode
                            this.isApplicationMode = enable;
                            this.NotifyActionEvent(VTActions.SetCursorKeyMode, enable ? VTCursorKeyMode.ApplicationMode : VTCursorKeyMode.NormalMode);
                            break;
                        }

                    case DECPrivateMode.DECANM_AnsiMode:
                        {
                            logger.DebugFormat("DECPrivateMode - DECANM_AnsiMode, enable = {0}", enable);
                            this.isAnsiMode = enable;
                            this.NotifyActionEvent(VTActions.SetVTMode, enable ? VTMode.AnsiMode : VTMode.VT52Mode);
                            break;
                        }

                    case DECPrivateMode.DECAWM_AutoWrapMode:
                        {
                            logger.DebugFormat("DECPrivateMode - DECAWM_AutoWrapMode, enable = {0}", enable);
                            this.NotifyActionEvent(VTActions.AutoWrapMode, enable);
                            break;
                        }

                    case DECPrivateMode.ASB_AlternateScreenBuffer:
                        {
                            // 打开VIM等编辑器的时候会触发
                            this.NotifyActionEvent(enable ? VTActions.UseAlternateScreenBuffer : VTActions.UseMainScreenBuffer);
                            break;
                        }

                    case DECPrivateMode.XTERM_BracketedPasteMode:
                        {
                            // Sets the XTerm bracketed paste mode. This controls whether pasted content is bracketed with control sequences to differentiate it from typed text.
                            logger.DebugFormat("DECPrivateMode - XTERM_BracketedPasteMode");
                            this.NotifyActionEvent(VTActions.XTERM_BracketedPasteMode, enable);
                            break;
                        }

                    case DECPrivateMode.ATT610_StartCursorBlink:
                        {
                            // 控制是否要闪烁光标
                            logger.DebugFormat("DECPrivateMode - ATT610_StartCursorBlink");
                            this.NotifyActionEvent(VTActions.ATT610_StartCursorBlink, enable);
                            break;
                        }

                    case DECPrivateMode.DECTCEM_TextCursorEnableMode:
                        {
                            // 控制是否要显示光标
                            logger.DebugFormat("DECPrivateMode - DECTCEM_TextCursorEnableMode");
                            this.NotifyActionEvent(VTActions.DECTCEM_TextCursorEnableMode, enable);
                            break;
                        }

                    //case DECPrivateMode.DECTCEM_TextCursorEnableMode:
                    //    {
                    //        if (enable)
                    //        {
                    //            this.NotifyActionEvent(VTActions.CursorVisible);
                    //        }
                    //        else
                    //        {
                    //            this.NotifyActionEvent(VTActions.CursorHiden);
                    //        }
                    //        break;
                    //    }

                    default:
                        logger.ErrorFormat("未实现DECSETPrivateMode, {0}", mode);
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
        private void PerformWindowManipulation(List<int> parameters)
        {
            WindowManipulationType type = (WindowManipulationType)parameters[0];

            logger.ErrorFormat("未处理的WindowManipulationType, {0}", type);
        }

        #endregion
    }
}
