using ModengTerm.Base.DataModels;
using ModengTerm.ViewModels.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels.Session
{
    public class SessionGroupVM : SessionTreeNodeVM
    {
        public override SessionTreeNodeTypeEnum NodeType => SessionTreeNodeTypeEnum.Group;

        public bool IsRoot { get { return string.IsNullOrEmpty(this.ID.ToString()); } }

        public SessionGroupVM(TreeViewModelContext context, object data = null)
            : base(context, data)
        {
            SessionGroup sessionGroup = data as SessionGroup;

            this.ID = sessionGroup.ID.ToString();
            this.Name = sessionGroup.Name;
        }
    }
}
