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
    /// 把指令和参数通过VTDispatchHandler透传给外部模块，外部模块去解析每个指令的参数
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

        private void ActionPrint(char ch)
        {
            this.TraceAction("Print");
            this.DispatchHandler.PrintCharacter(ch);
        }

        /// <summary>
        /// 执行C0字符集的控制字符
        /// </summary>
        /// <param name="ch"></param>
        private void ActionExecute(byte ch)
        {
            switch (ch)
            {
                case ASCIITable.NUL:
                    {
                        // do nothing
                        // VT applications expect to be able to write NUL
                        // and have _nothing_ happen. Filter the NULs here, so they don't fill the
                        // buffer with empty spaces.
                        this.TraceAction("NUL");
                        break;
                    }

                case ASCIITable.BEL:
                    {
                        // 响铃
                        this.TraceAction("BEL");
                        this.DispatchHandler.PlayBell();
                        break;
                    }

                case ASCIITable.BS:
                    {
                        // Backspace，退格，光标向前移动一位
                        this.TraceAction("BS");
                        this.DispatchHandler.Backspace();
                        break;
                    }

                case ASCIITable.TAB:
                    {
                        // tab键
                        this.TraceAction("TAB");
                        this.DispatchHandler.ForwardTab();
                        break;
                    }

                case ASCIITable.CR:
                    {
                        this.TraceAction("CR");
                        this.DispatchHandler.CarriageReturn();
                        break;
                    }

                case ASCIITable.LF:
                case ASCIITable.FF:
                case ASCIITable.VT:
                    {
                        // 这三个都是LF
                        this.TraceAction("LF");

                        int oldRow = this.DispatchHandler.CursorRow;
                        int oldCol = this.DispatchHandler.CursorCol;

                        this.DispatchHandler.LineFeed();

                        int newRow = this.DispatchHandler.CursorRow;
                        int newCol = this.DispatchHandler.CursorCol;
                        
                        VTDebug.Context.WriteInteractive("LineFeed", "{0},{1},{2},{3}", oldRow, oldCol, newRow, newCol);

                        break;
                    }

                case ASCIITable.SI:
                case ASCIITable.SO:
                    {
                        // 这两个不知道是什么意思
                        this.TraceAction("SO");
                        logger.FatalFormat("未处理的SI和SO");
                        break;
                    }

                default:
                    {
                        this.ActionPrint(Convert.ToChar(ch));
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
            if (Enum.IsDefined<CsiActionCodes>(code))
            {
                this.TraceAction(code.ToString());
            }

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

                        int oldRow = this.DispatchHandler.CursorRow;
                        int oldCol = this.DispatchHandler.CursorCol;

                        this.DispatchHandler.EraseDisplay(eraseType);

                        int newRow = this.DispatchHandler.CursorRow;
                        int newCol = this.DispatchHandler.CursorCol;

                        VTDebug.Context.WriteInteractive("EraseDisplay", "{0},{1},{2},{3},{4}", oldRow, oldCol, newRow, newCol, eraseType);

                        break;
                    }

                case CsiActionCodes.CUB_CursorBackward:
                    {
                        // 	光标向后（左）<n> 行

                        int oldRow = this.DispatchHandler.CursorRow;
                        int oldCol = this.DispatchHandler.CursorCol;
                        int n = VTParameter.GetParameter(parameters, 0, 1);

                        VTDebug.Context.WriteInteractive("CUB_CursorBackward", "{0},{1},{2}", oldRow, oldCol, n);

                        this.DispatchHandler.CUF_CursorForward(n);

                        break;
                    }

                case CsiActionCodes.HVP_HorizontalVerticalPosition:
                case CsiActionCodes.CUP_CursorPosition:
                    {
                        int oldRow = this.DispatchHandler.CursorRow;
                        int oldCol = this.DispatchHandler.CursorCol;

                        int row = 0, col = 0;
                        if (parameters.Count == 2)
                        {
                            // VT的光标原点是(1,1)，我们程序里的是(0,0)，所以要减1
                            int newrow = parameters[0];
                            int newcol = parameters[1];

                            // 测试中发现在ubuntu系统上执行apt install或者apt remove命令，HVP会发送0列过来，这里处理一下，如果遇到参数是0，那么就直接变成0
                            row = newrow == 0 ? 0 : newrow - 1;
                            col = newcol == 0 ? 0 : newcol - 1;

                            int viewportRow = this.DispatchHandler.ViewportRow;
                            int viewportColumn = this.DispatchHandler.ViewportColumn;

                            // 对行和列做限制
                            if (row >= viewportRow)
                            {
                                row = viewportRow - 1;
                            }

                            if (col >= viewportColumn)
                            {
                                col = viewportColumn - 1;
                            }
                        }
                        else
                        {
                            // 如果没有参数，那么说明就是定位到原点(0,0)
                        }

                        this.DispatchHandler.CUP_CursorPosition(row, col);

                        int newRow = this.DispatchHandler.CursorRow;
                        int newCol = this.DispatchHandler.CursorCol;

                        VTDebug.Context.WriteInteractive("CUP_CursorPosition", "{0},{1},{2},{3}", oldRow, oldCol, newRow, newCol);

                        break;
                    }

                case CsiActionCodes.CUF_CursorForward:
                    {
                        int oldRow = this.DispatchHandler.CursorRow;
                        int oldCol = this.DispatchHandler.CursorCol;
                        int n = VTParameter.GetParameter(parameters, 0, 1);

                        VTDebug.Context.WriteInteractive("CUF_CursorForward", "{0},{1},{2}", oldRow, oldCol, n);

                        this.DispatchHandler.CUF_CursorForward(n);

                        break;
                    }

                case CsiActionCodes.CUU_CursorUp:
                    {
                        int oldRow = this.DispatchHandler.CursorRow;
                        int oldCol = this.DispatchHandler.CursorCol;
                        int n = VTParameter.GetParameter(parameters, 0, 1);

                        VTDebug.Context.WriteInteractive("CUU_CursorUp", "{0},{1},{2}", oldRow, oldCol, n);

                        this.DispatchHandler.CUU_CursorUp(n);
                        break;
                    }

                case CsiActionCodes.CUD_CursorDown:
                    {
                        int oldRow = this.DispatchHandler.CursorRow;
                        int oldCol = this.DispatchHandler.CursorCol;
                        int n = VTParameter.GetParameter(parameters, 0, 1);

                        VTDebug.Context.WriteInteractive("CUD_CursorDown", "{0},{1},{2}", oldRow, oldCol, n);

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


                        // 设置可滚动区域
                        // 不可以操作滚动区域以外的行，只能对滚动区域内的行进行操作
                        // 对于滚动区域的作用的解释，举个例子说明
                        // 比方说marginTop是1，marginBottom也是1
                        // 那么在执行LineFeed动作的时候，默认情况下，是把第一行挂到最后一行的后面，有了margin之后，就要把第二行挂到倒数第二行的后面
                        // ScrollMargin会对很多动作产生影响：LF，RI_ReverseLineFeed，DeleteLine，InsertLine

                        // 视频终端的规范里说，如果topMargin等于bottomMargin，或者bottomMargin大于屏幕高度，那么忽略这个指令
                        // 边距还会影响插入行 (IL) 和删除行 (DL)、向上滚动 (SU) 和向下滚动 (SD) 修改的行。

                        // Notes on DECSTBM
                        // * The value of the top margin (Pt) must be less than the bottom margin (Pb).
                        // * The maximum size of the scrolling region is the page size
                        // * DECSTBM moves the cursor to column 1, line 1 of the page
                        // * https://github.com/microsoft/terminal/issues/1849

                        // 当前终端屏幕可显示的行数量
                        int lines = this.DispatchHandler.ViewportRow;

                        int topMargin = VTParameter.GetParameter(parameters, 0, 1);
                        int bottomMargin = VTParameter.GetParameter(parameters, 1, lines);

                        if (bottomMargin < 0 || topMargin < 0)
                        {
                            logger.ErrorFormat("DECSTBM_SetScrollingRegion参数不正确，忽略本次设置, topMargin = {0}, bottomMargin = {1}", topMargin, bottomMargin);
                            return;
                        }
                        if (topMargin >= bottomMargin)
                        {
                            logger.ErrorFormat("DECSTBM_SetScrollingRegion参数不正确，topMargin大于等bottomMargin，忽略本次设置, topMargin = {0}, bottomMargin = {1}", topMargin, bottomMargin);
                            return;
                        }
                        if (bottomMargin > lines)
                        {
                            logger.ErrorFormat("DECSTBM_SetScrollingRegion参数不正确，bottomMargin大于当前屏幕总行数, bottomMargin = {0}, lines = {1}", bottomMargin, lines);
                            return;
                        }

                        // 如果topMargin等于1，那么就表示使用默认值，也就是没有marginTop，所以当topMargin == 1的时候，marginTop改为0
                        int marginTop = topMargin == 1 ? 0 : topMargin - 1;
                        // 如果bottomMargin等于控制台高度，那么就表示使用默认值，也就是没有marginBottom，所以当bottomMargin == 控制台高度的时候，marginBottom改为0
                        int marginBottom = lines - bottomMargin;

                        VTDebug.Context.WriteInteractive("DECSTBM_SetScrollingRegion", "topMargin1 = {0}, bottomMargin1 = {1}, topMargin2 = {2}, bottomMargin2 = {3}", topMargin, bottomMargin, marginTop, marginBottom);

                        this.DispatchHandler.DECSTBM_SetScrollingRegion(marginTop, marginBottom);
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

                        VTDebug.Context.WriteInteractive("DECSLRM_SetLeftRightMargins", "leftMargin = {0}, rightMargin = {1}", leftMargin, rightMargin);
                        logger.ErrorFormat("未实现DECSLRM_SetLeftRightMargins");

                        this.DispatchHandler.DECSLRM_SetLeftRightMargins(leftMargin, rightMargin);
                        break;
                    }

                case CsiActionCodes.EL_EraseLine:
                    {
                        // 使用空白字符填充该行
                        // 注意空白字符需要应用当前样式

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
                        int oldRow = this.DispatchHandler.CursorRow;
                        int oldCol = this.DispatchHandler.CursorCol;
                        // 将光标移动到当前行中的第n列
                        int n = VTParameter.GetParameter(parameters, 0, -1);

                        if (n == -1)
                        {
                            VTDebug.Context.WriteInteractive("CHA_CursorHorizontalAbsolute", "{0},{1},{2}, n是-1, 不执行操作", oldRow, oldCol, n);
                        }
                        else
                        {
                            VTDebug.Context.WriteInteractive("CHA_CursorHorizontalAbsolute", "{0},{1},{2}", oldRow, oldCol, n);

                            this.DispatchHandler.CHA_CursorHorizontalAbsolute(n - 1);
                        }

                        break;
                    }

                case CsiActionCodes.VPA_VerticalLinePositionAbsolute:
                    {
                        int oldRow = this.DispatchHandler.CursorRow;
                        int oldCol = this.DispatchHandler.CursorCol;

                        // 绝对垂直行位置 光标在当前列中垂直移动到第 <n> 个位置
                        // 保持列不变，把光标移动到指定的行处
                        int row = VTParameter.GetParameter(parameters, 0, 1);
                        row = Math.Max(0, row - 1);

                        this.DispatchHandler.VPA_VerticalLinePositionAbsolute(row);

                        int newRow = this.DispatchHandler.CursorRow;
                        int newCol = this.DispatchHandler.CursorCol;

                        VTDebug.Context.WriteInteractive("VPA_VerticalLinePositionAbsolute", "{0},{1},{2}", oldRow, oldCol, newRow, newCol);

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
                        // 擦除当前光标位置的 <n> 个字符，方法是使用空格字符覆盖它们。
                        // Erase Characters from the current cursor position, by replacing them with a space

                        int count = VTParameter.GetParameter(parameters, 0, 1);
                        this.DispatchHandler.ECH_EraseCharacters(count);
                        break;
                    }

                case (CsiActionCodes)'~':
                    {
                        this.TraceAction("UnPerformed_CSI126_");
                        logger.ErrorFormat("不需要实现的CSIAction, ~");
                        break;
                    }

                default:
                    {
                        this.TraceAction("UnkownCSIAction");
                        logger.ErrorFormat("未实现CSIAction, {0}", (char)finalByte);
                        break;
                    }
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
            this.vtid.Finalize(ch);

            EscActionCodes code = (EscActionCodes)ch;
            if (Enum.IsDefined<EscActionCodes>(code))
            {
                this.TraceAction(code.ToString());
            }

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
                        int oldRow = this.DispatchHandler.CursorRow;
                        int oldCol = this.DispatchHandler.CursorCol;

                        // Performs a "Reverse line feed", essentially, the opposite of '\n'.
                        //    Moves the cursor up one line, and tries to keep its position in the line
                        this.DispatchHandler.RI_ReverseLineFeed();

                        int newRow = this.DispatchHandler.CursorRow;
                        int newCol = this.DispatchHandler.CursorCol;

                        VTDebug.Context.WriteInteractive("RI_ReverseLineFeed", "{0},{1},{2},{3}", oldRow, oldCol, newRow, newCol);

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

                case EscActionCodes.LS2_LockingShift:
                    {
                        // Invoke the G2 Character Set as GL (LS2).
                        this.DispatchHandler.LS2_LockingShift();
                        break;
                    }

                case EscActionCodes.LS3_LockingShift:
                    {
                        // Invoke the G3 Character Set as GL
                        this.DispatchHandler.LS3_LockingShift();
                        break;
                    }

                case EscActionCodes.LS1R_LockingShift:
                    {
                        this.DispatchHandler.LS1R_LockingShift();
                        break;
                    }

                case EscActionCodes.LS2R_LockingShift:
                    {
                        this.DispatchHandler.LS2R_LockingShift();
                        break;
                    }

                case EscActionCodes.LS3R_LockingShift:
                    {
                        this.DispatchHandler.LS3R_LockingShift();
                        break;
                    }

                default:
                    {
                        if (!this.HandleDesignateCharset(this.vtid))
                        {
                            this.TraceAction("UnkownESCAction");
                            logger.ErrorFormat("未实现EscAction, {0}", code);
                        }
                        else
                        {
                            this.TraceAction("DesignateCharset");
                        }

                        break;
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
            if (Enum.IsDefined<VT52ActionCodes>(code))
            {
                this.TraceAction(code.ToString());
            }

            switch (code)
            {
                default:
                    {
                        this.TraceAction("UnkownVt52EscAction");
                        logger.ErrorFormat("未实现VT52ActionCodes:{0}", code);
                        break;
                    }
            }
        }

        /// <summary>
        /// 指定要使用的字符集
        /// https://learn.microsoft.com/zh-cn/windows/console/console-virtual-terminal-sequences
        /// 
        /// 参考：
        /// terminal: OutputStateMachineEngine.cpp - OutputStateMachineEngine::ActionEscDispatch - default
        /// https://invisible-island.net/xterm/ctlseqs/ctlseqs.html: ESC ( C
        /// </summary>
        /// <param name="vtid">VTID里不包含ESC字符</param>
        private bool HandleDesignateCharset(VTID vtid)
        {
            byte ch = vtid[0];
            ulong finalBytes = vtid.SubSequence(1);

            switch ((char)ch)
            {
                case '%':
                    {
                        //Routine Description:
                        // DOCS - Selects the coding system through which character sets are activated.
                        //     When ISO2022 is selected, the code page is set to ISO-8859-1, C1 control
                        //     codes are accepted, and both GL and GR areas of the code table can be
                        //     remapped. When UTF8 is selected, the code page is set to UTF-8, the C1
                        //     control codes are disabled, and only the GL area can be remapped.
                        //Arguments:
                        // - codingSystem - The coding system that will be selected.

                        CodingSystem codingSystem = (CodingSystem)vtid[1];

                        switch (codingSystem)
                        {
                            case CodingSystem.UTF8:
                                {
                                    this.acceptC1Control = true;
                                    break;
                                }

                            case CodingSystem.ISO2022:
                                {
                                    this.acceptC1Control = false;
                                    break;
                                }

                            default:
                                throw new NotImplementedException();
                        }

                        break;
                    }

                case '(':
                    {
                        this.DispatchHandler.Designate94Charset(0, finalBytes);
                        break;
                    }

                case ')':
                    {
                        this.DispatchHandler.Designate94Charset(1, finalBytes);
                        break;
                    }

                case '*':
                    {
                        this.DispatchHandler.Designate94Charset(2, finalBytes);
                        break;
                    }

                case '+':
                    {
                        this.DispatchHandler.Designate94Charset(3, finalBytes);
                        break;
                    }

                case '-':
                    {
                        this.DispatchHandler.Designate96Charset(1, finalBytes);
                        break;
                    }

                case '.':
                    {
                        this.DispatchHandler.Designate96Charset(2, finalBytes);
                        break;
                    }

                case '/':
                    {
                        this.DispatchHandler.Designate96Charset(3, finalBytes);
                        break;
                    }

                default:
                    {
                        logger.ErrorFormat("HandleDesignateCharset失败, 可能是未处理的ESC指令??");
                        return false;
                    }
            }

            return true;
        }

        #endregion

        #region 实例方法

        private void TraceAction(string action)
        {
            VTDebug.Context.WriteCode(action, this.sequenceBytes);
            this.sequenceBytes.Clear();
        }

        /// <summary>
        /// 代码参考自microsoft/terminal项目
        /// AdaptDispatch::SetGraphicsRendition
        /// 和terminal项目不同的地方是，这里会判断parameters里是否包含参数，如果不包含参数，那么它会被视为单个0参数
        /// 参考自：https://learn.microsoft.com/zh-cn/windows/console/console-virtual-terminal-sequences - 文本格式
        /// 
        /// SGR - Modifies the graphical rendering options applied to the next
        ///   characters written into the buffer.
        ///       - Options include colors, invert, underlines, and other "font style"
        ///         type options.
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
