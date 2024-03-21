using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document
{
    /// <summary>
    /// ��ʾ�ı�Ԫ�����һ��λ��
    /// </summary>
    public class VTextPointer
    {
        /// <summary>
        /// ���е��ı���
        /// </summary>
        public VTextLine TextLine { get; set; }

        /// <summary>
        /// ���е��ַ�����������0��ʼ
        /// ���û���У���ô����-1
        /// </summary>
        public int CharacterIndex { get; set; }

        /// <summary>
        /// �����е��е���������
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
