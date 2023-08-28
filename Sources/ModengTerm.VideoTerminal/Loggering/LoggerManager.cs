using DotNEToolkit.Modular;
using ModengTerm.Base;
using ModengTerm.Terminal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XTerminal.Base;
using XTerminal.Document;

namespace ModengTerm.Terminal
{
    public class LoggerManager : AppModule<MTermManifest>
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("LoggerManager");

        #endregion

        #region 实例变量

        private Thread writeThread;
        private bool changed;

        #endregion

        #region AppModule

        protected override int OnInitialize()
        {
            this.writeThread = new Thread(this.WriteThreadProc);
            this.writeThread.IsBackground = true;
            this.writeThread.Start();

            return ResponseCode.SUCCESS;
        }

        protected override void OnRelease()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region 公开接口

        public void Start(VideoTerminal vt)
        {
            
        }

        public void Stop(VideoTerminal vt)
        { }

        public void Pause(VideoTerminal vt)
        { }

        public void Resume(VideoTerminal vt)
        { }

        #endregion

        #region 事件处理器

        private void WriteThreadProc()
        {
            while (true)
            {
                try
                {

                }
                catch (Exception ex)
                {
                }
            }
        }

        #endregion
    }
}
