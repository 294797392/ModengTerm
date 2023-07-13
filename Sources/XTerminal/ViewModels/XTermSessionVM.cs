using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;
using XTerminal.Base.DataModels;
using XTerminal.Channels;
using XTerminal.Session.Enumerations;
using XTerminal.Sessions;

namespace XTerminal.ViewModels
{
    public class XTermSessionVM : ItemViewModel
    {
        private SessionTypeEnum type;
        private string host;
        private int port;
        private string userName;
        private string password;
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
        /// 要连接的主机名
        /// </summary>
        public string Host
        {
            get { return this.host; }
            set
            {
                this.host = value;
                this.NotifyPropertyChanged("Host");
            }
        }

        /// <summary>
        /// 要连接的主机端口号
        /// </summary>
        public int Port
        {
            get { return this.port; }
            set
            {
                this.port = value;
                this.NotifyPropertyChanged("Port");
            }
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get { return this.userName; }
            set
            {
                this.userName = value;
                this.NotifyPropertyChanged("UserName");
            }
        }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password
        {
            get { return this.password; }
            set
            {
                this.password = value;
                this.NotifyPropertyChanged("Password");
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
        /// 身份验证类型
        /// </summary>
        public int AuthType { get; set; }

        public XTermSession Session { get; private set; }

        public XTermSessionVM(XTermSession session)
        {
            this.Session = session;
            this.ID = session.ID;
            this.Name = session.Name;
            this.Description = session.Description;
            this.CreationTime = session.CreationTime;
            this.Type = (SessionTypeEnum)session.Type;
            this.AuthType = session.AuthType;
            this.Host = session.Host;
            this.Port = session.Port;
            this.UserName = session.UserName;
            this.Password = session.Password;
        }
    }
}
