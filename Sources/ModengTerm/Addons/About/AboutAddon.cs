using ModengTerm.Terminal;
using ModengTerm.Terminal.ViewModels;
using System.Windows;

namespace ModengTerm.Addons.About
{
    public class AboutAddon : AddonModule
    {
        protected override void OnInitialize()
        {
            this.RegisterCommand("AboutAddon.OpenAboutWindow", ExecuteOpenAboutWindowCommand);
        }

        protected override void OnRelease()
        {
        }

        private void ExecuteOpenAboutWindowCommand(CommandEventArgs context)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.Owner = Application.Current.MainWindow;
            aboutWindow.ShowDialog();
        }
    }
}
