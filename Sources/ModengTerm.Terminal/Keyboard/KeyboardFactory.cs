using ModengTerm.Base.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Keyboard
{
    public static class KeyboardFactory
    {
        public static KeyboardBase Create(SessionTypeEnum sessionType)
        {
            switch (sessionType)
            {
                case SessionTypeEnum.Localhost:
                case SessionTypeEnum.SSH:
                    return new DefaultKeyboard();

                case SessionTypeEnum.Tcp: return new TcpKeyboard();
                case SessionTypeEnum.SerialPort: return new SerialPortKeyboard();

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
