using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoTerminal.Base;
using VideoTerminal.Parser;
using VTInterface;

namespace VideoTerminalConsole
{
    public class ConsolePresentationDevice : IPresentationDevice
    {
        /// <summary>
        /// 屏幕左上角的X坐标
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// 屏幕右上角的X坐标
        /// </summary>
        public int Y { get; set; }
    }

    public class ConsoleVT : IVideoTerminal, ICursorState
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("ConsoleVT");

        #endregion

        #region 实例变量

        private ConsoleColor defaultForeground;
        private ConsoleColor defaultBackground;

        private bool reversed;
        private ConsoleColor originalForeground;
        private ConsoleColor originalBackground;

        private ConsolePresentationDevice mainDevice;
        private IPresentationDevice activeDevice;

        #endregion

        #region 构造方法

        public ConsoleVT()
        {
            this.defaultForeground = Console.ForegroundColor;
            this.defaultBackground = Console.BackgroundColor;

            Console.TreatControlCAsInput = true;

            this.mainDevice = new ConsolePresentationDevice();
            this.activeDevice = this.mainDevice;

            Console.WindowHeight = 25;
            Console.BufferHeight = 25;
            Console.WindowWidth = 80;
            Console.BufferWidth = 80;
        }

        #endregion

        public void CarriageReturn()
        {
            Console.Write('\r');
        }

        public void CursorBackward(int distance)
        {
            Console.CursorLeft -= distance;
        }

        public void CursorForward(int distance)
        {
            Console.CursorLeft += distance;
        }

        #region CursorState

        public void CursorPosition(int row, int column)
        {
            Console.SetCursorPosition(column, row);
        }

        public void CursorRestoreState(ICursorState state)
        {
        }

        public ICursorState CursorSaveState()
        {
            return this;
        }

        public bool CursorVisibility(bool visible)
        {
            Console.CursorVisible = visible;
            return true;
        }

        #endregion

        #region IPresentationDevice

        public void DeletePresentationDevice(IPresentationDevice device)
        {
        }

        public IPresentationDevice GetActivePresentationDevice()
        {
            return this.activeDevice;
        }

        public IPresentationDevice CreatePresentationDevice()
        {
            return new ConsolePresentationDevice();
        }

        public bool SwitchPresentationDevice(IPresentationDevice activeDevice)
        {
            Console.Clear();
            return true;
        }

        #endregion

        public void ForwardTab()
        {
            logger.WarnFormat("未实现ForwardTab");
        }

        public void LineFeed()
        {
            Console.Write('\n');
        }

        public void Print(char c)
        {
            Console.Write(c);
        }

        public void Print(string text)
        {
            Console.Write(text);
        }

        public void SetBlinking(bool blinking)
        {
            Console.CursorVisible = blinking;
        }

        public void SetBold(bool bold)
        {
        }

        public void SetCrossedOut(bool crossedOut)
        {
        }

        public void SetDefaultAttributes()
        {
        }

        public void SetDefaultBackground()
        {
            Console.BackgroundColor = this.defaultBackground;
        }

        public void SetDefaultForeground()
        {
            Console.ForegroundColor = this.defaultForeground;
        }

        public void SetDoublyUnderlined(bool underline)
        {
            logger.WarnFormat("未实现SetDoublyUnderlined");
        }

        public void SetFaint(bool faint)
        {
        }

        public void SetIndexedBackground(TextColor color)
        {
            switch (color)
            {
                case TextColor.DARK_BLACK: Console.BackgroundColor = ConsoleColor.Black; break;
                case TextColor.DARK_BLUE: Console.BackgroundColor = ConsoleColor.DarkBlue; break;
                case TextColor.DARK_CYAN: Console.BackgroundColor = ConsoleColor.DarkCyan; break;
                case TextColor.DARK_GREEN: Console.BackgroundColor = ConsoleColor.DarkGreen; break;
                case TextColor.DARK_MAGENTA: Console.BackgroundColor = ConsoleColor.DarkMagenta; break;
                case TextColor.DARK_RED: Console.BackgroundColor = ConsoleColor.DarkRed; break;
                case TextColor.DARK_WHITE: Console.BackgroundColor = ConsoleColor.White; break;
                case TextColor.DARK_YELLOW: Console.BackgroundColor = ConsoleColor.DarkYellow; break;
                case TextColor.BRIGHT_BLACK: Console.BackgroundColor = ConsoleColor.Black; break;
                case TextColor.BRIGHT_BLUE: Console.BackgroundColor = ConsoleColor.Blue; break;
                case TextColor.BRIGHT_CYAN: Console.BackgroundColor = ConsoleColor.Cyan; break;
                case TextColor.BRIGHT_GREEN: Console.BackgroundColor = ConsoleColor.Green; break;
                case TextColor.BRIGHT_MAGENTA: Console.BackgroundColor = ConsoleColor.Magenta; break;
                case TextColor.BRIGHT_RED: Console.BackgroundColor = ConsoleColor.Red; break;
                case TextColor.BRIGHT_WHITE: Console.BackgroundColor = ConsoleColor.White; break;
                case TextColor.BRIGHT_YELLOW: Console.BackgroundColor = ConsoleColor.Yellow; break;
                default:
                    logger.WarnFormat("未实现IndexedBackground, {0}", color);
                    break;
            }
        }

        public void SetIndexedForeground(TextColor color)
        {
            switch (color)
            {
                case TextColor.DARK_BLACK: Console.ForegroundColor = ConsoleColor.Black; break;
                case TextColor.DARK_BLUE: Console.ForegroundColor = ConsoleColor.DarkBlue; break;
                case TextColor.DARK_CYAN: Console.ForegroundColor = ConsoleColor.DarkCyan; break;
                case TextColor.DARK_GREEN: Console.ForegroundColor = ConsoleColor.DarkGreen; break;
                case TextColor.DARK_MAGENTA: Console.ForegroundColor = ConsoleColor.DarkMagenta; break;
                case TextColor.DARK_RED: Console.ForegroundColor = ConsoleColor.DarkRed; break;
                case TextColor.DARK_WHITE: Console.ForegroundColor = ConsoleColor.White; break;
                case TextColor.DARK_YELLOW: Console.ForegroundColor = ConsoleColor.DarkYellow; break;
                case TextColor.BRIGHT_BLACK: Console.ForegroundColor = ConsoleColor.Black; break;
                case TextColor.BRIGHT_BLUE: Console.ForegroundColor = ConsoleColor.Blue; break;
                case TextColor.BRIGHT_CYAN: Console.ForegroundColor = ConsoleColor.Cyan; break;
                case TextColor.BRIGHT_GREEN: Console.ForegroundColor = ConsoleColor.Green; break;
                case TextColor.BRIGHT_MAGENTA: Console.ForegroundColor = ConsoleColor.Magenta; break;
                case TextColor.BRIGHT_RED: Console.ForegroundColor = ConsoleColor.Red; break;
                case TextColor.BRIGHT_WHITE: Console.ForegroundColor = ConsoleColor.White; break;
                case TextColor.BRIGHT_YELLOW: Console.ForegroundColor = ConsoleColor.Yellow; break;
                default:
                    logger.WarnFormat("未实现IndexedForeground, {0}", color);
                    break;
            }
        }

        public void SetInvisible(bool invisible)
        {
            logger.WarnFormat("未实现SetInvisible");
        }

        public void SetItalics(bool italics)
        {
        }

        public void SetOverlined(bool overline)
        {
            logger.WarnFormat("未实现SetOverlined");
        }

        public void SetReverseVideo(bool reverse)
        {
            if (reverse && !this.reversed)
            {
                this.reversed = true;

                this.originalBackground = Console.BackgroundColor;
                this.originalForeground = Console.ForegroundColor;

                Console.BackgroundColor = this.originalForeground;
                Console.ForegroundColor = this.originalBackground;
            }
            else
            {
                this.reversed = false;

                Console.BackgroundColor = this.originalBackground;
                Console.ForegroundColor = this.originalForeground;
            }
        }

        public void SetUnderline(bool underline)
        {
        }

        public void WarningBell()
        {
            Console.Beep();
        }

        public void SetForeground(byte r, byte g, byte b)
        {
            this.SetIndexedForeground(TextColor.DARK_YELLOW);
        }

        public void SetBackground(byte r, byte g, byte b)
        {
            this.SetIndexedBackground(TextColor.DARK_MAGENTA);
        }
    }
}
