using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoTerminal.Parser;

namespace VideoTerminalConsole
{
    public class ConsoleVT : IVideoTerminal
    {
        public ConsoleVT()
        {
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
            throw new NotImplementedException();
        }

        public void SetCrossedOut(bool crossedOut)
        {
            throw new NotImplementedException();
        }

        public void SetDefaultAttributes()
        {
            throw new NotImplementedException();
        }

        public void SetDefaultBackground()
        {
            Console.ResetColor();
        }

        public void SetDefaultForeground()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void SetIndexedForeground(TextColor color)
        {
            throw new NotImplementedException();
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
