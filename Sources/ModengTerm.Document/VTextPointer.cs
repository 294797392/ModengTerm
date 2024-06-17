using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document
{
    /// <summary>
    /// 表示文本元素里的一个位置
    /// </summary>
    public class VTextPointer
    {
        /// <summary>
        /// 所命中的行的物理行数
        /// </summary>
        public int PhysicsRow { get; set; }

        /// <summary>
        /// 命中的字符的索引，从0开始
        /// 如果没命中，那么就是-1
        /// </summary>
        public int CharacterIndex { get; set; }

        /// <summary>
        /// 如果没命中字符，但是命中了某一列，那么存储命中的列的索引
        /// </summary>
        public int ColumnIndex { get; set; }

        public VTextPointer()
        {
        }

        public VTextPointer(int physicsRow, int characterIndex, int columnIndex)
        {
            this.PhysicsRow = physicsRow;
            this.CharacterIndex = characterIndex;
        }
    }
}
