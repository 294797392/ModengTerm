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
        /// ���е��е�������������0��ʼ
        /// ���û���У���ô����-1
        /// </summary>
        public int PhysicsRow { get; set; }

        ///// <summary>
        ///// ���е��������Surface��Yƫ����
        ///// </summary>
        //public double OffsetY { get; set; }

        /// <summary>
        /// ���е��ַ�����������0��ʼ
        /// ���û���У���ô����-1
        /// </summary>
        public int CharacterIndex { get; set; }

        ///// <summary>
        ///// ���е��ַ��ı߽����Ϣ
        ///// ע��Yֵ��0����Ϊ������ڸ��еı߽��
        ///// </summary>
        //public VTRect CharacterBounds { get; set; }
    }
}
