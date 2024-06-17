using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;
using XTerminal.Base.DataModels;
using XTerminal.Base.Enumerations;

namespace ModengTerm.ViewModels
{
    public class XTermSessionVM : ItemViewModel
    {
        private SessionTypeEnum type;
        private DateTime creationTime;
        private string hostName;

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

        /// <summary>
        /// 主机名
        /// </summary>
        public string HostName
        {
            get { return this.hostName; }
            set
            {
                if (this.hostName != value)
                {
                    this.hostName = value;
                    this.NotifyPropertyChanged("HostName");
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
            this.Type = (SessionTypeEnum)session.Type;

            switch((SessionTypeEnum)session.Type)
            {
                case SessionTypeEnum.SerialPort:
                    {
                        this.HostName = session.GetOption<string>(OptionKeyEnum.SERIAL_PORT_NAME);
                        break;
                    }

                case SessionTypeEnum.SSH:
                    {
                        this.HostName = session.GetOption<string>(OptionKeyEnum.SSH_SERVER_ADDR);
                        break;
                    }

                case SessionTypeEnum.HostCommandLine:
                    {
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
