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
        /// �����е��е���������
        /// </summary>
        public int PhysicsRow { get; set; }

        /// <summary>
        /// ���е��ַ�����������0��ʼ
        /// ���û���У���ô����-1
        /// </summary>
        public int CharacterIndex { get; set; }

        /// <summary>
        /// ���û�����ַ�������������ĳһ�У���ô�洢���е��е�����
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
