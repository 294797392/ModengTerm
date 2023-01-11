using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminalClient
{
    public class SSHClientAuthorition : ClientAuthorition
    {
        public string KeyFilePath { get; set; }

        public string KeyFilePassphrase { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string ServerAddress { get; set; }

        public int ServerPort { get; set; }

        public string TerminalName { get; set; }

        public uint TerminalColumns { get; set; }

        public uint TerminalRows { get; set; }

        public SSHClientAuthorition()
        {
            //this.TerminalColumns = DefaultValues.TerminalColumns;
            //this.TerminalRows = DefaultValues.TerminalRows;
            //this.TerminalName = DefaultValues.TerminalName;
        }
    }
}
