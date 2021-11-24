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
            this.terminal.Print((char)ch);
        }

        public void ActionPrint(string text)
        {
            this.terminal.Print(text);
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
                        this.terminal.WarningBell();
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
                        this.terminal.CarriageReturn();
                        break;
                    }

                case ASCIIChars.LF:
                case ASCIIChars.FF:
                case ASCIIChars.VT:
                    {
                        // 这三个都是LF
                        this.terminal.LineFeed();
                        break;
                    }

                case ASCIIChars.SI:
                case ASCIIChars.SO:
                    {
                        // 这两个不知道是什么意思
                        throw new NotImplementedException();
                    }

                default:
                    {
                        //throw new NotImplementedException(string.Format("未实现的控制字符:{0}", ch));
                        this.terminal.Print((char)ch);
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

        #endregion

        #region 实例方法

        /// <summary>
        /// 代码从terminal里复制
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
                            this.terminal.SetDefaultAttributes();
                            this.terminal.SetDefaultBackground();
                            this.terminal.SetDefaultForeground();
                            break;
                        }

                    case GraphicsOptions.ForegroundDefault:
                        {
                            this.terminal.SetDefaultForeground();
                            break;
                        }

                    case GraphicsOptions.BackgroundDefault:
                        {
                            this.terminal.SetDefaultBackground();
                            break;
                        }

                    case GraphicsOptions.BoldBright:
                        {
                            this.terminal.SetBold(true);
                            break;
                        }

                    case GraphicsOptions.RGBColorOrFaint:
                        {
                            // 降低颜色强度
                            this.terminal.SetFaint(true);
                            break;
                        }

                    case GraphicsOptions.NotBoldOrFaint:
                        {
                            // 还原颜色强度和粗细
                            this.terminal.SetBold(false);
                            this.terminal.SetFaint(false);
                            break;
                        }

                    case GraphicsOptions.Italics:
                        {
                            this.terminal.SetItalics(true);
                            break;
                        }

                    case GraphicsOptions.NotItalics:
                        {
                            this.terminal.SetItalics(false);
                            break;
                        }

                    case GraphicsOptions.BlinkOrXterm256Index:
                    case GraphicsOptions.RapidBlink:
                        {
                            this.terminal.SetBlinking(true);
                            break;
                        }

                    case GraphicsOptions.Steady:
                        {
                            this.terminal.SetBlinking(false);
                            break;
                        }

                    case GraphicsOptions.Invisible:
                        {
                            // 隐藏字符
                            this.terminal.SetInvisible(true);
                            break;
                        }

                    case GraphicsOptions.Visible:
                        {
                            // 显示字符
                            this.terminal.SetInvisible(false);
                            break;
                        }

                    case GraphicsOptions.CrossedOut:
                        {
                            // characters still legible but marked as to be deleted
                            // 仍可读但标记为可删除的字符
                            this.terminal.SetCrossedOut(true);
                            break;
                        }

                    case GraphicsOptions.NotCrossedOut:
                        {
                            this.terminal.SetCrossedOut(false);
                            break;
                        }

                    case GraphicsOptions.Negative:
                        {
                            // negative image
                            // 图像反色？
                            this.terminal.SetReverseVideo(true);
                            break;
                        }

                    case GraphicsOptions.Positive:
                        {
                            this.terminal.SetReverseVideo(false);
                            break;
                        }

                    case GraphicsOptions.Underline:
                        {
                            this.terminal.SetUnderline(true);
                            break;
                        }

                    case GraphicsOptions.DoublyUnderlined:
                        {
                            this.terminal.SetDoublyUnderlined(true);
                            break;
                        }

                    case GraphicsOptions.NoUnderline:
                        {
                            this.terminal.SetUnderline(false);
                            this.terminal.SetDoublyUnderlined(false);
                            break;
                        }

                    case GraphicsOptions.Overline:
                        {
                            this.terminal.SetOverlined(true);
                            break;
                        }

                    case GraphicsOptions.NoOverline:
                        {
                            this.terminal.SetOverlined(false);
                            break;
                        }

                    case GraphicsOptions.ForegroundBlack:
                        {
                            this.terminal.SetIndexedForeground(TextColor.DARK_BLACK);
                            break;
                        }

                    case GraphicsOptions.ForegroundBlue:
                        {
                            this.terminal.SetIndexedForeground(TextColor.DARK_BLUE);
                            break;
                        }

                    case GraphicsOptions.ForegroundGreen:
                        {
                            this.terminal.SetIndexedForeground(TextColor.DARK_GREEN);
                            break;
                        }

                    case GraphicsOptions.ForegroundCyan:
                        {
                            this.terminal.SetIndexedForeground(TextColor.DARK_CYAN);
                            break;
                        }

                    case GraphicsOptions.ForegroundRed:
                        {
                            this.terminal.SetIndexedForeground(TextColor.DARK_RED);
                            break;
                        }

                    case GraphicsOptions.ForegroundMagenta:
                        {
                            this.terminal.SetIndexedForeground(TextColor.DARK_MAGENTA);
                            break;
                        }

                    case GraphicsOptions.ForegroundYellow:
                        {
                            this.terminal.SetIndexedForeground(TextColor.DARK_YELLOW);
                            break;
                        }

                    case GraphicsOptions.ForegroundWhite:
                        {
                            this.terminal.SetIndexedForeground(TextColor.DARK_WHITE);
                            break;
                        }

                    case GraphicsOptions.BackgroundBlack:
                        {
                            this.terminal.SetIndexedBackground(TextColor.DARK_BLACK);
                            break;
                        }

                    case GraphicsOptions.BackgroundBlue:
                        {
                            this.terminal.SetIndexedBackground(TextColor.DARK_BLUE);
                            break;
                        }

                    case GraphicsOptions.BackgroundGreen:
                        {
                            this.terminal.SetIndexedBackground(TextColor.DARK_GREEN);
                            break;
                        }

                    case GraphicsOptions.BackgroundCyan:
                        {
                            this.terminal.SetIndexedBackground(TextColor.DARK_CYAN);
                            break;
                        }

                    case GraphicsOptions.BackgroundRed:
                        this.terminal.SetIndexedBackground(TextColor.DARK_RED);
                        break;
                    case GraphicsOptions.BackgroundMagenta:
                        this.terminal.SetIndexedBackground(TextColor.DARK_MAGENTA);
                        break;
                    case GraphicsOptions.BackgroundYellow:
                        this.terminal.SetIndexedBackground(TextColor.DARK_YELLOW);
                        break;
                    case GraphicsOptions.BackgroundWhite:
                        this.terminal.SetIndexedBackground(TextColor.DARK_WHITE);
                        break;
                    case GraphicsOptions.BrightForegroundBlack:
                        this.terminal.SetIndexedForeground(TextColor.BRIGHT_BLACK);
                        break;
                    case GraphicsOptions.BrightForegroundBlue:
                        this.terminal.SetIndexedForeground(TextColor.BRIGHT_BLUE);
                        break;
                    case GraphicsOptions.BrightForegroundGreen:
                        this.terminal.SetIndexedForeground(TextColor.BRIGHT_GREEN);
                        break;
                    case GraphicsOptions.BrightForegroundCyan:
                        this.terminal.SetIndexedForeground(TextColor.BRIGHT_CYAN);
                        break;
                    case GraphicsOptions.BrightForegroundRed:
                        this.terminal.SetIndexedForeground(TextColor.BRIGHT_RED);
                        break;
                    case GraphicsOptions.BrightForegroundMagenta:
                        this.terminal.SetIndexedForeground(TextColor.BRIGHT_MAGENTA);
                        break;
                    case GraphicsOptions.BrightForegroundYellow:
                        this.terminal.SetIndexedForeground(TextColor.BRIGHT_YELLOW);
                        break;
                    case GraphicsOptions.BrightForegroundWhite:
                        this.terminal.SetIndexedForeground(TextColor.BRIGHT_WHITE);
                        break;
                    case GraphicsOptions.BrightBackgroundBlack:
                        this.terminal.SetIndexedBackground(TextColor.BRIGHT_BLACK);
                        break;
                    case GraphicsOptions.BrightBackgroundBlue:
                        this.terminal.SetIndexedBackground(TextColor.BRIGHT_BLUE);
                        break;
                    case GraphicsOptions.BrightBackgroundGreen:
                        this.terminal.SetIndexedBackground(TextColor.BRIGHT_GREEN);
                        break;
                    case GraphicsOptions.BrightBackgroundCyan:
                        this.terminal.SetIndexedBackground(TextColor.BRIGHT_CYAN);
                        break;
                    case GraphicsOptions.BrightBackgroundRed:
                        this.terminal.SetIndexedBackground(TextColor.BRIGHT_RED);
                        break;
                    case GraphicsOptions.BrightBackgroundMagenta:
                        this.terminal.SetIndexedBackground(TextColor.BRIGHT_MAGENTA);
                        break;
                    case GraphicsOptions.BrightBackgroundYellow:
                        this.terminal.SetIndexedBackground(TextColor.BRIGHT_YELLOW);
                        break;
                    case GraphicsOptions.BrightBackgroundWhite:
                        this.terminal.SetIndexedBackground(TextColor.BRIGHT_WHITE);
                        break;

                    case GraphicsOptions.ForegroundExtended:
                        {
                            byte r, g, b;
                            i += this.SetRgbColorsHelper(parameters.Skip(i + 1).ToList(), true, out r, out g, out b);
                            this.terminal.SetForeground(r, g, b);
                            break;
                        }

                    case GraphicsOptions.BackgroundExtended:
                        {
                            byte r, g, b;
                            i += this.SetRgbColorsHelper(parameters.Skip(i + 1).ToList(), false, out r, out g, out b);
                            this.terminal.SetBackground(r, g, b);
                            break;
                        }

                    default:
                        //logger.WarnFormat("未实现的SGRCode = {0}", option);
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
        /// <param name="set">启用或者禁用</param>
        private bool PerformDECPrivateMode(List<int> privateModes, bool set)
        {
            bool success = false;

            foreach (int mode in privateModes)
            {
                switch ((DECSETPrivateModeSet)mode)
                {
                    case DECSETPrivateModeSet.DECCKM_CursorKeysMode:
                        {
                            // set - Enable Application Mode, reset - Normal mode
                            break;
                        }

                    case DECSETPrivateModeSet.ASB_AlternateScreenBuffer:
                        {
                            success = set ? this.UseAlternateScreenBuffer() : this.UseMainScreenBuffer();
                            break;
                        }

                    case DECSETPrivateModeSet.DECTCEM_TextCursorEnableMode:
                        {
                            success = this.terminal.CursorVisibility(set);
                            break;
                        }

                    case DECSETPrivateModeSet.XTERM_BracketedPasteMode:
                        {
                            break;
                        }

                    case DECSETPrivateModeSet.ATT610_StartCursorBlink:
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
            foreach (int eraseType in parameters)
            {

            }
        }

        #endregion
    }
}
