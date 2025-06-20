using ModengTerm.Addons;
using ModengTerm.Base;
using ModengTerm.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.OfficialAddons.Logger
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

        private void ExecuteStartLoggerCommand(CommandArgs e)
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

        private void ExecuteStopLoggerCommand(CommandArgs e)
        {
            throw new RefactorImplementedException();
            //ShellSessionVM shellSessionVM = context.OpenedSession as ShellSessionVM;
            //shellSessionVM.StopLogger();
        }
    }
}
