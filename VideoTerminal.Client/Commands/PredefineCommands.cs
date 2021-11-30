using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XTerminal.Terminal;

namespace XTerminal.Commands
{
    /// <summary>
    /// 默认定义好了的命令
    /// </summary>
    public class PredefineCommands : IEscapeSequencesCommand
    {
        /// <summary>
        /// 换行
        /// </summary>
        public static PredefineCommands NewLine { get; private set; }

        /// <summary>
        /// 回车
        /// </summary>
        public static PredefineCommands Enter { get; private set; }

        /// <summary>
        /// Set alternate font.
        /// </summary>
        public static PredefineCommands SetAlternateFont { get; private set; }

        /// <summary>
        /// Set default font
        /// </summary>
        public static PredefineCommands SetDefaultFont { get; private set; }

        /// <summary>
        /// Text wraps to next line if longer than the length of the display area.
        /// </summary>
        public static PredefineCommands EnableLineWrap { get; private set; }

        /// <summary>
        /// Disables line wrapping.
        /// </summary>
        public static PredefineCommands DisableLineWrap { get; private set; }

        /// <summary>
        /// Scroll display up one line.
        /// </summary>
        public static PredefineCommands ScrollUp { get; private set; }

        /// <summary>
        /// Scroll display down one line
        /// </summary>
        public static PredefineCommands ScrollDown { get; private set; }

        /// <summary>
        /// Enable scrolling for entire display
        /// </summary>
        public static PredefineCommands ScrollScreen { get; private set; }

        /// <summary>
        /// Clears tab at the current position
        /// </summary>
        public static PredefineCommands ClearTab { get; private set; }

        /// <summary>
        /// ClearAllTabs
        /// </summary>
        public static PredefineCommands ClearAllTabs { get; private set; }

        /// <summary>
        /// Sets a tab at the current position.
        /// </summary>
        public static PredefineCommands SetTab { get; private set; }

        /// <summary>
        /// 显示光标
        /// </summary>
        public static PredefineCommands VisibleCursor { get; private set; }

        /// <summary>
        /// 隐藏光标
        /// </summary>
        public static PredefineCommands HiddenCursor { get; private set; }

        /// <summary>
        /// 保存光标位置
        /// </summary>
        public static PredefineCommands SaveCursorPosition { get; private set; }

        /// <summary>
        /// 恢复光标位置
        /// </summary>
        public static PredefineCommands RestorCursorPosition { get; private set; }

        /// <summary>
        /// 清除从光标到行尾的内容
        /// </summary>
        public static PredefineCommands EraseEndOfLine { get; private set; }

        /// <summary>
        /// 清除从光标到行头的内容
        /// </summary>
        public static PredefineCommands EraseStartOfLine { get; private set; }

        /// <summary>
        /// 清除一行内容
        /// </summary>
        public static PredefineCommands EraseLine { get; private set; }

        /// <summary>
        /// Erases the screen from the current line down to the bottom of the screen
        /// </summary>
        public static PredefineCommands EraseDown { get; private set; }

        /// <summary>
        /// Erases the screen from the current line up to the top of the screen
        /// </summary>
        public static PredefineCommands EraseUp { get; private set; }

        /// <summary>
        /// Erases the screen with the background colour and moves the cursor to home.
        /// </summary>
        public static PredefineCommands EraseScreen { get; private set; }

        static PredefineCommands()
        {
            SetAlternateFont = new PredefineCommands();
            SetDefaultFont = new PredefineCommands();
            EnableLineWrap = new PredefineCommands();
            ClearTab = new PredefineCommands();
            SetTab = new PredefineCommands();
            EraseScreen = new PredefineCommands();
            EraseUp = new PredefineCommands();
            VisibleCursor = new PredefineCommands();
            HiddenCursor = new PredefineCommands();
            SaveCursorPosition = new PredefineCommands();
            RestorCursorPosition = new PredefineCommands();
            EraseEndOfLine = new PredefineCommands();
            EraseStartOfLine = new PredefineCommands();
            EraseLine = new PredefineCommands();
            EraseDown = new PredefineCommands();
            ClearAllTabs = new PredefineCommands();
            ScrollScreen = new PredefineCommands();
            ScrollDown = new PredefineCommands();
            ScrollUp = new PredefineCommands();

            NewLine = new PredefineCommands();
            Enter = new PredefineCommands();
        }
    }
}