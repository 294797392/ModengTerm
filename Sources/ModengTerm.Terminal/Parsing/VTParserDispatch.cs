namespace ModengTerm.Terminal.Parsing
{
    /// <summary>
    /// 负责分发VTParser的事件
    /// 把指令和参数通过VTDispatchHandler透传给外部模块，外部模块去解析每个指令的参数
    /// 所有的指令注释都写在VTParser里
    /// </summary>
    public partial class VTParser
    {
        #region 类变量

        #endregion

        #region 公开事件

        public event Action<VTParser, ASCIITable> OnC0Actions;
        public event Action<VTParser, EscActionCodes, VTID> OnESCActions;
        public event Action<VTParser, char> OnPrint;
        public event Action<VTParser, CsiActionCodes, List<int>> OnCSIActions;

        #endregion

        #region 实例变量

        #endregion

        #region 属性

        #endregion

        #region 构造方法

        public VTParser()
        {
        }

        #endregion

        #region 公开接口

        private void ActionPrint(char ch)
        {
            this.WriteCode("Print");
            this.OnPrint?.Invoke(this, ch);
        }

        /// <summary>
        /// 执行C0字符集的控制字符
        /// </summary>
        /// <param name="ch"></param>
        private void ActionExecute(byte ch)
        {
            switch ((ASCIITable)ch)
            {
                case ASCIITable.NUL:
                    {
                        // do nothing
                        // VT applications expect to be able to write NUL
                        // and have _nothing_ happen. Filter the NULs here, so they don't fill the
                        // buffer with empty spaces.
                        this.WriteCode("NUL");
                        break;
                    }

                case ASCIITable.BEL:
                    {
                        // 响铃
                        this.WriteCode("BEL");
                        this.OnC0Actions?.Invoke(this, ASCIITable.BEL);
                        break;
                    }

                case ASCIITable.BS:
                    {
                        // Backspace，退格，光标向前移动一位
                        this.WriteCode("BS");
                        this.OnC0Actions?.Invoke(this, ASCIITable.BS);
                        break;
                    }

                case ASCIITable.TAB:
                    {
                        // tab键
                        this.WriteCode("TAB");
                        this.OnC0Actions?.Invoke(this, ASCIITable.TAB);
                        break;
                    }

                case ASCIITable.CR:
                    {
                        this.WriteCode("CR");
                        this.OnC0Actions?.Invoke(this, ASCIITable.CR);
                        break;
                    }

                case ASCIITable.LF:
                case ASCIITable.FF:
                case ASCIITable.VT:
                    {
                        // 这三个都是LF
                        this.WriteCode("LF");
                        this.OnC0Actions?.Invoke(this, ASCIITable.LF);
                        break;
                    }

                case ASCIITable.SI:
                    {
                        this.WriteCode("SI");
                        this.OnC0Actions?.Invoke(this, ASCIITable.SI);
                        break;
                    }

                case ASCIITable.SO:
                    {
                        this.WriteCode("SO");
                        this.OnC0Actions?.Invoke(this, ASCIITable.SO);
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

            switch (code)
            {
                case CsiActionCodes.SGR_SetGraphicsRendition:
                    {
                        // Modifies the graphical rendering options applied to the next characters written into the buffer.
                        // Options include colors, invert, underlines, and other "font style" type options.
                        this.WriteCode("SGR_SetGraphicsRendition");
                        this.OnCSIActions?.Invoke(this, code, parameters);
                        break;
                    }

                case CsiActionCodes.DECRST_PrivateModeReset:
                    {
                        this.WriteCode("DECRST_PrivateModeReset");
                        this.OnCSIActions?.Invoke(this, code, parameters);
                        break;
                    }

                case CsiActionCodes.DECSET_PrivateModeSet:
                    {
                        this.WriteCode("DECSET_PrivateModeSet");
                        this.OnCSIActions?.Invoke(this, code, parameters);
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
                        this.WriteCode("ED_EraseDisplay");
                        this.OnCSIActions?.Invoke(this, code, parameters);

                        break;
                    }

                case CsiActionCodes.CUB_CursorBackward:
                    {
                        // 	光标向后（左）<n> 行

                        this.WriteCode("CUB_CursorBackward");
                        this.OnCSIActions?.Invoke(this, code, parameters);
                        break;
                    }

                case CsiActionCodes.HVP_HorizontalVerticalPosition:
                case CsiActionCodes.CUP_CursorPosition:
                    {
                        this.WriteCode("HVP_HorizontalVerticalPosition__CUP_CursorPosition");
                        this.OnCSIActions?.Invoke(this, code, parameters);
                        break;
                    }

                case CsiActionCodes.CUF_CursorForward:
                    {
                        this.WriteCode("CUF_CursorForward");
                        this.OnCSIActions?.Invoke(this, code, parameters);
                        break;
                    }

                case CsiActionCodes.CUU_CursorUp:
                    {
                        this.WriteCode("CUU_CursorUp");
                        this.OnCSIActions?.Invoke(this, code, parameters);
                        break;
                    }

                case CsiActionCodes.CUD_CursorDown:
                    {
                        this.WriteCode("CUD_CursorDown");
                        this.OnCSIActions?.Invoke(this, code, parameters);
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

                        this.WriteCode("DTTERM_WindowManipulation");
                        this.OnCSIActions?.Invoke(this, code, parameters);
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

                        this.WriteCode("DECSTBM_SetScrollingRegion");
                        this.OnCSIActions?.Invoke(this, code, parameters);
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

                        this.WriteCode("DECSLRM_SetLeftRightMargins");
                        this.OnCSIActions?.Invoke(this, code, parameters);
                        break;
                    }

                case CsiActionCodes.EL_EraseLine:
                    {
                        // 使用空白字符填充光标所在行
                        // 注意空白字符需要应用当前字符样式

                        this.WriteCode("EL_EraseLine");
                        this.OnCSIActions?.Invoke(this, code, parameters);
                        break;
                    }

                case CsiActionCodes.DCH_DeleteCharacter:
                    {
                        // 从光标位置删除n个字符，删除后的字符串要左对齐，默认删除1个字符
                        // 删除的字符包含光标所在位置的字符

                        this.WriteCode("DCH_DeleteCharacter");
                        this.OnCSIActions?.Invoke(this, code, parameters);
                        break;
                    }

                case CsiActionCodes.ICH_InsertCharacter:
                    {
                        // 相关命令：
                        // MainDocument：sudo apt install pstat，然后回车，最后按方向键上回到历史命令
                        // AlternateDocument：暂无

                        // Insert Ps (Blank) Character(s) (default = 1) (ICH).
                        // 在当前光标处插入N个空白字符,这会将所有现有文本移到右侧。 向右溢出屏幕的文本会被删除

                        this.WriteCode("ICH_InsertCharacter");
                        this.OnCSIActions?.Invoke(this, code, parameters);
                        break;
                    }

                case CsiActionCodes.DSR_DeviceStatusReport:
                    {
                        // DSR，参考https://invisible-island.net/xterm/ctlseqs/ctlseqs.html

                        this.WriteCode("DSR_DeviceStatusReport");
                        this.OnCSIActions?.Invoke(this, code, parameters);
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

                        this.WriteCode("DA_DeviceAttributes");
                        this.OnCSIActions?.Invoke(this, code, parameters);
                        break;
                    }

                case CsiActionCodes.IL_InsertLine:
                    {
                        // 将 <n> 行插入光标位置的缓冲区。 光标所在的行及其下方的行将向下移动。

                        this.WriteCode("IL_InsertLine");
                        this.OnCSIActions?.Invoke(this, code, parameters);
                        break;
                    }

                case CsiActionCodes.DL_DeleteLine:
                    {
                        // 从缓冲区中删除<n> 行，从光标所在的行开始。

                        this.WriteCode("DL_DeleteLine");
                        this.OnCSIActions?.Invoke(this, code, parameters);
                        break;
                    }

                case CsiActionCodes.CHA_CursorHorizontalAbsolute:
                    {
                        // 将光标移动到当前行中的第n列

                        this.WriteCode("CHA_CursorHorizontalAbsolute");
                        this.OnCSIActions?.Invoke(this, code, parameters);
                        break;
                    }

                case CsiActionCodes.VPA_VerticalLinePositionAbsolute:
                    {
                        this.WriteCode("VPA_VerticalLinePositionAbsolute");
                        this.OnCSIActions?.Invoke(this, code, parameters);
                        break;
                    }

                case CsiActionCodes.SD_ScrollDown:
                    {
                        // Scroll down Ps lines (default = 1) (SD), VT420.

                        // 当您执行滚动命令时，实际上是文本在窗口内向上或向下移动，而并非窗口（或视口）本身在移动。例如，当您执行“向下滚动”时，新的文本会从下方进入视窗，而现有的文本则向上移出视窗，尽管这可能给人一种视窗在向上移动的错觉。
                        // 向下滚动 <n> 行。也被称为向上平移，新行从屏幕顶部填充进来。
                        // 这意味着当你执行“向下滚动”操作时，当前显示的文本会向下移动<n> 行，而新的文本行则会从屏幕的顶部出现，填补留下的空白。尽管这被称为“向下滚动”，但实际上是屏幕上的内容在向下移动，给人一种视窗向上移动的感觉。
                        // 经过在WindowsTerminal上测试，和光标位置没关系，光标位置不动，只移动可视区域的文本，注意要判断ScrollMargin

                        this.WriteCode("SD_ScrollDown");
                        this.OnCSIActions?.Invoke(this, code, parameters);
                        break;
                    }

                case CsiActionCodes.SU_ScrollUp:
                    {
                        this.WriteCode("SU_ScrollUp");
                        this.OnCSIActions?.Invoke(this, code, parameters);
                        break;
                    }

                case CsiActionCodes.ECH_EraseCharacters:
                    {
                        // 从当前光标处用空格填充n个字符
                        // 擦除当前光标位置的 <n> 个字符，方法是使用空格字符覆盖它们。
                        // Erase Characters from the current cursor position, by replacing them with a space

                        this.WriteCode("ECH_EraseCharacters");
                        this.OnCSIActions?.Invoke(this, code, parameters);
                        break;
                    }

                case CsiActionCodes.REP_RepeatCharacter:
                    {
                        // Repeat the preceding graphic character Ps times

                        this.WriteCode("REP_RepeatCharacter");
                        this.OnCSIActions?.Invoke(this, code, parameters);
                        break;
                    }

                case (CsiActionCodes)'~':
                    {
                        this.WriteCode("UnPerformed_CSI126_");
                        logger.ErrorFormat("不需要实现的CSIAction, ~");
                        break;
                    }

                default:
                    {
                        this.WriteCode(string.Format("UnkownCSIAction_{0}", finalByte));
                        logger.FatalFormat("未实现CSIAction, {0}", (char)finalByte);
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

            switch (code)
            {
                case EscActionCodes.DECSC_CursorSave:
                    {
                        this.WriteCode("DECSC_CursorSave");
                        this.OnESCActions?.Invoke(this, code, this.vtid);
                        break;
                    }

                case EscActionCodes.DECRC_CursorRestore:
                    {
                        this.WriteCode("DECRC_CursorRestore");
                        this.OnESCActions?.Invoke(this, code, this.vtid);
                        break;
                    }

                case EscActionCodes.DECKPAM_KeypadApplicationMode:
                    {
                        this.WriteCode("DECKPAM_KeypadApplicationMode");
                        this.OnESCActions?.Invoke(this, code, this.vtid);
                        break;
                    }

                case EscActionCodes.DECKPNM_KeypadNumericMode:
                    {
                        this.WriteCode("DECKPNM_KeypadNumericMode");
                        this.OnESCActions?.Invoke(this, code, this.vtid);
                        break;
                    }

                case EscActionCodes.RI_ReverseLineFeed:
                    {
                        // Performs a "Reverse line feed", essentially, the opposite of '\n'.
                        //    Moves the cursor up one line, and tries to keep its position in the line
                        this.WriteCode("RI_ReverseLineFeed");
                        this.OnESCActions?.Invoke(this, code, this.vtid);
                        break;
                    }

                case EscActionCodes.SS2_SingleShift:
                    {
                        this.WriteCode("SS2_SingleShift");
                        this.OnESCActions?.Invoke(this, code, this.vtid);
                        break;
                    }

                case EscActionCodes.SS3_SingleShift:
                    {
                        this.WriteCode("SS3_SingleShift");
                        this.OnESCActions?.Invoke(this, code, this.vtid);
                        break;
                    }

                case EscActionCodes.LS2_LockingShift:
                    {
                        // Invoke the G2 Character Set as GL (LS2).
                        this.WriteCode("LS2_LockingShift");
                        this.OnESCActions?.Invoke(this, code, this.vtid);
                        break;
                    }

                case EscActionCodes.LS3_LockingShift:
                    {
                        // Invoke the G3 Character Set as GL
                        this.WriteCode("LS3_LockingShift");
                        this.OnESCActions?.Invoke(this, code, this.vtid);
                        break;
                    }

                case EscActionCodes.LS1R_LockingShift:
                    {
                        this.WriteCode("LS1R_LockingShift");
                        this.OnESCActions?.Invoke(this, code, this.vtid);
                        break;
                    }

                case EscActionCodes.LS2R_LockingShift:
                    {
                        this.WriteCode("LS2R_LockingShift");
                        this.OnESCActions?.Invoke(this, code, this.vtid);
                        break;
                    }

                case EscActionCodes.LS3R_LockingShift:
                    {
                        this.WriteCode("LS3R_LockingShift");
                        this.OnESCActions?.Invoke(this, code, this.vtid);
                        break;
                    }

                default:
                    {
                        this.WriteCode("DesignateCharset");
                        this.OnESCActions?.Invoke(this, code, this.vtid);
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

            switch (code)
            {
                default:
                    {
                        this.WriteCode("UnkownVt52EscAction");
                        logger.ErrorFormat("未实现VT52ActionCodes:{0}", code);
                        break;
                    }
            }
        }

        #endregion

        #region 实例方法

        private void WriteCode(string action)
        {
            VTDebug.Context.WriteCode(action, this.sequenceBytes);
            this.sequenceBytes.Clear();
        }

        #endregion
    }
}
