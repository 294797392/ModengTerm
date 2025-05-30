using ModengTerm.Base;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.ViewModels;
using ModengTerm.Terminal.Windows;
using ModengTerm.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModengTerm.Addons.Record
{
    public class RecordAddon : AddonModule
    {
        protected override void OnActive(ActiveContext e)
        {
            this.RegisterCommand("RecordAddon.StartRecord", ExecuteStartRecordCommand);
            this.RegisterCommand("RecordAddon.StopRecord", ExecuteStopRecordCommand);
            this.RegisterCommand("RecordAddon.OpenRecord", ExecuteOpenRecordCommand);
        }

        protected override void OnDeactive()
        {
        }

        private void ExecuteStartRecordCommand(CommandArgs context)
        {
            ShellSessionVM shellSessionVM = MTermApp.Context.MainWindowVM.SelectedSession as ShellSessionVM;

            if (shellSessionVM.RecordStatus == RecordStatusEnum.Recording)
            {
                MTMessageBox.Info("正在录制中");
                return;
            }

            RecordOptionsVM recordOptionsVM = new RecordOptionsVM();

            RecordOptionsWindow recordOptionsWindow = new RecordOptionsWindow();
            recordOptionsWindow.Owner = Window.GetWindow(shellSessionVM.Content);
            recordOptionsWindow.DataContext = recordOptionsVM;
            if (!(bool)recordOptionsWindow.ShowDialog())
            {
                return;
            }

            shellSessionVM.StartRecord(recordOptionsVM.FileName);
        }

        private void ExecuteStopRecordCommand(CommandArgs context)
        {
            ShellSessionVM shellSessionVM = MTermApp.Context.MainWindowVM.SelectedSession as ShellSessionVM;
            shellSessionVM.StopRecord();
        }

        private void ExecuteOpenRecordCommand(CommandArgs context)
        {
            ShellSessionVM shellSessionVM = MTermApp.Context.MainWindowVM.SelectedSession as ShellSessionVM;
            OpenRecordWindow openRecordWindow = new OpenRecordWindow(shellSessionVM.Session);
            openRecordWindow.Owner = Window.GetWindow(shellSessionVM.Content);
            openRecordWindow.Show();
        }
    }
}
