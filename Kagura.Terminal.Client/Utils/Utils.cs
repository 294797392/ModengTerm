using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using XTerminal.Terminals;

namespace XTerminal.Utils
{
    public class Utils
    {
        public static Keys WPFKey2TerminalKey(Key key)
        {
            switch (key)
            {
                case Key.Back:return Keys.Backspace;
                case Key.Tab: return Keys.Tab;

                case Key.LeftCtrl: return Keys.LeftCtrl;
                case Key.RightCtrl: return Keys.RightCtrl;

                case Key.LeftShift: return Keys.LeftShift;
                case Key.RightShift: return Keys.RightShift;

                case Key.Enter: return Keys.Enter;
                case Key.Space: return Keys.Space;

                case Key.Up: return Keys.CusorUp;
                case Key.Down: return Keys.CursorDown;
                case Key.Left: return Keys.CursorLeft;
                case Key.Right: return Keys.CursorRight;

                case Key.A: return Keys.A;
                case Key.B: return Keys.B;
                case Key.C: return Keys.C;
                case Key.D: return Keys.D;
                case Key.E: return Keys.E;
                case Key.F: return Keys.F;
                case Key.G: return Keys.G;
                case Key.H: return Keys.H;
                case Key.I: return Keys.I;
                case Key.J: return Keys.J;
                case Key.K: return Keys.K;
                case Key.L: return Keys.L;
                case Key.M: return Keys.M;
                case Key.N: return Keys.N;
                case Key.O: return Keys.O;
                case Key.P: return Keys.P;
                case Key.Q: return Keys.Q;
                case Key.R: return Keys.R;
                case Key.S: return Keys.S;
                case Key.T: return Keys.T;
                case Key.U: return Keys.U;
                case Key.V: return Keys.V;
                case Key.W: return Keys.W;
                case Key.X: return Keys.X;
                case Key.Y: return Keys.Y;
                case Key.Z: return Keys.Z;

                case Key.D0: return Keys.D0;
                case Key.D1: return Keys.D1;
                case Key.D2: return Keys.D2;
                case Key.D3: return Keys.D3;
                case Key.D4: return Keys.D4;
                case Key.D5: return Keys.D5;
                case Key.D6: return Keys.D6;
                case Key.D7: return Keys.D7;
                case Key.D8: return Keys.D8;
                case Key.D9: return Keys.D9;

                case Key.OemMinus: return Keys.OemMinus;
                case Key.OemPlus: return Keys.OemPlus;
                case Key.OemOpenBrackets: return Keys.OemOpenBrackets;
                case Key.Oem1: return Keys.Oem1;
                case Key.OemQuotes: return Keys.OemQuotes;
                case Key.OemComma: return Keys.OemComma;
                case Key.OemPeriod: return Keys.OemPeriod;
                case Key.OemQuestion: return Keys.OemQuestion;
                case Key.Oem5: return Keys.Oem5;
                case Key.Oem3: return Keys.Oem3;
                case Key.Oem6: return Keys.Oem6;
                default:
                    return Keys.Null;
            }
        }
    }
}