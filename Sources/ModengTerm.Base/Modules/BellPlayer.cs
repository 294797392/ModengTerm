using DotNEToolkit;
using DotNEToolkit.Media.Audio;
using DotNEToolkit.Modular;
using ModengTerm.Base;
using ModengTerm.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace ModengTerm.Base.Modules
{
    /// <summary>
    /// 播放响铃的播放器
    /// </summary>
    public static class BellPlayer
    {
        private static readonly object Object = new object();

        private static BufferQueue<object> playQueue = new BufferQueue<object>();

        static BellPlayer()
        {
            Task.Factory.StartNew(PlayThreadProc);
        }

        public static void Enqueue()
        {
            playQueue.Enqueue(Object);
        }

        private static void PlayThreadProc()
        {
            while (true)
            {
                object playItem = playQueue.Dequeue();

                Console.Beep(800, 10);
            }
        }
    }
}
