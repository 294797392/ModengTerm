using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoTerminal.Interface;
using VideoTerminal.Parser;

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

        public event Action<IVideoTerminal, VTInputEventArgs> InputEvent;

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

        public void WarningBell()
        {
            Console.Beep();
        }

        //public void PerformAction(List<VTActions> vtActions)
        //{
        //    foreach (VTAction vtAction in vtActions)
        //    {
        //        switch (vtAction.Type)
        //        {
        //            case VTActions.Foreground:
        //                {
        //                    switch ((TextColor)vtAction.Data)
        //                    {
        //                        case TextColor.DARK_BLACK: Console.ForegroundColor = ConsoleColor.Black; break;
        //                        case TextColor.DARK_BLUE: Console.ForegroundColor = ConsoleColor.DarkBlue; break;
        //                        case TextColor.DARK_CYAN: Console.ForegroundColor = ConsoleColor.DarkCyan; break;
        //                        case TextColor.DARK_GREEN: Console.ForegroundColor = ConsoleColor.DarkGreen; break;
        //                        case TextColor.DARK_MAGENTA: Console.ForegroundColor = ConsoleColor.DarkMagenta; break;
        //                        case TextColor.DARK_RED: Console.ForegroundColor = ConsoleColor.DarkRed; break;
        //                        case TextColor.DARK_WHITE: Console.ForegroundColor = ConsoleColor.White; break;
        //                        case TextColor.DARK_YELLOW: Console.ForegroundColor = ConsoleColor.DarkYellow; break;
        //                        case TextColor.BRIGHT_BLACK: Console.ForegroundColor = ConsoleColor.Black; break;
        //                        case TextColor.BRIGHT_BLUE: Console.ForegroundColor = ConsoleColor.Blue; break;
        //                        case TextColor.BRIGHT_CYAN: Console.ForegroundColor = ConsoleColor.Cyan; break;
        //                        case TextColor.BRIGHT_GREEN: Console.ForegroundColor = ConsoleColor.Green; break;
        //                        case TextColor.BRIGHT_MAGENTA: Console.ForegroundColor = ConsoleColor.Magenta; break;
        //                        case TextColor.BRIGHT_RED: Console.ForegroundColor = ConsoleColor.Red; break;
        //                        case TextColor.BRIGHT_WHITE: Console.ForegroundColor = ConsoleColor.White; break;
        //                        case TextColor.BRIGHT_YELLOW: Console.ForegroundColor = ConsoleColor.Yellow; break;
        //                        default:
        //                            break;
        //                    }
        //                    break;
        //                }

        //            case VTActions.Background:
        //                {
        //                    switch ((TextColor)vtAction.Data)
        //                    {
        //                        case TextColor.DARK_BLACK: Console.BackgroundColor = ConsoleColor.Black; break;
        //                        case TextColor.DARK_BLUE: Console.BackgroundColor = ConsoleColor.DarkBlue; break;
        //                        case TextColor.DARK_CYAN: Console.BackgroundColor = ConsoleColor.DarkCyan; break;
        //                        case TextColor.DARK_GREEN: Console.BackgroundColor = ConsoleColor.DarkGreen; break;
        //                        case TextColor.DARK_MAGENTA: Console.BackgroundColor = ConsoleColor.DarkMagenta; break;
        //                        case TextColor.DARK_RED: Console.BackgroundColor = ConsoleColor.DarkRed; break;
        //                        case TextColor.DARK_WHITE: Console.BackgroundColor = ConsoleColor.White; break;
        //                        case TextColor.DARK_YELLOW: Console.BackgroundColor = ConsoleColor.DarkYellow; break;
        //                        case TextColor.BRIGHT_BLACK: Console.BackgroundColor = ConsoleColor.Black; break;
        //                        case TextColor.BRIGHT_BLUE: Console.BackgroundColor = ConsoleColor.Blue; break;
        //                        case TextColor.BRIGHT_CYAN: Console.BackgroundColor = ConsoleColor.Cyan; break;
        //                        case TextColor.BRIGHT_GREEN: Console.BackgroundColor = ConsoleColor.Green; break;
        //                        case TextColor.BRIGHT_MAGENTA: Console.BackgroundColor = ConsoleColor.Magenta; break;
        //                        case TextColor.BRIGHT_RED: Console.BackgroundColor = ConsoleColor.Red; break;
        //                        case TextColor.BRIGHT_WHITE: Console.BackgroundColor = ConsoleColor.White; break;
        //                        case TextColor.BRIGHT_YELLOW: Console.BackgroundColor = ConsoleColor.Yellow; break;
        //                        default:
        //                            break;
        //                    }
        //                    break;
        //                }

        //            case VTActions.ForegroundRGB:
        //                {
        //                    break;
        //                }

        //            case VTActions.BackgroundRGB:
        //                {
        //                    break;
        //                }

        //            case VTActions.Blink:
        //                {
        //                    Console.CursorVisible = true;
        //                    break;
        //                }

        //            case VTActions.BlinkUnset:
        //                {
        //                    Console.CursorVisible = false;
        //                    break;
        //                }

        //            case VTActions.Bold:
        //                {
        //                    break;
        //                }

        //            case VTActions.BoldUnset:
        //                {
        //                    break;
        //                }

        //            case VTActions.CrossedOut:
        //                {
        //                    break;
        //                }

        //            case VTActions.CrossedOutUnset:
        //                {
        //                    break;
        //                }

        //            case VTActions.DefaultAttributes:
        //                {
        //                    break;
        //                }

        //            case VTActions.DefaultBackground:
        //                {
        //                    Console.BackgroundColor = this.defaultBackground;
        //                    break;
        //                }

        //            case VTActions.DefaultForeground:
        //                {
        //                    Console.ForegroundColor = this.defaultForeground;
        //                    break;
        //                }

        //            case VTActions.DoublyUnderlined:
        //                {
        //                    break;
        //                }

        //            case VTActions.DoublyUnderlinedUnset:
        //                {
        //                    break;
        //                }

        //            case VTActions.Faint:
        //                {
        //                    break;
        //                }

        //            case VTActions.FaintUnset:
        //                {
        //                    break;
        //                }

        //            case VTActions.Invisible:
        //                {
        //                    break;
        //                }

        //            case VTActions.InvisibleUnset:
        //                {
        //                    break;
        //                }

        //            case VTActions.Italics:
        //                {
        //                    break;
        //                }

        //            case VTActions.ItalicsUnset:
        //                {
        //                    break;
        //                }

        //            case VTActions.Overlined:
        //                {
        //                    break;
        //                }

        //            case VTActions.OverlinedUnset:
        //                {
        //                    break;
        //                }

        //            case VTActions.ReverseVideo:
        //                {
        //                    this.originalBackground = Console.BackgroundColor;
        //                    this.originalForeground = Console.ForegroundColor;

        //                    Console.BackgroundColor = this.originalForeground;
        //                    Console.ForegroundColor = this.originalBackground;
        //                    break;
        //                }

        //            case VTActions.ReverseVideoUnset:
        //                {
        //                    Console.BackgroundColor = this.originalBackground;
        //                    Console.ForegroundColor = this.originalForeground;
        //                    break;
        //                }

        //            case VTActions.Underline:
        //                {
        //                    break;
        //                }

        //            case VTActions.UnderlineUnset:
        //                {
        //                    break;
        //                }

        //            default:
        //                break;
        //        }
        //    }
        //}

        public void PerformAction(VTActions vtAction, params object[] param)
        {

        }
    }
}
