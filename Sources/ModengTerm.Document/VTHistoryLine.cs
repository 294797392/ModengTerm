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
        /// TODO：考虑删掉，如果在编辑文档的时候，删除了一行，那么所有的HistoryLine都要更新，这会是一个灾难
        /// 解决方法是在VTDocument里保存当前显示的第一行的索引
        /// </summary>
        //public int PhysicsRow { get; set; }

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
