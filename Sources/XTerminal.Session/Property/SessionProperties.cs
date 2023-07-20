using DotNEToolkit.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Session.Property
{
    public class SessionProperties
    {
        public string KeyFilePath { get; set; }

        public string KeyFilePassphrase { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        /// <summary>
        /// SSH：服务器地址
        /// 串口：端口号
        /// </summary>
        public string ServerAddress { get; set; }

        public int ServerPort { get; set; }

        /// <summary>
        /// 串口波特率
        /// </summary>
        public int BaudRate { get; set; }
    }
}
