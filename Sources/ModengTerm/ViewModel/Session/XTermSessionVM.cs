using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using System;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel.Session
{
    public class XTermSessionVM : SessionTreeNodeVM
    {
        private SessionTypeEnum type;
        private DateTime creationTime;
        private string uri;

        /// <summary>
        /// 会话类型
        /// </summary>
        public SessionTypeEnum Type
        {
            get { return type; }
            set
            {
                type = value;
                NotifyPropertyChanged("Type");
            }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime
        {
            get { return creationTime; }
            set
            {
                if (creationTime != value)
                {
                    creationTime = value;
                    NotifyPropertyChanged("CreationTime");
                }
            }
        }

        /// <summary>
        /// 主机名
        /// </summary>
        public string URI
        {
            get { return uri; }
            set
            {
                if (uri != value)
                {
                    uri = value;
                    NotifyPropertyChanged("URI");
                }
            }
        }

        public XTermSession Session { get; private set; }

        public override SessionTreeNodeTypeEnum NodeType => SessionTreeNodeTypeEnum.Session;

        public XTermSessionVM(TreeViewModelContext context, int level, object data = null)
            : base(context, level, data)
        {
            XTermSession session = data as XTermSession;

            Session = session;
            ID = session.ID;
            Name = session.Name;
            Description = session.Description;
            CreationTime = session.CreationTime;
            Type = (SessionTypeEnum)session.Type;

            switch ((SessionTypeEnum)session.Type)
            {
                case SessionTypeEnum.SerialPort:
                    {
                        this.URI = session.GetOption<string>(PredefinedOptions.SERIAL_PORT_NAME);
                        break;
                    }

                case SessionTypeEnum.Ssh:
                    {
                        this.URI = session.GetOption<string>(PredefinedOptions.SSH_SERVER_ADDR);
                        break;
                    }

                case SessionTypeEnum.LocalConsole:
                    {
                        this.URI = session.GetOption<string>(PredefinedOptions.CONSOLE_STARTUP_PATH);
                        break;
                    }

                case SessionTypeEnum.Tcp:
                    {
                        string ipaddr = session.GetOption<string>(PredefinedOptions.RAW_TCP_ADDRESS);
                        int port = session.GetOption<int>(PredefinedOptions.RAW_TCP_PORT);
                        this.URI = string.Format("{0}:{1}", ipaddr, port);
                        break;
                    }

                case SessionTypeEnum.Sftp:
                    {
                        this.URI = session.GetOption<string>(PredefinedOptions.SSH_SERVER_ADDR);
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
