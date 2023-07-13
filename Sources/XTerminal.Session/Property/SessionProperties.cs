using DotNEToolkit.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Session.Property
{
    public class SessionProperties
    {
        public string KeyFilePath { get; set; }

        public string KeyFilePassphrase { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string ServerAddress { get; set; }

        public int ServerPort { get; set; }
    }
}
