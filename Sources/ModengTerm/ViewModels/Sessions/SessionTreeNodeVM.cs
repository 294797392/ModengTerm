using ModengTerm.ViewModels.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels.Session
{
    public abstract class SessionTreeNodeVM : TreeNodeViewModel
    {
        public abstract SessionTreeNodeTypeEnum NodeType { get; }

        protected SessionTreeNodeVM(TreeViewModelContext context, int level, object data = null) :
            base(context, null)
        {
            this.Level = level;
        }
    }
}
