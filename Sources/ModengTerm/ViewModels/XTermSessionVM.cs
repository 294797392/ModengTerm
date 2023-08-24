using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;
using XTerminal.Base.DataModels;
using XTerminal.Base.Enumerations;

namespace XTerminal.ViewModels
{
    public class XTermSessionVM : ItemViewModel
    {
        private SessionTypeEnum type;
        private DateTime creationTime;

        /// <summary>
        /// 会话类型
        /// </summary>
        public SessionTypeEnum Type
        {
            get { return this.type; }
            set
            {
                this.type = value;
                this.NotifyPropertyChanged("Type");
            }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime
        {
            get { return this.creationTime; }
            set
            {
                if (this.creationTime != value)
                {
                    this.creationTime = value;
                    this.NotifyPropertyChanged("CreationTime");
                }
            }
        }

        public XTermSession Session { get; private set; }

        public XTermSessionVM(XTermSession session)
        {
            this.Session = session;
            this.ID = session.ID;
            this.Name = session.Name;
            this.Description = session.Description;
            this.CreationTime = session.CreationTime;
            this.Type = (SessionTypeEnum)session.SessionType;
        }
    }
}
