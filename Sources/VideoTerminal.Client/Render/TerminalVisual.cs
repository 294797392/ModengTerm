using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace XTerminal.Render
{
    /// <summary>
    /// 表示终端上的一个可视化对象（光标，文本块...）
    /// </summary>
    public abstract class TerminalVisual : DrawingVisual
    {
        public TerminalVisual()
        {
        }

        protected abstract void Draw(DrawingContext dc);

        public void Draw()
        {
            DrawingContext dc = this.RenderOpen();

            this.Draw(dc);

            dc.Close();
        }
    }
}
