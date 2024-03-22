using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.ServiceAgents.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Session
{
    public class PlaybackSession : SessionDriver
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("PlaybackSession");

        private string filePath;
        private PlaybackFile playbackFile;
        private PlaybackStream playbackStream;
        private AutoResetEvent playbackEvent;
        private long prevTimestamp = 0;

        public PlaybackSession(XTermSession session) :
            base(session)
        {
            this.filePath = session.GetOption<string>(OptionKeyEnum.SSH_PLAYBACK_FILE_PATH);
        }

        public override int Open()
        {
            this.playbackStream = new PlaybackStream();
            int code = this.playbackStream.OpenRead(this.playbackFile);
            if (code != ResponseCode.SUCCESS)
            {
                logger.ErrorFormat("打开回放文件异常, {0}", this.filePath);
                return code;
            }

            this.playbackEvent = new AutoResetEvent(false);
            return ResponseCode.SUCCESS;
        }

        public override void Close()
        {
            this.playbackStream.Close();
        }

        public override void Resize(int row, int col)
        {
        }

        public override int Write(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        internal override int Read(byte[] buffer)
        {
            // 读取下一帧
            PlaybackFrame nextFrame = this.playbackStream.GetNextFrame();

            if (nextFrame == null)
            {
                // 此时说明读取文件有异常
                logger.ErrorFormat("读取回放文件下一帧失败, 结束回放");
                return 0;
            }

            // 模拟延时
            if (prevTimestamp > 0)
            {
                long interval = nextFrame.Timestamp - prevTimestamp;
                this.playbackEvent.WaitOne((int)interval / 10000);
            }

            prevTimestamp = nextFrame.Timestamp;

            buffer = nextFrame.Data;
            return buffer.Length;
        }
    }
}
