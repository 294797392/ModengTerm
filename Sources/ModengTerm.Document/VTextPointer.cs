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
        /// 命中的文本行
        /// </summary>
        public VTextLine TextLine { get; set; }

        /// <summary>
        /// 命中的字符的索引，从0开始
        /// 如果没命中，那么就是-1
        /// </summary>
        public int CharacterIndex { get; set; }

        /// <summary>
        /// 所命中的行的物理行数
        /// </summary>
        public int PhysicsRow { get; set; }

        public VTextPointer()
        { }

        public VTextPointer(int physicsRow, int characterIndex)
        {
            this.PhysicsRow = physicsRow;
            this.CharacterIndex = characterIndex;
        }

        public VTextPointer(VTextLine textLine, int physicsRow, int characterIndex)
        {
            this.TextLine = textLine;
            this.PhysicsRow = physicsRow;
            this.CharacterIndex = characterIndex;
        }
    }
}
