using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;
using XTerminal.Session.Property;

namespace XTerminal.ViewModels
{
    public class SSHSessionPropertiesVM : SessionPropertiesVM
    {
        #region 实例变量

        private string hostName;
        private string password;
        private int port;

        #endregion

        #region 属性

        /// <summary>
        /// 所支持的身份验证方式
        /// </summary>
        public BindableCollection<SSHAuthEnum> AuthList { get; private set; }

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

        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password
        {
            get { return this.password; }
            set
            {
                if (this.password != value)
                {
                    this.password = value;
                    this.NotifyPropertyChanged("Password");
                }
            }
        }

        /// <summary>
        /// 服务器端口号
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

        #endregion

        #region 构造方法

        public SSHSessionPropertiesVM()
        {
            this.AuthList = new BindableCollection<SSHAuthEnum>();
            this.AuthList.AddRange(Enum.GetValues(typeof(SSHAuthEnum)).Cast<SSHAuthEnum>());
        }

        #endregion

        #region SessionPropertiesVM

        public override string GetProperties()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
