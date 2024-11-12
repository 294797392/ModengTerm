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
        public static readonly RenderEventArgs Instance = new RenderEventArgs();

        private RenderEventArgs()
        {
        }
    }
}
