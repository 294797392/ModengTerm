using ModengTerm.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal
{
    /// <summary>
    /// 管理备用屏幕缓冲区
    /// </summary>
    public class AlternateScreen : VTScreen
    {
        public override bool IsAlternate => true;

        public AlternateScreen(VTOptions options) :
            base(options)
        {

        }

        protected override void OnInitialize()
        {
        }

        protected override void OnRelease()
        {
        }
    }
}
