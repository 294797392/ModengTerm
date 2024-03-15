using ModengTerm.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal
{
    /// <summary>
    /// 管理主屏幕缓冲区
    /// </summary>
    public class PrimaryScreen : VTScreen
    {
        public override bool IsAlternate => false;

        public PrimaryScreen(VTOptions options) :
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
