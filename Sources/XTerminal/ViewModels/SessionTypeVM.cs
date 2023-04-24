using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;
using XTerminal.Sessions;

namespace XTerminal.ViewModels
{
    public class SessionTypeVM : ItemViewModel
    {
        private SessionTypeEnum type;

        /// <summary>
        /// 参数配置界面的入口点类名
        /// </summary>
        public string ProviderEntry { get; set; }

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
    }
}
