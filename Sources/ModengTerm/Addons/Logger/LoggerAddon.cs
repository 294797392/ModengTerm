using ModengTerm.Terminal.ViewModels;
using ModengTerm.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addons.Logger
{
    public class LoggerAddon : AddonBase
    {
        protected override void OnInitialize()
        {
            RegisterCommand("LoggerAddon.StartLogger", ExecuteStartLoggerCommand);
            RegisterCommand("LoggerAddon.StopLogger", ExecuteStopLoggerCommand);
        }

        protected override void OnRelease()
        {
        }

        protected override void OnEvent(AddonEventTypes ev, params object[] param)
        {
        }

        private void ExecuteStartLoggerCommand()
        {
            ShellSessionVM shellSessionVM = MTermApp.Context.MainWindowVM.SelectedSession as ShellSessionVM;
            LoggerOptionsWindow window = new LoggerOptionsWindow(shellSessionVM);
            window.Owner = System.Windows.Window.GetWindow(shellSessionVM.Content);
            if ((bool)window.ShowDialog())
            {
                shellSessionVM.StartLogger(shellSessionVM.VideoTerminal, window.Options);
            }
        }

        private void ExecuteStopLoggerCommand()
        {
            ShellSessionVM shellSessionVM = MTermApp.Context.MainWindowVM.SelectedSession as ShellSessionVM;
            shellSessionVM.StopLogger();
        }
    }
}
