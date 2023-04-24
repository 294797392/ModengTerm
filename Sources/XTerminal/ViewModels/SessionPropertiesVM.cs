using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;
using XTerminal.Session.Property;
using XTerminal.Sessions;

namespace XTerminal.ViewModels
{
    public abstract class SessionPropertiesVM : ViewModelBase
    {
        /// <summary>
        /// 获取该会话的属性
        /// </summary>
        /// <returns></returns>
        public abstract SessionProperties GetProperties();

        public static SessionPropertiesVM Create(SessionTypeEnum type)
        {
            switch (type)
            {
                case SessionTypeEnum.SSH:
                case SessionTypeEnum.libvtssh: return new SSHSessionPropertiesVM();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
