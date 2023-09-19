using DotNEToolkit;
using ModengTerm.Terminal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base;

namespace XTerminal.Document
{
    /// <summary>
    /// 用来存储历史的行记录
    /// </summary>
    public class VTHistoryLine
    {
        /// <summary>
        /// 行索引，从0开始
        /// </summary>
        public int PhysicsRow { get; set; }

        /// <summary>
        /// 该行的所有字符
        /// 显示历史行的时候用到
        /// </summary>
        public List<VTCharacter> Characters { get; private set; }

        private VTHistoryLine()
        {
            this.Characters = new List<VTCharacter>();
        }

        ///// <summary>
        ///// 设置该历史行的数据
        ///// </summary>
        ///// <param name="textLine">要设置的行</param>
        //public void SetVTextLine(VTextLine textLine)
        //{
        //    this.PhysicsRow = textLine.PhysicsRow;
        //    // 复制一份字符列表
        //    VTUtils.CopyCharacter(textLine.Characters, this.Characters);
        //}

        public static VTHistoryLine Create(VTextLine textLine)
        {
            VTHistoryLine historyLine = new VTHistoryLine();
            historyLine.PhysicsRow = textLine.PhysicsRow;
            VTUtils.CopyCharacter(textLine.Characters, historyLine.Characters);
            return historyLine;
        }
    }
}
