using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using XTerminal;
using ModengTerm.ViewModel.Terminal;
using ModengTerm.ViewModel.FileTrans;

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
                case SessionTypeEnum.Tcp:
                case SessionTypeEnum.SerialPort:
                case SessionTypeEnum.Ssh:
                case SessionTypeEnum.Console:
                    {
                        return new ShellSessionVM(session);
                    }

                case SessionTypeEnum.Sftp:
                    {
                        return new FtpSessionVM(session);
                    }

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
