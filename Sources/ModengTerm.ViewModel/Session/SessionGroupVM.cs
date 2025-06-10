using ModengTerm.Base.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel.Session
{
    public class SessionGroupVM : SessionTreeNodeVM
    {
        public override SessionTreeNodeTypeEnum NodeType => SessionTreeNodeTypeEnum.Group;

        public bool IsRoot { get { return string.IsNullOrEmpty(ID.ToString()); } }

        public SessionGroupVM(TreeViewModelContext context, int level, object data = null)
            : base(context, level, data)
        {
            SessionGroup sessionGroup = data as SessionGroup;

            ID = sessionGroup.ID.ToString();
            Name = sessionGroup.Name;
        }
    }
}
