using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document
{
    /// <summary>
    /// 存储要输入的数据
    /// </summary>
    public class VTInputData
    {
        /// <summary>
        /// 大写锁定键是否打开
        /// </summary>
        public bool CapsLock { get; set; }

        /// <summary>
        /// 是否按下了Shift键
        /// </summary>
        public bool ShiftPressed { get; set; }

        /// <summary>
        /// 是否按下了Control键
        /// </summary>
        public bool CtrlPressed { get; set; }

        /// <summary>
        /// 输入的文本信息
        /// </summary>
        public string Text { get; set; }
    }

    public class InputEventImpl
    {
        private VTDocument document;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="document">要编辑的文档对象</param>
        public InputEventImpl(VTDocument document)
        {
            this.document = document;
        }

        /// <summary>
        /// 当有输入信息的时候调用
        /// </summary>
        /// <param name="vtInput"></param>
        public void HandleInput(VTInputData vtInput)
        {

        }
    }
}
