using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document.Rendering
{
    /// <summary>
    /// 表示画板容器
    /// 容器里可以显示多个画图
    /// </summary>
    public interface IDocumentCanvasPanel
    {
        IDocumentCanvas CreateCanvas();

        void AddCanvas(IDocumentCanvas canvas);
    }
}
