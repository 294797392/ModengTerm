using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Document.Rendering;

namespace XTerminal.Document
{
    /// <summary>
    /// �洢���ѡ�е��ı���Ϣ
    /// </summary>
    public class VTextSelection : VTDocumentElement
    {
        public override VTDocumentElements Type => VTDocumentElements.SelectionRange;

        /// <summary>
        /// ѡ�е��ı���Χ
        /// </summary>
        public List<VTRect> Ranges { get; private set; }

        /// <summary>
        /// ��ѡ���ݵĿ�ʼλ��
        /// </summary>
        public VTextPointer Start { get; private set; }

        /// <summary>
        /// ��ѡ���ݵĽ���λ��
        /// </summary>
        public VTextPointer End { get; private set; }

        public VTextSelection()
        {
            this.Ranges = new List<VTRect>();
            this.Start = new VTextPointer();
            this.End = new VTextPointer();
        }

        /// <summary>
        /// ����ѡ�е�״̬
        /// </summary>
        public void Reset()
        {
            this.Ranges.Clear();

            this.Start.IsCharacterHit = false;
            this.Start.CharacterIndex = -1;
            this.Start.Line = null;

            this.End.IsCharacterHit = false;
            this.End.CharacterIndex = -1;
            this.End.Line = null;
        }
    }
}
