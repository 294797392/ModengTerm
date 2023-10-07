using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Document
{
    /// <summary>
    /// ��ʾ�ı�Ԫ�����һ��λ��
    /// </summary>
    public class VTextPointer
    {
        /// <summary>
        /// ���е��е�������������0��ʼ
        /// ���û���У���ô����-1
        /// </summary>
        public int PhysicsRow { get; set; }

        /// <summary>
        /// ���е��ַ�����������0��ʼ
        /// ���û���У���ô����-1
        /// </summary>
        public int CharacterIndex { get; set; }

        public VTextPointer()
        { }

        public VTextPointer(int physicsRow, int characterIndex)
        {
            this.PhysicsRow = physicsRow;
            this.CharacterIndex = characterIndex;
        }
    }
}
