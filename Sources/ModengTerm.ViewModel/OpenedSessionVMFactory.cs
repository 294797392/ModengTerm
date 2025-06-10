using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base.Enumerations;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using XTerminal;
using ModengTerm.ViewModel.Terminal;

namespace ModengTerm.ViewModel
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
                case SessionTypeEnum.Localhost:
                    {
                        return new ShellSessionVM(session);
                    }

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
