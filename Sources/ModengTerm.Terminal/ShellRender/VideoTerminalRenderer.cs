using ModengTerm.Base.DataModels;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.ShellRender
{
    /// <summary>
    /// 渲染为终端界面
    /// </summary>
    public class VideoTerminalRenderer : ShellRenderer
    {
        public VTParser Parser { get; set; }

        public VideoTerminalRenderer(VideoTerminal vt) :
            base(vt)
        {
        }

        public override void Initialize()
        {
        }

        public override void OnInteractionStateChanged(InteractionStateEnum istate)
        {
        }

        public override void Release()
        {
        }

        public override void Render(byte[] bytes, int length)
        {
            // 使用解析器处理数据，然后解析器回调到VideoTerminal，VideoTerminal再进行处理
            this.Parser.ProcessCharacters(bytes, length);
        }
    }
}
