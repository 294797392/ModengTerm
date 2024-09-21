using ModengTerm.Base.DataModels;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Renderer
{
    /// <summary>
    /// 将收到的数据渲染为终端要显示的数据
    /// </summary>
    public class DefaultRenderer : VTRenderer
    {
        private VTParser vtParser;

        public DefaultRenderer(VideoTerminal vt, XTermSession session) :
            base(vt, session)
        {

        }

        public override void Initialize()
        {
            this.vtParser = new VTParser();
            this.vtParser.DispatchHandler = this.videoTerminal;
            this.vtParser.Initialize();
        }

        public override void Release()
        {
            this.vtParser.Release();
        }

        public override void OnInteractionStateChanged(InteractionStateEnum istate)
        {
        }

        public override void Render(byte[] bytes, int length)
        {
            this.vtParser.ProcessCharacters(bytes, length);
        }
    }
}
