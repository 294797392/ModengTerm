using ModengTerm.Document;
using ModengTerm.Terminal;
using ModengTerm.Terminal.Parsing;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using XTerminal.Base;

namespace ModengTerm.Terminal.Parsing
{
    /// <summary>
    /// 负责分发VTParser的事件
    /// 把指令透传给外部模块，外部模块去解析每个指令的参数
    /// </summary>
    public partial class VTParser
    {
        #region 类变量

        #endregion

        #region 公开事件

        #endregion

        #region 实例变量

        #endregion

        #region 属性

        /// <summary>
        /// 处理终端命令的处理器
        /// </summary>
        public VTDispatchHandler DispatchHandler { get; set; }

        #endregion

        #region 构造方法

        public VTParser()
        {
        }

        #endregion

        #region 公开接口

        private void ActionPrint(byte ch)
        {
            this.DispatchHandler.PrintCharacter(Convert.ToChar(ch));
        }

        private void ActionPrint(char ch)
        {
            this.DispatchHandler.PrintCharacter(ch);
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
                        // VT applications expect to be able to write NUL
                        // and have _nothing_ happen. Filter the NULs here, so they don't fill the
                        // buffer with empty spaces.
                        break;
                    }

                case ASCIITable.BEL:
                    {
                        // 响铃
                        this.DispatchHandler.PlayBell();
                        break;
                    }

                case ASCIITable.BS:
                    {
                        // Backspace，退格，光标向前移动一位
                        this.DispatchHandler.Backspace();
                        break;
                    }

                case ASCIITable.TAB:
                    {
                        // tab键
                        this.DispatchHandler.ForwardTab();
                        break;
                    }

                case ASCIITable.CR:
                    {
                        this.DispatchHandler.CarriageReturn();
                        break;
                    }

                case ASCIITable.LF:
                case ASCIITable.FF:
                case ASCIITable.VT:
                    {
                        // 这三个都是LF
                        this.DispatchHandler.LineFeed();
                        break;
                    }

                case ASCIITable.SI:
                case ASCIITable.SO:
                    {
                        // 这两个不知道是什么意思
                        logger.FatalFormat("未处理的SI和SO");
                        break;
                    }

                default:
                    {
                        this.DispatchHandler.PrintCharacter(Convert.ToChar(ch));
                        break;
                        //throw new NotImplementedException(string.Format("未实现的控制字符:{0}", ch));
                    }
            }
        }

        /// <summary>
        /// CSI状态解析完毕，开始执行CSI对应的动作
        /// </summary>
        /// <param name="finalByte">Final Byte</param>
        private void ActionCSIDispatch(int finalByte, List<int> parameters)
        {
            CsiActionCodes code = (CsiActionCodes)finalByte;

            switch (code)
            {
                case CsiActionCodes.SGR_SetGraphicsRendition:
                    {
                        // Modifies the graphical rendering options applied to the next characters written into the buffer.
                        // Options include colors, invert, underlines, and other "font style" type options.
                        this.PerformSetGraphicsRendition(parameters);
                        break;
                    }

                case CsiActionCodes.DECRST_PrivateModeReset:
                    {
                        this.PerformDECPrivateMode(parameters, false);
                        break;
                    }

                case CsiActionCodes.DECSET_PrivateModeSet:
                    {
                        this.PerformDECPrivateMode(parameters, true);
                        break;
                    }

                case CsiActionCodes.ED_EraseDisplay:
                    {
                        /// <summary>
                        /// This sequence erases some or all of the characters in the display according to the parameter. Any complete line erased by this sequence will return that line to single width mode. Editor Function
                        /// 0	Erase from the active position to the end of the screen, inclusive (default)
                        /// 1	Erase from start of the screen to the active position, inclusive
                        /// 2	Erase all of the display -- all lines are erased, changed to single-width, and the cursor does not move.
                        /// 
                        /// 触发场景：VIM
                        /// </summary>

                        VTEraseType eraseType = (VTEraseType)VTParameter.GetParameter(parameters, 0, 0);

                        this.DispatchHandler.EraseDisplay(eraseType);
                        break;
                    }

                case CsiActionCodes.HVP_HorizontalVerticalPosition:
                case CsiActionCodes.CUP_CursorPosition:
                    {
                        this.DispatchHandler.CUP_CursorPosition(this.parameters);
                        break;
                    }

                case CsiActionCodes.CUF_CursorForward:
                    {
                        int n = VTParameter.GetParameter(parameters, 0, 1);

                        this.DispatchHandler.CUF_CursorForward(n);
                        break;
                    }

                case CsiActionCodes.CUU_CursorUp:
                    {
                        int n = VTParameter.GetParameter(parameters, 0, 1);

                        this.DispatchHandler.CUU_CursorUp(n);
                        break;
                    }

                case CsiActionCodes.CUD_CursorDown:
                    {
                        int n = VTParameter.GetParameter(parameters, 0, 1);

                        this.DispatchHandler.CUD_CursorDown(n);
                        break;
                    }

                case CsiActionCodes.DTTERM_WindowManipulation:
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

                case CsiActionCodes.DECSTBM_SetScrollingRegion:
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


                        this.DispatchHandler.DECSTBM_SetScrollingRegion(parameters);
                        break;
                    }

                case CsiActionCodes.DECSLRM_SetLeftRightMargins:
                    {
                        /// Note that CSI s is dispatched as SetLeftRightScrollingMargins rather
                        // than CursorSaveState, so we don't test that here. The CursorSaveState
                        // will only be triggered by this sequence (in AdaptDispatch) when the
                        // Left-Right-Margin mode (DECLRMM) is disabled.

                        // 当DECLRMM启用的时候，执行设置左右边距的动作
                        // 当DECLRMM没启用的时候，执行CursorSaveState动作

                        int leftMargin = VTParameter.GetParameter(parameters, 0, 0);
                        int rightMargin = VTParameter.GetParameter(parameters, 1, 0);

                        this.DispatchHandler.DECSLRM_SetLeftRightMargins(leftMargin, rightMargin);
                        break;
                    }

                case CsiActionCodes.EL_EraseLine:
                    {
                        VTEraseType eraseType = (VTEraseType)VTParameter.GetParameter(this.parameters, 0, 0);

                        this.DispatchHandler.EL_EraseLine(eraseType);
                        break;
                    }

                case CsiActionCodes.DCH_DeleteCharacter:
                    {
                        // 从指定位置删除n个字符，删除后的字符串要左对齐，默认删除1个字符
                        int count = VTParameter.GetParameter(parameters, 0, 1);
                        this.DispatchHandler.DCH_DeleteCharacter(count);
                        break;
                    }

                case CsiActionCodes.ICH_InsertCharacter:
                    {
                        // 相关命令：
                        // MainDocument：sudo apt install pstat，然后回车，最后按方向键上回到历史命令
                        // AlternateDocument：暂无

                        // Insert Ps (Blank) Character(s) (default = 1) (ICH).
                        // 在当前光标处插入N个空白字符,这会将所有现有文本移到右侧。 向右溢出屏幕的文本会被删除
                        // 目前没发现这个操作对终端显示有什么影响，所以暂时不实现
                        int count = VTParameter.GetParameter(parameters, 0, 1);
                        this.DispatchHandler.ICH_InsertCharacter(count);
                        break;
                    }

                case CsiActionCodes.DSR_DeviceStatusReport:
                    {
                        // DSR，参考https://invisible-island.net/xterm/ctlseqs/ctlseqs.html

                        StatusType statusType = (StatusType)Convert.ToInt32(parameters[0]);
                        this.DispatchHandler.DSR_DeviceStatusReport(statusType);
                        break;
                    }

                case CsiActionCodes.DA_DeviceAttributes:
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

                        this.DispatchHandler.DA_DeviceAttributes(parameters);
                        break;
                    }

                case CsiActionCodes.IL_InsertLine:
                    {
                        // 将 <n> 行插入光标位置的缓冲区。 光标所在的行及其下方的行将向下移动。
                        int lines = VTParameter.GetParameter(parameters, 0, 1);
                        this.DispatchHandler.IL_InsertLine(lines);
                        break;
                    }

                case CsiActionCodes.DL_DeleteLine:
                    {
                        // 从缓冲区中删除<n> 行，从光标所在的行开始。
                        int lines = VTParameter.GetParameter(parameters, 0, 1);
                        this.DispatchHandler.DL_DeleteLine(lines);
                        break;
                    }

                case CsiActionCodes.CHA_CursorHorizontalAbsolute:
                    {
                        // 将光标移动到当前行中的第n列
                        int n = VTParameter.GetParameter(parameters, 0, -1);
                        if (n == -1)
                        {
                            break;
                        }

                        this.DispatchHandler.CHA_CursorHorizontalAbsolute(n - 1);
                        break;
                    }

                case CsiActionCodes.VPA_VerticalLinePositionAbsolute:
                    {
                        // 绝对垂直行位置 光标在当前列中垂直移动到第 <n> 个位置
                        // 保持列不变，把光标移动到指定的行处
                        int row = VTParameter.GetParameter(parameters, 0, 1);
                        row = Math.Max(0, row - 1);

                        this.DispatchHandler.VPA_VerticalLinePositionAbsolute(row);
                        break;
                    }

                case CsiActionCodes.SD_ScrollDown:
                    {
                        // Scroll down Ps lines (default = 1) (SD), VT420.
                        this.DispatchHandler.SD_ScrollDown(parameters);
                        break;
                    }

                case CsiActionCodes.SU_ScrollUp:
                    {
                        this.DispatchHandler.SU_ScrollUp(parameters);
                        break;
                    }

                case CsiActionCodes.ECH_EraseCharacters:
                    {
                        // 从当前光标处用空格填充n个字符
                        // Erase Characters from the current cursor position, by replacing them with a space

                        int count = VTParameter.GetParameter(parameters, 0, 1);
                        this.DispatchHandler.ECH_EraseCharacters(count);
                        break;
                    }

                case (CsiActionCodes)'~':
                    {
                        logger.ErrorFormat("不需要实现的CSIAction, ~");
                        break;
                    }

                default:
                    throw new NotImplementedException(string.Format("未实现CSIAction, {0}", (char)finalByte));
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
                case EscActionCodes.DECSC_CursorSave:
                    {
                        this.DispatchHandler.DECSC_CursorSave();
                        break;
                    }

                case EscActionCodes.DECRC_CursorRestore:
                    {
                        this.DispatchHandler.DECRC_CursorRestore();
                        break;
                    }

                case EscActionCodes.DECKPAM_KeypadApplicationMode:
                    {
                        this.DispatchHandler.DECKPAM_KeypadApplicationMode();
                        break;
                    }

                case EscActionCodes.DECKPNM_KeypadNumericMode:
                    {
                        this.DispatchHandler.DECKPNM_KeypadNumericMode();
                        break;
                    }

                case EscActionCodes.RI_ReverseLineFeed:
                    {
                        // Performs a "Reverse line feed", essentially, the opposite of '\n'.
                        //    Moves the cursor up one line, and tries to keep its position in the line
                        this.DispatchHandler.RI_ReverseLineFeed();
                        break;
                    }

                case EscActionCodes.SS2_SingleShift:
                    {
                        this.DispatchHandler.SS2_SingleShift();
                        break;
                    }

                case EscActionCodes.SS3_SingleShift:
                    {
                        this.DispatchHandler.SS3_SingleShift();
                        break;
                    }

                default:
                    {
                        if (this.parameters.Count > 0) 
                        {
                            /// <summary>
                            /// 指定要使用的字符集
                            /// https://learn.microsoft.com/zh-cn/windows/console/console-virtual-terminal-sequences
                            /// 
                            /// 参考：
                            /// terminal: OutputStateMachineEngine.cpp - OutputStateMachineEngine::ActionEscDispatch - default
                            /// https://invisible-island.net/xterm/ctlseqs/ctlseqs.html: ESC ( C
                            /// </summary>
                            /// <param name="commandChar"></param>
                            /// <param name="commandParameter">finalByte</param>

                            int commandChar = this.parameters[0];
                            int commandParameter = ch;

                            switch (commandChar)
                            {
                                case '(':
                                    {
                                        this.DispatchHandler.Designate94Charset(0, commandParameter);
                                        break;
                                    }

                                case ')':
                                    {
                                        this.DispatchHandler.Designate94Charset(1, commandParameter);
                                        break;
                                    }

                                case '*':
                                    {
                                        this.DispatchHandler.Designate94Charset(2, commandParameter);
                                        break;
                                    }

                                case '+':
                                    {
                                        this.DispatchHandler.Designate94Charset(3, commandParameter);
                                        break;
                                    }

                                case '-':
                                    {
                                        this.DispatchHandler.Designate96Charset(1, commandParameter);
                                        break;
                                    }

                                case '.':
                                    {
                                        this.DispatchHandler.Designate96Charset(2, commandParameter);
                                        break;
                                    }

                                case '/':
                                    {
                                        this.DispatchHandler.Designate96Charset(3, commandParameter);
                                        break;
                                    }

                                default:
                                    {
                                        throw new NotImplementedException(string.Format("未实现EscAction, finalByte = {0}", ch));
                                    }
                            }
                        }

                        throw new NotImplementedException(string.Format("未实现EscAction, {0}", code));
                    }
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
                    throw new NotImplementedException(string.Format("未实现VT52ActionCodes:{0}", code));
            }
        }

        #endregion

        #region 实例方法

        /// <summary>
        /// 代码参考自microsoft/terminal项目
        /// AdaptDispatch::SetGraphicsRendition
        /// 和terminal项目不同的地方是，这里会判断parameters里是否包含参数，如果不包含参数，那么它会被视为单个0参数
        /// 参考自：https://learn.microsoft.com/zh-cn/windows/console/console-virtual-terminal-sequences - 文本格式
        /// 
        /// SGR - Modifies the graphical rendering options applied to the next
        //   characters written into the buffer.
        //       - Options include colors, invert, underlines, and other "font style"
        //         type options.
        /// </summary>
        /// <param name="parameters"></param>
        private void PerformSetGraphicsRendition(List<int> parameters)
        {
            if (parameters.Count == 0)
            {
                // 如果未指定任何参数，它会被视为单个 0 参数
                parameters.Add(0);
            }

            int size = parameters.Count;

            for (int i = 0; i < size; i++)
            {
                byte option = (byte)parameters[i];

                VTColor rgbColor = null;

                switch ((GraphicsOptions)option)
                {
                    case GraphicsOptions.ForegroundExtended:
                        {
                            i += this.SetRgbColorsHelper(parameters, i + 1, out rgbColor);
                            break;
                        }

                    case GraphicsOptions.BackgroundExtended:
                        {
                            i += this.SetRgbColorsHelper(parameters, i + 1, out rgbColor);
                            break;
                        }

                    default:
                        break;
                }

                this.DispatchHandler.PerformSGR((GraphicsOptions)option, rgbColor);
            }
        }

        /// <summary>
        /// - Helper to parse extended graphics options, which start with 38 (FG) or 48 (BG)
        ///     These options are followed by either a 2 (RGB) or 5 (xterm index)
        ///      RGB sequences then take 3 MORE params to designate the R, G, B parts of the color
        ///      Xterm index will use the param that follows to use a color from the preset 256 color xterm color table.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="paramIndex">从第几个参数开始使用</param>
        /// <param name="rgbColor"></param>
        /// <returns></returns>
        private int SetRgbColorsHelper(List<int> parameters, int paramIndex, out VTColor rgbColor)
        {
            rgbColor = null;

            int optionsConsumed = 1;
            GraphicsOptions options = (GraphicsOptions)parameters[paramIndex];
            if (options == GraphicsOptions.RGBColorOrFaint)
            {
                // 这里使用RGB颜色
                optionsConsumed = 4;
                rgbColor = VTColor.CreateFromRgb((byte)parameters[paramIndex + 1], (byte)parameters[paramIndex + 2], (byte)parameters[paramIndex + 3], 255);
            }
            else if (options == GraphicsOptions.BlinkOrXterm256Index)
            {
                // 这里使用xterm颜色表里的颜色
                optionsConsumed = 2;
                int tableIndex = parameters.Count > paramIndex ? parameters[paramIndex + 1] : 0;
                if (tableIndex <= 255)
                {
                    byte r, g, b;
                    byte index = (byte)tableIndex;
                    Xterm256Color.ConvertRGB(index, out r, out g, out b);
                    rgbColor = VTColor.CreateFromRgb(r, g, b, 255);
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
                            // set - Enable Application Mode, reset - Normal mode

                            // true表示ApplicationMode
                            // false表示NormalMode
                            this.isApplicationMode = enable;
                            this.DispatchHandler.DECCKM_CursorKeysMode(enable);
                            break;
                        }

                    case DECPrivateMode.DECANM_AnsiMode:
                        {
                            this.isAnsiMode = enable;
                            this.DispatchHandler.DECANM_AnsiMode(enable);
                            break;
                        }

                    case DECPrivateMode.DECAWM_AutoWrapMode:
                        {
                            this.DispatchHandler.DECAWM_AutoWrapMode(enable);
                            break;
                        }

                    case DECPrivateMode.ASB_AlternateScreenBuffer:
                        {
                            // 是否使用备用缓冲区
                            // 打开VIM等编辑器的时候会触发
                            this.DispatchHandler.ASB_AlternateScreenBuffer(enable);
                            break;
                        }

                    case DECPrivateMode.XTERM_BracketedPasteMode:
                        {
                            // Sets the XTerm bracketed paste mode. This controls whether pasted content is bracketed with control sequences to differentiate it from typed text.
                            this.DispatchHandler.XTERM_BracketedPasteMode(enable);
                            break;
                        }

                    case DECPrivateMode.ATT610_StartCursorBlink:
                        {
                            // 控制是否要闪烁光标
                            this.DispatchHandler.ATT610_StartCursorBlink(enable);
                            break;
                        }

                    case DECPrivateMode.DECTCEM_TextCursorEnableMode:
                        {
                            // 控制是否要显示光标
                            this.DispatchHandler.DECTCEM_TextCursorEnableMode(enable);
                            break;
                        }


                    case DECPrivateMode.VT200_MOUSE_MODE:
                        {
                            logger.FatalFormat("VT200_MOUSE_MODE有待实现");
                            break;
                        }

                    case (DECPrivateMode)4:
                        {
                            logger.FatalFormat("terminal没实现DECPrivateMode - 4");
                            break;
                        }

                    default:
                        throw new NotImplementedException(string.Format("未实现DECSETPrivateMode, {0}", mode));
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
