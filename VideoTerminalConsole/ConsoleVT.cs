using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoTerminal.Parser;

namespace VideoTerminalConsole
{
    public class ConsoleVT : IVideoTerminal
    {
        private ConsoleColor defaultForeground;
        private ConsoleColor defaultBackground;

        public ConsoleVT()
        {
            this.defaultForeground = Console.ForegroundColor;
            this.defaultBackground = Console.BackgroundColor;
        }

        public void CarriageReturn()
        {
            Console.Write('\r');
        }

        public void CursorBackward(int distance)
        {
            throw new NotImplementedException();
        }

        public void ForwardTab()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void SetBold(bool bold)
        {
        }

        public void SetCrossedOut(bool crossedOut)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void SetFaint(bool faint)
        {
            throw new NotImplementedException();
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
                default:
                    throw new NotImplementedException();
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
                default:
                    throw new NotImplementedException();
            }
        }

        public void SetInvisible(bool invisible)
        {
            throw new NotImplementedException();
        }

        public void SetItalics(bool italics)
        {
            throw new NotImplementedException();
        }

        public void SetOverlined(bool overline)
        {
            throw new NotImplementedException();
        }

        public void SetReverseVideo(bool reverse)
        {
            throw new NotImplementedException();
        }

        public void SetUnderline(bool underline)
        {
            throw new NotImplementedException();
        }

        public void WarningBell()
        {
            Console.Beep();
        }
    }
}
