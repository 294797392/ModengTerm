using ModengTerm.Addons;
using ModengTerm.Addons.Shell;
using ModengTerm.Base;
using ModengTerm.Base.Addon;
using ModengTerm.Base.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using WPFToolkit.MVVM;

namespace ModengTerm.OfficialAddons.BroadcastInput
{
    public class BroadcastInputAddon : AddonModule
    {
        #region 实例变量

        private BindableCollection<BroadcastSessionVM> broadcastSessions;

        #endregion

        #region AddonModule

        protected override void OnActive(ActiveContext context)
        {
            this.RegisterEvent(EventType.SHELL_SESSION_OPENED, this.OnShellSessionOpened);
            this.RegisterCommand("BroadcastInputAddon.OpenBroadcastInputWindow", this.OpenBroadcastInputWindow);
        }

        protected override void OnDeactive()
        {
        }

        #endregion

        #region 实例方法

        private void LoadBroadcastList()
        {
            this.broadcastSessions.Clear();

            IShellPanel terminalShell = ShellFactory.GetActivePanel<IShellPanel>();
            List<BroadcastSession> broadcastSessions = this.ObjectStorage.GetObjects<BroadcastSession>(terminalShell.Id);
            throw new RefactorImplementedException();
            //List<IShellPanel> terminalShells = ShellFactory.GetAllPanels<IShellPanel>();

            //foreach (IShellPanel shell in terminalShells)
            //{
            //    if (shell == terminalShell)
            //    {
            //        continue;
            //    }

            //    BroadcastSessionVM broadcastSession = new BroadcastSessionVM()
            //    {
            //        ID = shell.Id,
            //        Name = shell.Name,
            //        Session = shell,
            //    };

            //    this.broadcastSessions.Add(broadcastSession);
            //}
        }

        #endregion

        #region 事件处理器

        private void OpenBroadcastInputWindow(CommandArgs e)
        {
            throw new RefactorImplementedException();
            //IShellPanel terminalShell = ShellFactory.GetActivePanel<IShellPanel>();
            //List<IShellPanel> terminalShells = ShellFactory.GetAllPanels<IShellPanel>();
            //List<BroadcastSessionVM> broadcastSessions = this.broadcastSessions.ToList();

            //BroadcastInputManagerWindow window = new BroadcastInputManagerWindow(broadcastSessions, terminalShells);
            //window.ObjectStorage = this.ObjectStorage;
            //window.SessionId = terminalShell.Id;
            //window.Owner = e.MainWindow;
            //if ((bool)window.ShowDialog())
            //{
            //    this.LoadBroadcastList();
            //}
        }

        /// <summary>
        /// 当用户输入之后触发
        /// </summary>
        /// <param name="e"></param>
        private void OnUserInput(CommandArgs e)
        {
            foreach (BroadcastSessionVM broadcastSession in this.broadcastSessions)
            {
                IShellPanel session = broadcastSession.Session;

                if (session.Status != SessionStatusEnum.Connected)
                {
                    continue;
                }

                byte[] bytes = e.Argument as byte[];

                session.Send(bytes);
            }
        }

        private void OnShellSessionOpened(EventType evType, EventArgs args)
        {
            this.LoadBroadcastList();
        }

        #endregion
    }
}
