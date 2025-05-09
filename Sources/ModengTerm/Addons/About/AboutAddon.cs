using ModengTerm.Terminal;
using ModengTerm.Terminal.ViewModels;
using System.Windows;

namespace ModengTerm.Addons.About
{
    public class AboutAddon : AddonBase
    {
        protected override void OnInitialize()
        {
            RegisterCommand("AboutAddon.OpenAboutWindow", ExecuteOpenAboutWindowCommand);
        }

        protected override void OnRelease()
        {
        }

        protected override void OnEvent(AddonEventTypes evt, params object[] evp)
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
