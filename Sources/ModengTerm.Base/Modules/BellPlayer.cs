using DotNEToolkit;
using DotNEToolkit.Media.Audio;
using DotNEToolkit.Modular;
using ModengTerm.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Base.Modules
{
    /// <summary>
    /// 播放响铃的播放器
    /// </summary>
    public class BellPlayer : ModuleBase
    {
        private static readonly object Object = new object();

        private BufferQueue<object> playQueue;

        protected override int OnInitialize()
        {
            this.playQueue = new BufferQueue<object>();
            Task.Factory.StartNew(this.PlayThreadProc);

            return ResponseCode.SUCCESS;
        }

        protected override void OnRelease()
        {
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
