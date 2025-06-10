using ModengTerm.Base;
using ModengTerm.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addons.Logger
{
    public class LoggerAddon : AddonModule
    {
        protected override void OnActive(ActiveContext e)
        {
            this.RegisterCommand("LoggerAddon.StartLogger", ExecuteStartLoggerCommand);
            this.RegisterCommand("LoggerAddon.StopLogger", ExecuteStopLoggerCommand);
        }

        protected override void OnDeactive()
        {
        }

        private void ExecuteStartLoggerCommand(CommandArgs context)
        {
            throw new RefactorImplementedException();
            //ShellSessionVM shellSessionVM = context.OpenedSession as ShellSessionVM;
            //LoggerOptionsWindow window = new LoggerOptionsWindow(shellSessionVM);
            //window.Owner = context.MainWindow;
            //if ((bool)window.ShowDialog())
            //{
            //    shellSessionVM.StartLogger(shellSessionVM.VideoTerminal, window.Options);
            //}
        }

        private void ExecuteStopLoggerCommand(CommandArgs context)
        {
            throw new RefactorImplementedException();
            //ShellSessionVM shellSessionVM = context.OpenedSession as ShellSessionVM;
            //shellSessionVM.StopLogger();
        }
    }
}
