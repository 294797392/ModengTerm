using DotNEToolkit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document
{
    /// <summary>
    /// 用来存储历史的行记录
    /// </summary>
    public class VTHistoryLine
    {
        /// <summary>
        /// 行索引，从0开始
        /// </summary>
        public int PhysicsRow { get; set; }

        /// <summary>
        /// 该行的所有字符
        /// 显示历史行的时候用到
        /// </summary>
        public List<VTCharacter> Characters { get; private set; }

        public VTHistoryLine()
        {
            Characters = new List<VTCharacter>();
        }
    }
}
