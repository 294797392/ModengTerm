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

        protected override void OnActive(ActiveContext e)
        {
            this.RegisterCommand("BroadcastInputAddon.OpenBroadcastInputWindow", this.OpenBroadcastInputWindow);
            this.RegisterCommand(ShellSessionCommands.CMD_USER_INPUT, this.OnUserInput);
            this.RegisterCommand(GlobalCommands.CMD_SHELL_SESSION_OPENED, this.OnShellSessionOpened);
        }

        protected override void OnDeactive()
        {
        }

        #endregion

        #region 实例方法

        private void LoadBroadcastList()
        {
            this.broadcastSessions.Clear();

            IAddonSession session = this.Shell.GetCurrentSession();
            List<BroadcastSession> broadcastSessions = this.ObjectStorage.GetObjects<BroadcastSession>(session.Id);
            List<IShellSession> shellSessions = this.Shell.GetSessions<IShellSession>();

            foreach (IShellSession shellSession in shellSessions)
            {
                if (shellSession == session)
                {
                    continue;
                }

                BroadcastSessionVM broadcastSession = new BroadcastSessionVM()
                {
                    ID = session.Id,
                    Name = session.Name,
                    Session = shellSession,
                };

                this.broadcastSessions.Add(broadcastSession);
            }
        }

        #endregion

        #region 事件处理器

        private void OpenBroadcastInputWindow(CommandEventArgs e)
        {
            IAddonSession session = this.Shell.GetCurrentSession();
            List<IShellSession> shellSessions = this.Shell.GetSessions<IShellSession>();
            List<BroadcastSessionVM> broadcastSessions = this.broadcastSessions.ToList();

            BroadcastInputManagerWindow window = new BroadcastInputManagerWindow(broadcastSessions, shellSessions);
            window.ObjectStorage = this.ObjectStorage;
            window.SessionId = session.Id;
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
        private void OnUserInput(CommandEventArgs e)
        {
            foreach (BroadcastSessionVM broadcastSession in this.broadcastSessions)
            {
                IShellSession session = broadcastSession.Session;

                if (session.Status != SessionStatusEnum.Connected) 
                {
                    continue;
                }

                byte[] bytes = e.Argument as byte[];

                session.Send(bytes);
            }
        }

        private void OnShellSessionOpened(CommandEventArgs e)
        {
            this.LoadBroadcastList();
        }

        #endregion
    }
}
