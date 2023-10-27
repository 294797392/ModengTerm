using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base.Enumerations;
using ModengTerm.ViewModels;
using ModengTerm.Terminal.ViewModels;
using XTerminal.Base.DataModels;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;

namespace ModengTerm
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
                case SessionTypeEnum.libvtssh:
                case SessionTypeEnum.SerialPort:
                case SessionTypeEnum.SSH:
                case SessionTypeEnum.Win32CommandLine:
                    {
                        return new ShellSessionVM(session);
                    }

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
