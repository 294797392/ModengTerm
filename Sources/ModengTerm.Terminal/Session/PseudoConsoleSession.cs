using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Terminal.Session.ConPTY;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConPTYTerminal = ModengTerm.Terminal.Session.ConPTY.Terminal;

namespace ModengTerm.Terminal.Session
{
    /// <summary>
    /// 使用PseudoConsoleAPI实现的Windows命令行会话，只支持在Windows10或者更高版本使用
    /// 参考：
    /// https://github.com/microsoft/terminal/tree/main/samples/ConPTY/GUIConsole/GUIConsole.ConPTY
    /// </summary>
    public class PseudoConsoleSession : SessionDriver
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("PseudoConsoleSession");

        private ConPTYTerminal terminal;
        private Stream consoleInStream;
        private Stream consoleOutStream;

        public PseudoConsoleSession(XTermSession session) :
            base(session)
        {
        }

        public override int Open()
        {
            int row = this.session.GetOption<int>(OptionKeyEnum.SSH_TERM_ROW);
            int col = this.session.GetOption<int>(OptionKeyEnum.SSH_TERM_COL);

            string startupPath = this.session.GetOption<string>(OptionKeyEnum.CMD_STARTUP_PATH);
            string startupArgs = this.session.GetOption<string>(OptionKeyEnum.CMD_STARTUP_ARGUMENT);
            string startupDirectory = this.session.GetOption<string>(OptionKeyEnum.CMD_STARTUP_DIR);

            string path = string.Format("{0} {1}", startupPath, startupArgs);

            this.terminal = new ConPTYTerminal();
            this.terminal.Start(path, startupDirectory, row, col);

            this.consoleInStream = this.terminal.ConsoleInStream;
            this.consoleOutStream = this.terminal.ConsoleOutStream;

            return ResponseCode.SUCCESS;
        }

        public override void Close()
        {
            this.terminal.Stop();
        }

        public override void Resize(int row, int col)
        {
            this.terminal.Resize(row, col);
        }

        public override void Write(byte[] buffer)
        {
            this.consoleInStream.Write(buffer);
            this.consoleInStream.Flush();
        }

        internal override int Read(byte[] buffer)
        {
            return this.consoleOutStream.Read(buffer, 0, buffer.Length);
        }
    }
}
