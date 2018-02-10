using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XTerminal.Terminal.AnsiEscapeSequencesCommands;
using XTerminal.Base;

namespace XTerminal.Terminal
{
    /// <summary>
    /// ANSI终端转义序列，又称VT100终端转义序列
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
        private ParserStatusEnum status = ParserStatusEnum.Normal;

        private static Dictionary<int, TextDecorationEnum> TextDecorationsMap = new Dictionary<int, TextDecorationEnum>()
        {
            { 0, TextDecorationEnum.ResetAllAttributes }, { 1, TextDecorationEnum.Bright }, { 2,  TextDecorationEnum.Dim},
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
        private IEscapeSequencesCommand ParseColorizedTextCommand(List<byte> chars, List<byte> content)
        {
            ColorizedTextCommand textCmd = new ColorizedTextCommand();
            textCmd.Text = Encoding.ASCII.GetString(content.ToArray());
            string text = Encoding.ASCII.GetString(chars.ToArray());
            string[] attrs = text.Split(';');
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
        private IEscapeSequencesCommand ParseCursorActionCommand(List<byte> chars, CursorDirectionEnum direction)
        {
            CursorActionCommand actionCmd = new CursorActionCommand();
            actionCmd.Direction = direction;
            if (direction == CursorDirectionEnum.Auto)
            {
                if (chars.Count == 0)
                {
                    actionCmd.X = 0;
                    actionCmd.Y = 0;
                }
                else
                {
                    string text = Encoding.ASCII.GetString(chars.ToArray());
                    string[] xy = text.Split(';');
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
                string text = Encoding.ASCII.GetString(chars.ToArray());
                actionCmd.MoveLength = int.Parse(text);
            }
            return actionCmd;
        }

        /// <summary>
        /// Enable scrolling from row {start} to row {end}.
        /// </summary>
        /// <param name="chars"></param>
        /// <returns></returns>
        private IEscapeSequencesCommand ParseScrollActionCommand(List<byte> chars)
        {
            ScrollActionCommand scrollCmd = new ScrollActionCommand();
            string text = Encoding.ASCII.GetString(chars.ToArray());
            string[] xy = text.Split(';');
            scrollCmd.StartRow = int.Parse(xy[0]);
            scrollCmd.EndRow = int.Parse(xy[1]);
            return scrollCmd;
        }

        public List<IEscapeSequencesCommand> Parse(byte[] chars)
        {
            List<IEscapeSequencesCommand> commands = new List<IEscapeSequencesCommand>();
            List<byte> attr = new List<byte>();
            List<byte> content = new List<byte>();

            IEnumerator enumtor = chars.GetEnumerator();
            while (enumtor.MoveNext())
            {
                byte c = (byte)enumtor.Current;
                switch (this.status)
                {
                    case ParserStatusEnum.Normal:
                        if (c == 27) //esc
                        {
                            attr.Clear();
                            content.Clear();
                            this.status = ParserStatusEnum.Csi;
                            continue;
                        }
                        else
                        {
                            content.Add(c);
                        }
                        break;

                    case ParserStatusEnum.Csi:
                        if (c == '[')
                        {
                            attr.Add(c);
                            continue;
                        }
                        if (c == 'm')
                        {
                            this.status = ParserStatusEnum.Normal;
                            attr.RemoveAt(0);
                            commands.Add(this.ParseColorizedTextCommand(attr, content));
                            continue;
                        }
                        if (c == 'A')
                        {
                            this.status = ParserStatusEnum.Normal;
                            attr.RemoveAt(0);
                            commands.Add(this.ParseCursorActionCommand(attr, CursorDirectionEnum.Top));
                            continue;
                        }
                        if (c == 'B')
                        {
                            this.status = ParserStatusEnum.Normal;
                            attr.RemoveAt(0);
                            commands.Add(this.ParseCursorActionCommand(attr, CursorDirectionEnum.Bottom));
                            continue;
                        }
                        if (c == 'C')
                        {
                            this.status = ParserStatusEnum.Normal;
                            attr.RemoveAt(0);
                            commands.Add(this.ParseCursorActionCommand(attr, CursorDirectionEnum.Right));
                            continue;
                        }
                        if (c == 'D')
                        {
                            this.status = ParserStatusEnum.Normal;
                            attr.RemoveAt(0);
                            commands.Add(this.ParseCursorActionCommand(attr, CursorDirectionEnum.Left));
                            continue;
                        }
                        if (c == 'E')
                        {
                            this.status = ParserStatusEnum.Normal;
                            commands.Add(PredefineCommands.NewLine);
                            continue;
                        }
                        if (attr.EqualsEx("H"))
                        {
                            this.status = ParserStatusEnum.Normal;
                            attr.RemoveAt(0);
                            commands.Add(this.ParseCursorActionCommand(attr, CursorDirectionEnum.Auto));
                            continue;
                        }
                        if (attr.EqualsEx("[J"))
                        {
                            this.status = ParserStatusEnum.Normal;
                            commands.Add(PredefineCommands.EraseDown);
                            continue;
                        }
                        if (attr.EqualsEx("[K"))
                        {
                            this.status = ParserStatusEnum.Normal;
                            commands.Add(PredefineCommands.EraseEndOfLine);
                            continue;
                        }
                        if (attr.EqualsEx("1K"))
                        {
                            this.status = ParserStatusEnum.Normal;
                            commands.Add(PredefineCommands.EraseStartOfLine);
                            continue;
                        }
                        if (attr.EqualsEx("2K"))
                        {
                            this.status = ParserStatusEnum.Normal;
                            commands.Add(PredefineCommands.EraseLine);
                            continue;
                        }
                        if (attr.EqualsEx("1J"))
                        {
                            this.status = ParserStatusEnum.Normal;
                            commands.Add(PredefineCommands.EraseUp);
                            continue;
                        }
                        if (attr.EqualsEx("[2J"))
                        {
                            this.status = ParserStatusEnum.Normal;
                            commands.Add(PredefineCommands.EraseScreen);
                            continue;
                        }
                        if (attr.EqualsEx("?25l"))
                        {
                            this.status = ParserStatusEnum.Normal;
                            commands.Add(PredefineCommands.HiddenCursor);
                            continue;
                        }
                        if (attr.EqualsEx("?25h"))
                        {
                            this.status = ParserStatusEnum.Normal;
                            commands.Add(PredefineCommands.VisibleCursor);
                            continue;
                        }
                        if (attr.EqualsEx("[H"))
                        {
                            this.status = ParserStatusEnum.Normal;
                            commands.Add(PredefineCommands.SetTab);
                            continue;
                        }
                        if (attr.EqualsEx("[g"))
                        {
                            this.status = ParserStatusEnum.Normal;
                            commands.Add(PredefineCommands.ClearTab);
                            continue;
                        }
                        if (attr.EqualsEx("3g"))
                        {
                            this.status = ParserStatusEnum.Normal;
                            commands.Add(PredefineCommands.ClearAllTabs);
                            continue;
                        }
                        if (attr.EqualsEx("[r"))
                        {
                            this.status = ParserStatusEnum.Normal;
                            commands.Add(PredefineCommands.ScrollScreen);
                            continue;
                        }
                        if (attr.EqualsEx("D"))
                        {
                            this.status = ParserStatusEnum.Normal;
                            commands.Add(PredefineCommands.ScrollDown);
                            continue;
                        }
                        if (attr.EqualsEx("M"))
                        {
                            this.status = ParserStatusEnum.Normal;
                            commands.Add(PredefineCommands.ScrollUp);
                            continue;
                        }
                        if (attr.EqualsEx("r"))
                        {
                            this.status = ParserStatusEnum.Normal;
                            attr.RemoveAt(0);
                            commands.Add(this.ParseScrollActionCommand(attr));
                            continue;
                        }
                        if (attr.EqualsEx("[7h"))
                        {
                            this.status = ParserStatusEnum.Normal;
                            commands.Add(PredefineCommands.EnableLineWrap);
                            continue;
                        }
                        if (attr.EqualsEx("[7l"))
                        {
                            this.status = ParserStatusEnum.Normal;
                            commands.Add(PredefineCommands.DisableLineWrap);
                            continue;
                        }
                        if (c == '(')
                        {
                            this.status = ParserStatusEnum.Normal;
                            commands.Add(PredefineCommands.SetDefaultFont);
                            continue;
                        }
                        if (c == ')')
                        {
                            this.status = ParserStatusEnum.Normal;
                            commands.Add(PredefineCommands.SetAlternateFont);
                            continue;
                        }
                        if (attr.EqualsEx("]0") || attr.EqualsEx("]1") || attr.EqualsEx("]2") || attr.EqualsEx("]4") || attr.EqualsEx("]10"))
                        {
                            this.status = ParserStatusEnum.Normal;
                            continue;
                        }
                        attr.Add(c);
                        break;
                }

            }

            #region ...

            //foreach (byte c in chars)
            //{
            //    switch (this.status)
            //    {
            //        case ParserStatusEnum.Normal:
            //            content.Add(c);
            //            if (c == 27) //esc
            //            {
            //                attr.Clear();
            //                content.Clear();
            //                this.status = ParserStatusEnum.Csi;
            //                continue;
            //            }
            //            break;

            //        case ParserStatusEnum.Csi:
            //            if (c == '[')
            //            {
            //                attr.Add(c);
            //                continue;
            //            }
            //            if (c == 'm')
            //            {
            //                this.status = ParserStatusEnum.Normal;
            //                attr.RemoveAt(0);
            //                commands.Add(this.ParseColorizedTextCommand(attr, content));
            //                continue;
            //            }
            //            if (c == 'A')
            //            {
            //                this.status = ParserStatusEnum.Normal;
            //                attr.RemoveAt(0);
            //                commands.Add(this.ParseCursorActionCommand(attr, CursorDirectionEnum.Top));
            //                continue;
            //            }
            //            if (c == 'B')
            //            {
            //                this.status = ParserStatusEnum.Normal;
            //                attr.RemoveAt(0);
            //                commands.Add(this.ParseCursorActionCommand(attr, CursorDirectionEnum.Bottom));
            //                continue;
            //            }
            //            if (c == 'C')
            //            {
            //                this.status = ParserStatusEnum.Normal;
            //                attr.RemoveAt(0);
            //                commands.Add(this.ParseCursorActionCommand(attr, CursorDirectionEnum.Right));
            //                continue;
            //            }
            //            if (c == 'D')
            //            {
            //                this.status = ParserStatusEnum.Normal;
            //                attr.RemoveAt(0);
            //                commands.Add(this.ParseCursorActionCommand(attr, CursorDirectionEnum.Left));
            //                continue;
            //            }
            //            if (c == 'E')
            //            {
            //                this.status = ParserStatusEnum.Normal;
            //                commands.Add(PredefineCommands.NewLine);
            //                continue;
            //            }
            //            if (attr.EqualsEx("H"))
            //            {
            //                this.status = ParserStatusEnum.Normal;
            //                attr.RemoveAt(0);
            //                commands.Add(this.ParseCursorActionCommand(attr, CursorDirectionEnum.Auto));
            //                continue;
            //            }
            //            if (attr.EqualsEx("[J"))
            //            {
            //                this.status = ParserStatusEnum.Normal;
            //                commands.Add(PredefineCommands.EraseDown);
            //                continue;
            //            }
            //            if (attr.EqualsEx("[K"))
            //            {
            //                this.status = ParserStatusEnum.Normal;
            //                commands.Add(PredefineCommands.EraseEndOfLine);
            //                continue;
            //            }
            //            if (attr.EqualsEx("1K"))
            //            {
            //                this.status = ParserStatusEnum.Normal;
            //                commands.Add(PredefineCommands.EraseStartOfLine);
            //                continue;
            //            }
            //            if (attr.EqualsEx("2K"))
            //            {
            //                this.status = ParserStatusEnum.Normal;
            //                commands.Add(PredefineCommands.EraseLine);
            //                continue;
            //            }
            //            if (attr.EqualsEx("1J"))
            //            {
            //                this.status = ParserStatusEnum.Normal;
            //                commands.Add(PredefineCommands.EraseUp);
            //                continue;
            //            }
            //            if (attr.EqualsEx("[2J"))
            //            {
            //                this.status = ParserStatusEnum.Normal;
            //                commands.Add(PredefineCommands.EraseScreen);
            //                continue;
            //            }
            //            if (attr.EqualsEx("?25l"))
            //            {
            //                this.status = ParserStatusEnum.Normal;
            //                commands.Add(PredefineCommands.HiddenCursor);
            //                continue;
            //            }
            //            if (attr.EqualsEx("?25h"))
            //            {
            //                this.status = ParserStatusEnum.Normal;
            //                commands.Add(PredefineCommands.VisibleCursor);
            //                continue;
            //            }
            //            if (attr.EqualsEx("[H"))
            //            {
            //                this.status = ParserStatusEnum.Normal;
            //                commands.Add(PredefineCommands.SetTab);
            //                continue;
            //            }
            //            if (attr.EqualsEx("[g"))
            //            {
            //                this.status = ParserStatusEnum.Normal;
            //                commands.Add(PredefineCommands.ClearTab);
            //                continue;
            //            }
            //            if (attr.EqualsEx("3g"))
            //            {
            //                this.status = ParserStatusEnum.Normal;
            //                commands.Add(PredefineCommands.ClearAllTabs);
            //                continue;
            //            }
            //            if (attr.EqualsEx("[r"))
            //            {
            //                this.status = ParserStatusEnum.Normal;
            //                commands.Add(PredefineCommands.ScrollScreen);
            //                continue;
            //            }
            //            if (attr.EqualsEx("D"))
            //            {
            //                this.status = ParserStatusEnum.Normal;
            //                commands.Add(PredefineCommands.ScrollDown);
            //                continue;
            //            }
            //            if (attr.EqualsEx("M"))
            //            {
            //                this.status = ParserStatusEnum.Normal;
            //                commands.Add(PredefineCommands.ScrollUp);
            //                continue;
            //            }
            //            if (attr.EqualsEx("r"))
            //            {
            //                this.status = ParserStatusEnum.Normal;
            //                attr.RemoveAt(0);
            //                commands.Add(this.ParseScrollActionCommand(attr));
            //                continue;
            //            }
            //            if (attr.EqualsEx("[7h"))
            //            {
            //                this.status = ParserStatusEnum.Normal;
            //                commands.Add(PredefineCommands.EnableLineWrap);
            //                continue;
            //            }
            //            if (attr.EqualsEx("[7l"))
            //            {
            //                this.status = ParserStatusEnum.Normal;
            //                commands.Add(PredefineCommands.DisableLineWrap);
            //                continue;
            //            }
            //            if (c == '(')
            //            {
            //                this.status = ParserStatusEnum.Normal;
            //                commands.Add(PredefineCommands.SetDefaultFont);
            //                continue;
            //            }
            //            if (c == ')')
            //            {
            //                this.status = ParserStatusEnum.Normal;
            //                commands.Add(PredefineCommands.SetAlternateFont);
            //                continue;
            //            }
            //            if (attr.EqualsEx("]0") || attr.EqualsEx("]1") || attr.EqualsEx("]2") || attr.EqualsEx("]4") || attr.EqualsEx("]10"))
            //            {
            //                this.status = ParserStatusEnum.Normal;
            //                continue;
            //            }
            //            attr.Add(c);
            //            break;
            //    }

            //}

            #endregion

            return commands;
        }
    }
}