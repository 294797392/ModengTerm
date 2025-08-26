using ModengTerm.Base;
using ModengTerm.Terminal.Engines;
using Renci.SshNet;

namespace ModengTerm.Terminal.FileWatch
{
    public class SshFileWatcher : FileWatcher
    {
        #region 实例变量

        private SshNetEngine sshNetDrv;
        private SshClient sshClient;
        private SshCommand sshCommand;

        #endregion

        #region FileWatcher

        public override long Avaliable => this.sshCommand.OutputStream.Length;

        protected override int OnInitialize()
        {
            this.sshNetDrv = this.SessionDriver as SshNetEngine;
            this.sshClient = this.sshNetDrv.SshClient;
            this.sshCommand = this.sshClient.CreateCommand(string.Format("tail -f {0}", this.FilePath));
            this.sshCommand.ExecuteAsync();

            return ResponseCode.SUCCESS;
        }

        protected override void OnRelease()
        {
            this.sshCommand.Dispose();
        }

        public override int Read(byte[] buffer, int offset, int size)
        {
            return this.sshCommand.OutputStream.Read(buffer, offset, size);
        }

        #endregion
    }
}
