using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;
using XTerminal.Channels;
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

        public XTermSessionVM()
        {
        }
    }
}
