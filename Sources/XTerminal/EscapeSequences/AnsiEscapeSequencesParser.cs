using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using XTerminal.Terminal.EscapeSequences;
using XTerminal.Commands;

namespace XTerminal.Terminal
{
    /// <summary>
    /// ANSI终端转义序列解析器，又称VT100终端转义序列
    /// 使用ASCII码定义了终端显示的方式，比如哪些字符要显示什么颜色等等
    /// 所有的VT100控制符以\033（即ESC的ASCII码）打头
    /// 格式具体有两种：
    /// 1.\033[<数字>m 如\33[40m表示让后面字符输出用黑色背景输出, \33[0m表示取消前面的设置
    /// 2.控制字符形式，即最后一个字符不是m，而是控制字符。\033[k清除从光标到行尾的内容，\033[nC光标右移n行
    /// 
    /// 在linux环境下，使用man console_code查看详细指令集合
    /// </summary>
    public class AnsiEscapeSequencesParser : IEscapeSequencesParser
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("AnsiEscapeSequencesParser");

        private static char[] CSISplitter = { (char)27 };

        private static Dictionary<int, TextDecorationEnum> TextDecorationsMap = new Dictionary<int, TextDecorationEnum>()
        {
            { 0, XTerminalCore.Invocations.SGRInvocation.TextDecorationEnum.ResetAllAttributes }, { 1, TextDecorationEnum.Bright }, { 2,  TextDecorationEnum.Dim},
            { 4, TextDecorationEnum.Underscore}, { 5,TextDecorationEnum.Blink }, {7,TextDecorationEnum.Reverse },{ 8,TextDecorationEnum.Hidden}
        };
        private static Dictionary<int, string> ForegroundColorsMap = new Dictionary<int, string>()
        {
            {30, "Black" }, { 31,"Red"}, {32 ,"Green"}, {33,"Yellow" }, { 34,"Blue"}, { 35,"Magenta"},{ 36,"Cyan"}, { 37,"White"}
        };
        private static Dictionary<int, string> BackgroundColorsMap = new Dictionary<int, string>()
        {
            {40, "Black" }, { 41,"Red"}, {42 ,"Green"}, {43,"Yellow" }, { 44,"Blue"}, { 45,"Magenta"},{ 46,"Cyan"}, { 47,"White"}
        };

        /// <summary>
        /// 解析有颜色的文本
        /// </summary>
        /// <param name="chars"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private IEscapeSequencesCommand ParseColorizedTextCommand(string attrTxt, string text)
        {
            ColorizedTextCommand textCmd = new ColorizedTextCommand(text);
            string[] attrs = attrTxt.Trim('[', 'm').Split(';');
            foreach (string attr in attrs)
            {
                int iattr = int.Parse(attr);
                if (iattr >= 0 && iattr <= 8)
                {
                    textCmd.Decorations.Add(TextDecorationsMap[iattr]);
                }
                if (iattr >= 30 && iattr <= 37)
                {
                    textCmd.Foreground = ForegroundColorsMap[iattr];
                }
                if (iattr >= 40 && iattr <= 47)
                {
                    textCmd.Background = BackgroundColorsMap[iattr];
                }
            }
            return textCmd;
        }

        /// <summary>
        /// 解析操作光标命令
        /// </summary>
        /// <param name="chars"></param>
        /// <returns></returns>
        private IEscapeSequencesCommand ParseCursorActionCommand(string attrTxt, CursorDirectionEnum direction)
        {
            CursorActionCommand actionCmd = new CursorActionCommand();
            actionCmd.Direction = direction;
            if (direction == CursorDirectionEnum.Auto)
            {
                if (attrTxt.Length == 0)
                {
                    actionCmd.X = 0;
                    actionCmd.Y = 0;
                }
                else
                {
                    string[] xy = attrTxt.Trim('[').Split(';');
                    if (xy.Count() == 0)
                    {
                        actionCmd.X = 0;
                        actionCmd.Y = 0;
                    }
                    else
                    {
                        actionCmd.X = int.Parse(xy[1]);
                        actionCmd.Y = int.Parse(xy[0]);
                    }
                }
            }
            else
            {
                actionCmd.MoveLength = int.Parse(attrTxt);
            }
            return actionCmd;
        }

        /// <summary>
        /// Enable scrolling from row {start} to row {end}.
        /// </summary>
        /// <param name="chars"></param>
        /// <returns></returns>
        private IEscapeSequencesCommand ParseScrollActionCommand(string attr)
        {
            ScrollActionCommand scrollCmd = new ScrollActionCommand();
            string[] xy = attr.Trim('[').Split(';');
            scrollCmd.StartRow = int.Parse(xy[0]);
            scrollCmd.EndRow = int.Parse(xy[1]);
            return scrollCmd;
        }

        public List<IEscapeSequencesCommand> Parse(byte[] chars)
        {
            List<IEscapeSequencesCommand> commands = new List<IEscapeSequencesCommand>();
            if (chars[0] == 27)
            {
                string text = Encoding.ASCII.GetString(chars);
                string[] csis = text.Split(CSISplitter, StringSplitOptions.RemoveEmptyEntries);
                foreach (string csi in csis)
                {
                    commands.Add(this.ProcessCSI(csi));
                }
            }
            else
            {
                if (chars.Length == 1)
                {
                    commands.Add(new NormalTextCommand(Encoding.ASCII.GetString(chars)));
                }
                else
                {
                    List<byte> cs = new List<byte>();
                    foreach (byte c in chars)
                    {
                        if (c == '\r')
                        {
                            // 回车
                            //commands.Add(new NormalTextCommand(Encoding.ASCII.GetString(cs.ToArray())));
                            //commands.Add(PredefineCommands.Enter);
                            //cs.Clear();
                        }
                        else if (c == '\n')
                        {
                            // 换行
                            //commands.Add(new NormalTextCommand(Encoding.ASCII.GetString(cs.ToArray())));
                            //commands.Add(PredefineCommands.NewLine);
                            //cs.Clear();
                        }
                        else
                        {
                            cs.Add(c);
                        }
                    }
                }
            }

            return commands;
        }

        private IEscapeSequencesCommand ProcessCSI(string csiText)
        {
            Match match = null;

            match = Regex.Match(csiText, @"\[.+m");
            if (match.Success)
            {
                string text = csiText.Substring(match.Length, csiText.Length - match.Length);
                return this.ParseColorizedTextCommand(match.Value, text);
            }
            if ((match = Regex.Match(csiText, @"\[\d+A")).Success)
            {
                return this.ParseCursorActionCommand(match.Value, CursorDirectionEnum.Top);
            }
            if ((match = Regex.Match(csiText, @"\[\d+B")).Success)
            {
                return this.ParseCursorActionCommand(match.Value, CursorDirectionEnum.Bottom);
            }
            if ((match = Regex.Match(csiText, @"\[\d+C")).Success)
            {
                return this.ParseCursorActionCommand(match.Value, CursorDirectionEnum.Right);
            }
            if ((match = Regex.Match(csiText, @"\[\d+D")).Success)
            {
                return this.ParseCursorActionCommand(match.Value, CursorDirectionEnum.Left);
            }
            if (csiText[0] == 'E')
            {
                return PredefineCommands.NewLine;
            }
            if ((match = Regex.Match(csiText, @"\[.+H")).Success)
            {
                return this.ParseCursorActionCommand(match.Value, CursorDirectionEnum.Auto);
            }
            if ((match = Regex.Match(csiText, @"\[J")).Success)
            {
                return PredefineCommands.EraseDown;
            }
            if ((match = Regex.Match(csiText, @"\[K")).Success)
            {
                return PredefineCommands.EraseEndOfLine;
            }
            if ((match = Regex.Match(csiText, @"\[1K")).Success)
            {
                return PredefineCommands.EraseStartOfLine;
            }
            if ((match = Regex.Match(csiText, @"\[2K")).Success)
            {
                return PredefineCommands.EraseLine;
            }
            if ((match = Regex.Match(csiText, @"\[1J")).Success)
            {
                return PredefineCommands.EraseUp;
            }
            if ((match = Regex.Match(csiText, @"\[2J")).Success)
            {
                return PredefineCommands.EraseScreen;
            }
            if ((match = Regex.Match(csiText, @"\?25l")).Success)
            {
                return PredefineCommands.HiddenCursor;
            }
            if ((match = Regex.Match(csiText, @"\?25h")).Success)
            {
                return PredefineCommands.VisibleCursor;
            }
            if (csiText[0] == 'H')
            {
                return PredefineCommands.SetTab;
            }
            if ((match = Regex.Match(csiText, @"\[g")).Success)
            {
                return PredefineCommands.ClearTab;
            }
            if ((match = Regex.Match(csiText, @"\[3g")).Success)
            {
                return PredefineCommands.ClearAllTabs;
            }
            if ((match = Regex.Match(csiText, @"\[r")).Success)
            {
                return PredefineCommands.ScrollScreen;
            }
            if (csiText[0] == 'D')
            {
                return PredefineCommands.ScrollDown;
            }
            if (csiText[0] == 'M')
            {
                return PredefineCommands.ScrollUp;
            }
            if (csiText[0] == 'M')
            {
                return PredefineCommands.ScrollUp;
            }
            if ((match = Regex.Match(csiText, @"\[.+r")).Success)
            {
                return this.ParseScrollActionCommand(match.Value);
            }
            if ((match = Regex.Match(csiText, @"\[7h")).Success)
            {
                return PredefineCommands.EnableLineWrap;
            }
            if ((match = Regex.Match(csiText, @"\[71")).Success)
            {
                return PredefineCommands.DisableLineWrap;
            }
            if (csiText[0] == '(')
            {
                return PredefineCommands.SetDefaultFont;
            }
            if (csiText[0] == ')')
            {
                return PredefineCommands.SetAlternateFont;
            }
            if ((match = Regex.Match(csiText, @"\]0")).Success || (match = Regex.Match(csiText, @"\]1")).Success || (match = Regex.Match(csiText, @"\]2")).Success || (match = Regex.Match(csiText, @"\]4")).Success || (match = Regex.Match(csiText, @"\]10")).Success)
            {
                string text = csiText.Substring(match.Length, csiText.Length - match.Length);
                return new NormalTextCommand(text);
            }

            logger.ErrorFormat("不识别的命令:{0}", csiText);
            return null;
        }
    }
}