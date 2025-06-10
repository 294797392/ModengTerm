using ModengTerm.Addons.Shell;
using ModengTerm.Base.Enumerations;
using System.Collections.Generic;
using System.Linq;
using WPFToolkit.MVVM;

namespace ModengTerm.Addons.BroadcastInput
{
    public class BroadcastInputAddon : AddonModule
    {
        #region 实例变量

        private BindableCollection<BroadcastSessionVM> broadcastSessions;

        #endregion

        #region AddonModule

        protected override void OnActive(ActiveContext context)
        {
            this.RegisterCommand("BroadcastInputAddon.OpenBroadcastInputWindow", this.OpenBroadcastInputWindow);
            this.RegisterCommand(AddonCommands.TERM_SESSION_OPENED, this.OnShellSessionOpened);
        }

        protected override void OnDeactive()
        {
        }

        #endregion

        #region 实例方法

        private void LoadBroadcastList()
        {
            this.broadcastSessions.Clear();

            ITerminalShell terminalShell = ShellFactory.GetActiveShell<ITerminalShell>();
            List<BroadcastSession> broadcastSessions = this.ObjectStorage.GetObjects<BroadcastSession>(terminalShell.Id);
            List<ITerminalShell> terminalShells = ShellFactory.GetAllShells<ITerminalShell>();

            foreach (ITerminalShell shell in terminalShells)
            {
                if (shell == terminalShell)
                {
                    continue;
                }

                BroadcastSessionVM broadcastSession = new BroadcastSessionVM()
                {
                    ID = shell.Id,
                    Name = shell.Name,
                    Session = shell,
                };

                this.broadcastSessions.Add(broadcastSession);
            }
        }

        #endregion

        #region 事件处理器

        private void OpenBroadcastInputWindow(CommandArgs e)
        {
            ITerminalShell terminalShell = ShellFactory.GetActiveShell<ITerminalShell>();
            List<ITerminalShell> terminalShells = ShellFactory.GetAllShells<ITerminalShell>();
            List<BroadcastSessionVM> broadcastSessions = this.broadcastSessions.ToList();

            BroadcastInputManagerWindow window = new BroadcastInputManagerWindow(broadcastSessions, terminalShells);
            window.ObjectStorage = this.ObjectStorage;
            window.SessionId = terminalShell.Id;
            window.Owner = e.MainWindow;
            if ((bool)window.ShowDialog())
            {
                this.LoadBroadcastList();
            }
        }

        /// <summary>
        /// 当用户输入之后触发
        /// </summary>
        /// <param name="e"></param>
        private void OnUserInput(CommandArgs e)
        {
            foreach (BroadcastSessionVM broadcastSession in this.broadcastSessions)
            {
                ITerminalShell session = broadcastSession.Session;

                if (session.Status != SessionStatusEnum.Connected)
                {
                    continue;
                }

                byte[] bytes = e.Argument as byte[];

                session.Send(bytes);
            }
        }

        private void OnShellSessionOpened(CommandArgs e)
        {
            this.LoadBroadcastList();
        }

        #endregion
    }
}
