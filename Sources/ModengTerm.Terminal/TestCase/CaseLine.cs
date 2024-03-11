using ModengTerm.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.TestCase
{
    /// <summary>
    /// 存储测试用例需要用到的一行数据
    /// </summary>
    public class CaseLine
    {
        /// <summary>
        /// 行里的所有字符
        /// </summary>
        public List<VTCharacter> Characters { get; set; }

        /// <summary>
        /// 物理行号
        /// </summary>
        public int PhysicsRow { get; set; }

        public CaseLine() 
        {
            this.Characters = new List<VTCharacter>();
        }
    }
}
