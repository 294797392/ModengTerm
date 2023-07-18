using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base;

namespace XTerminal.Session
{
    public class Win32CMDLineSession : SessionBase
    {
        #region 实例变量

        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("Win32CMDLineSession");

        #endregion

        #region 实例变量

        private Process process;
        private Task outputThread;
        private byte[] outputBuffer;
        private bool processExited;

        #endregion

        public Win32CMDLineSession(VTInitialOptions options) :
            base(options)
        { }

        public override int Connect()
        {
            this.processExited = false;
            this.outputBuffer = new byte[base.options.ReadBufferSize];

            this.process = Process.Start(new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = "",
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            });
            this.outputThread = Task.Factory.StartNew(this.OutputThreadProc);

            return ResponseCode.SUCCESS;
        }

        public override void Disconnect()
        {
            this.processExited = true;
            this.process.Kill();
            this.process.Dispose();
            this.outputThread.Wait();
        }

        public override int Input(VTInputEvent ievt)
        {
            this.process.StandardInput.Write((char)97);
            //this.process.StandardInput.Write((char)97);
            //this.process.StandardInput.Write('\r');
            //this.process.StandardInput.Write('\n');

            return ResponseCode.SUCCESS;
        }

        public override int Write(byte[] bytes)
        {
            return ResponseCode.SUCCESS;
        }

        #region 事件处理器

        private void OutputThreadProc()
        {
            while (!this.processExited)
            {
                try
                {
                    int count = this.process.StandardOutput.BaseStream.Read(this.outputBuffer, 0, this.outputBuffer.Length);
                    byte[] buffe = new byte[count];
                    Buffer.BlockCopy(this.outputBuffer, 0, buffe, 0, count);
                    this.NotifyDataReceived(buffe);
                }
                catch (Exception ex)
                {
                    logger.Error("OutputThreadProc异常", ex);
                }
            }

            logger.ErrorFormat("OutputThread退出");
        }

        #endregion
    }
}
