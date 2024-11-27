using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.ViewModels.Sessions;
using System;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels.Session
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
                        URI = session.GetOption<string>(OptionKeyEnum.SERIAL_PORT_NAME);
                        break;
                    }

                case SessionTypeEnum.SSH:
                    {
                        URI = session.GetOption<string>(OptionKeyEnum.SSH_ADDR);
                        break;
                    }

                case SessionTypeEnum.Localhost:
                    {
                        URI = session.GetOption<string>(OptionKeyEnum.CMD_STARTUP_PATH);
                        break;
                    }

                case SessionTypeEnum.AdbShell:
                    {
                        AdbLoginTypeEnum loginType = session.GetOption<AdbLoginTypeEnum>(OptionKeyEnum.ADBSH_LOGIN_TYPE);
                        switch (loginType)
                        {
                            case AdbLoginTypeEnum.UserNamePassword:
                                {
                                    string userName = session.GetOption<string>(OptionKeyEnum.ADBSH_USERNAME);
                                    string password = session.GetOption<string>(OptionKeyEnum.ADBSH_PASSWORD);
                                    URI = string.Format("{0}", userName);
                                    break;
                                }

                            case AdbLoginTypeEnum.Password:
                                {
                                    string password = this.Session.GetOption<string>(OptionKeyEnum.ADBSH_PASSWORD);
                                    URI = string.Format("{0}", password);
                                    break;
                                }

                            case AdbLoginTypeEnum.None:
                                {
                                    break;
                                }

                            default:
                                throw new NotImplementedException();
                        }

                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
