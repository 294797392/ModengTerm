using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base.Enumerations;
using ModengTerm.Terminal.ViewModels;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using XTerminal;

namespace ModengTerm.ViewModels
{
    public static class OpenedSessionVMFactory
    {
        /// <summary>
        /// 创建OpenedSessionVM的实例
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static OpenedSessionVM Create(XTermSession session)
        {
            switch ((SessionTypeEnum)session.Type)
            {
                case SessionTypeEnum.SerialPort:
                case SessionTypeEnum.SSH:
                case SessionTypeEnum.WindowsConsole:
                    {
                        return new ShellSessionVM(session);
                    }

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
