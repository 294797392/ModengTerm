using ModengTerm.Terminal;
using ModengTerm.Terminal.ViewModels;
using System.Windows;

namespace ModengTerm.Addons.About
{
    public class AboutAddon : AddonModule
    {
        protected override void OnActive(ActiveContext context)
        {
            this.RegisterCommand("AboutAddon.OpenAboutWindow", ExecuteOpenAboutWindowCommand);
        }

        protected override void OnDeactive()
        {
        }

        private void ExecuteOpenAboutWindowCommand(CommandArgs context)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.Owner = Application.Current.MainWindow;
            aboutWindow.ShowDialog();
        }
    }
}
