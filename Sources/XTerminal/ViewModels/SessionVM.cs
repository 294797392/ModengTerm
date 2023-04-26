﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;
using XTerminal.Channels;
using XTerminal.Sessions;

namespace XTerminal.ViewModels
{
    public class SessionVM : ItemViewModel
    {
        private SessionTypeEnum type;

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
        /// 该会话的属性
        /// </summary>
        public SessionPropertiesVM PropertiesVM { get; set; }

        public SessionVM()
        {
        }
    }
}