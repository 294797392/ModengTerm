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
        /// <summary>
        /// 唯一标志符
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// 选项名字
        /// </summary>
        public string Name { get; set; }

        public Type EntryType { get; set; }

        /// <summary>
        /// 子选项页面
        /// </summary>
        public List<OptionDefinition> Children { get; set; }

        public OptionDefinition(string name, Type entryType)
        {
            this.ID = Guid.NewGuid().ToString();
            this.Name = name;
            this.EntryType = entryType;
        }
    }
}
