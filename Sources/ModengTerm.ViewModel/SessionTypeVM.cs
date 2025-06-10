using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;
using XTerminal.Base.Enumerations;

namespace ModengTerm.ViewModel
{
    public class SessionTypeVM : ItemViewModel
    {
        private SessionTypeEnum type;

        /// <summary>
        /// 会话类型
        /// </summary>
        public SessionTypeEnum Type
        {
            get { return type; }
            set
            {
                type = value;
                this.NotifyPropertyChanged("Type");
            }
        }

        /// <summary>
        /// 该会话类型对应的选项菜单ID
        /// </summary>
        public string MenuId { get; private set; }

        public SessionTypeVM(SessionDefinition session)
        {
            this.ID = session.ID;
            this.Name = session.Name;
            this.Description = session.Description;
            Type = (SessionTypeEnum)session.Type;
            MenuId = session.MenuId;
        }
    }
}
