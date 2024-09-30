using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using System;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels
{
    public class XTermSessionVM : ItemViewModel
    {
        private SessionTypeEnum type;
        private DateTime creationTime;
        private string uri;

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
        public string URI
        {
            get { return this.uri; }
            set
            {
                if (this.uri != value)
                {
                    this.uri = value;
                    this.NotifyPropertyChanged("URI");
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
                        this.URI = session.GetOption<string>(OptionKeyEnum.SERIAL_PORT_NAME);
                        break;
                    }

                case SessionTypeEnum.SSH:
                    {
                        this.URI = session.GetOption<string>(OptionKeyEnum.SSH_ADDR);
                        break;
                    }

                case SessionTypeEnum.CommandLine:
                    {
                        this.URI = session.GetOption<string>(OptionKeyEnum.CMD_STARTUP_PATH);
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
