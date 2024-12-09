using ModengTerm.Base.DataModels;
using ModengTerm.Terminal.Session;
using Renci.SshNet;

namespace ModengTerm.Terminal.Watch
{
    public class UnixSshWatcher : UnixWatcher
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("UnixSshWatcher");

        #region 实例变量

        private SshNetSession sshNetDrv;
        private SshClient sshClient;
        private SshCommand sshCommand;

        #endregion

        #region 构造方法

        public UnixSshWatcher(XTermSession session, SessionDriver driver) :
            base(session, driver)
        {
            this.sshNetDrv = driver as SshNetSession;
            this.sshClient = sshNetDrv.SshClient;
            this.sshCommand = this.sshClient.CreateCommand(string.Empty);
        }

        #endregion

        #region UnixWatcher

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Release()
        {
            base.Release();
        }

        protected override string ReadFile(string filePath)
        {
            string commandText = string.Format("cat {0}", filePath);

            return this.sshCommand.Execute(commandText);
        }

        protected override string Execute(string command)
        {
            return this.sshCommand.Execute(command);
        }

        #endregion
    }
}
