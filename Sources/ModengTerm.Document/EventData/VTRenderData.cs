using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document.EventData
{
    public class VTRenderData
    {
        public static readonly VTRenderData Instance = new VTRenderData();

        /// <summary>
        /// 渲染耗时，单位是毫秒
        /// </summary>
        public int Elapsed { get; internal set; }
    }
}
