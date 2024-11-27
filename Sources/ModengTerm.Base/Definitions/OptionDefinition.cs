using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Base.Definitions
{
    public class OptionDefinition
    {
        public static readonly string CommandLineID = Guid.NewGuid().ToString();
        public static readonly string SshID = Guid.NewGuid().ToString();
        public static readonly string SerialPortID = Guid.NewGuid().ToString();
        public static readonly string TcpID = Guid.NewGuid().ToString();
        public static readonly string AdbShellID = Guid.NewGuid().ToString();

        /// <summary>
        /// 唯一标志符
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// 选项名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 控件类型
        /// </summary>
        public Type EntryType { get; set; }

        /// <summary>
        /// 子选项页面
        /// </summary>
        public List<OptionDefinition> Children { get; set; }

        public OptionDefinition(string name)
        {
            this.ID = Guid.NewGuid().ToString();
            this.Name = name;
        }

        public OptionDefinition(string name, Type entryType)
        {
            this.ID = Guid.NewGuid().ToString();
            this.Name = name;
            this.EntryType = entryType;
        }

        public OptionDefinition(string id, string name, Type entryType)
        {
            this.ID = id;
            this.Name = name;
            this.EntryType = entryType;
        }
    }
}
