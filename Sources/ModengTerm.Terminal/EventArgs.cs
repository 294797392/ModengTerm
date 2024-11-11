using ModengTerm.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal
{
    public class RenderEventArgs
    {
        public class ChangedLine
        {
            public int PhysicsRow { get; set; }

            public VTHistoryLine HistoryLine { get; set; }
        }

        /// <summary>
        /// 存储被修改的行列表
        /// </summary>
        public List<ChangedLine> ChangedLines { get; private set; }

        public RenderEventArgs()
        {
            this.ChangedLines = new List<ChangedLine>();
        }
    }
}
