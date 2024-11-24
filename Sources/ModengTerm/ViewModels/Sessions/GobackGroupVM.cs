using ModengTerm.ViewModels.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels.Sessions
{
    public class GobackGroupVM : SessionTreeNodeVM
    {
        public GobackGroupVM(TreeViewModelContext context, int level, object data = null) : base(context, level, data)
        {
        }

        public override SessionTreeNodeTypeEnum NodeType => SessionTreeNodeTypeEnum.GobackGroup;
    }
}
