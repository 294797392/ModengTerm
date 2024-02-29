using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document
{
    public enum VTInputTypes
    {
        TextInput,
        KeyInput
    }

    /// <summary>
    /// 存储文本的输入信息
    /// </summary>
    public class VTInput
    {
        public VTInputTypes Type { get; set; }

        public VTKeys Key { get; set; }

        public string Text { get; set; }
    }
}
