using DotNEToolkit;
using DotNEToolkit.Media.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal
{
    /// <summary>
    /// 播放响铃的播放器
    /// </summary>
    public class BellPlayer : SingletonObject<BellPlayer>
    {
        private static readonly object Object = new object();

        private BufferQueue<object> playQueue;

        public BellPlayer() 
        {
            this.playQueue = new BufferQueue<object>();
            Task.Factory.StartNew(this.PlayThreadProc);
        }

        public void Enqueue() 
        {
            this.playQueue.Enqueue(Object);
        }

        private void PlayThreadProc() 
        {
            while (true) 
            {
                object playItem = this.playQueue.Dequeue();

                Console.Beep(800, 10);
            }
        }
    }
}
