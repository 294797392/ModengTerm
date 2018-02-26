using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminal.Terminal
{
    public interface ITerminal
    {
        event Action<object, IEnumerable<IEscapeSequencesCommand>> CommandReceived;

        event Action<object, byte[]> DataReceived;

        ITerminalAuthorition Authorition { get; set; }

        bool Initialize();

        bool Release();

        bool Connect();

        bool Disconnect();

        bool Send(string text);
    }
}