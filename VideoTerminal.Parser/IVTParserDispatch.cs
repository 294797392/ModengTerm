using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoTerminal.Parser
{
    internal class VTParserDispatch
    {
        protected IVideoTerminal terminal;

        public VTParserDispatch(IVideoTerminal terminal)
        {
            this.terminal = terminal;
        }

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

        public void ActionCSIDispatch(int id, List<byte> parameters)
        {
            CSIActionCodes code = (CSIActionCodes)id;

            switch (code)
            {
                case CSIActionCodes.SGR_SetGraphicsRendition:
                    {
                        try
                        {
                            // Modifies the graphical rendering options applied to the next characters written into the buffer.
                            // Options include colors, invert, underlines, and other "font style" type options.
                            this.DispatchSetGraphicsRendition(parameters);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);       
                        }
                        break;
                    }

                default:
                    throw new NotImplementedException(string.Format("未实现CSIAction, {0}", id));
            }
        }

        #region 实例方法

        private void DispatchSetGraphicsRendition(List<byte> bytes)
        {
            foreach (byte option in bytes)
            {
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
                    case GraphicsOptions.BackgroundExtended:
                        {
                            break;
                        }

                    default:
                        //throw new NotImplementedException(string.Format("未实现的SGRCode = {0}", option));
                        break;
                }
            }
        }

        #endregion
    }
}
