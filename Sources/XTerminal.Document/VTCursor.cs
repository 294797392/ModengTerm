using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document
{
    public class VTCursor : VTDocumentElement
    {
        /// <summary>
        /// 光标所在列
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// 光标所在行
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// 光标所在的行
        /// </summary>
        public VTextLine OwnerLine { get; set; }
    }
}
