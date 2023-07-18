using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document
{
    /// <summary>
    /// ��ʾ�ı�Ԫ�����һ��λ��
    /// </summary>
    public class VTextPointer
    {
        /// <summary>
        /// ָ�����е�����
        /// </summary>
        public int Row { get { return this.LineHit.Row; } }

        /// <summary>
        /// ���������
        /// </summary>
        public VTHistoryLine LineHit { get; set; }

        /// <summary>
        /// ��������е��ַ��ı߽����Ϣ
        /// </summary>
        public VTRect CharacterBounds { get; set; }

        /// <summary>
        /// ���е��ַ�������
        /// ���û���У���ô����-1
        /// </summary>
        public int CharacterIndex { get; set; }
    }
}
