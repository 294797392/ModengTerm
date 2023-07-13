using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminal.Session.Property
{
    public class SSHSessionProperties : SessionProperties
    {
        public string KeyFilePath { get; set; }

        public string KeyFilePassphrase { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string ServerAddress { get; set; }

        public int ServerPort { get; set; }

        public SSHSessionProperties()
        {
            //this.TerminalColumns = DefaultValues.TerminalColumns;
            //this.TerminalRows = DefaultValues.TerminalRows;
            //this.TerminalName = DefaultValues.TerminalName;
        }
    }
}
