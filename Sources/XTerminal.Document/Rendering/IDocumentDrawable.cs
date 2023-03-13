using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document.Rendering
{
    /// <summary>
    /// 存储界面上渲染对象的状态信息
    /// </summary>
    public interface IDocumentDrawable
    {
        /// <summary>
        /// 要渲染的文档模型里的信息
        /// </summary>
        VTDocumentElement OwnerElement { get; set; }
    }
}
