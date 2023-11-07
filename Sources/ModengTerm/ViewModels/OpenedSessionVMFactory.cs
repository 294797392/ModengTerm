using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base.Enumerations;
using ModengTerm.Terminal.ViewModels;
using XTerminal.Base.DataModels;
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
                case SessionTypeEnum.libvtssh:
                case SessionTypeEnum.SerialPort:
                case SessionTypeEnum.SSH:
                case SessionTypeEnum.Win32CommandLine:
                    {
                        MainWindow mainWindow = App.Current.MainWindow as MainWindow;

                        return new ShellSessionVM(session)
                        {
                            SendToAllCallback = mainWindow.SendToAllTerminal,
                            LoggerManager = MTermApp.Context.LoggerManager,
                        };
                    }

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
