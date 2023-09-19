using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base;
using XTerminal.Document;

namespace ModengTerm.Terminal.Document
{
    /// <summary>
    /// 管理历史记录
    /// </summary>
    public abstract class VTScrollback
    {
        public VTHistoryLine FirstLine { get; private set; }

        public VTHistoryLine LastLine { get; private set; }

        /// <summary>
        /// 更新文本行对应的历史记录
        /// 如果不存在历史记录，那么创建历史记录
        /// </summary>
        /// <param name="textLine"></param>
        public void UpdateHistory(VTextLine textLine)
        {
            VTHistoryLine historyLine;
            if (!this.TryGetHistory(textLine.PhysicsRow, out historyLine))
            {
                historyLine = this.CreateHistory(textLine);

                if (textLine.PhysicsRow == 0)
                {
                    this.FirstLine = historyLine;
                    this.LastLine = historyLine;
                }
                else if (textLine.PhysicsRow > this.LastLine.PhysicsRow)
                {
                    this.LastLine = historyLine;
                }
            }
            else
            {
                this.UpdateHistory(textLine, historyLine);
            }
        }

        /// <summary>
        /// 从指定行开始获取指定的行数
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool TryGetHistories(int startRow, int count, out List<VTHistoryLine> historyLines)
        {
            historyLines = new List<VTHistoryLine>();

            for (int i = 0; i < count; i++)
            {
                VTHistoryLine historyLine;
                if (!this.TryGetHistory(startRow + i, out historyLine))
                {
                    return false;
                }

                historyLines.Add(historyLine);
            }

            return true;
        }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public abstract int Initialize();

        /// <summary>
        /// 释放资源
        /// </summary>
        public abstract void Release();

        /// <summary>
        /// 获取指定的历史记录行
        /// </summary>
        /// <param name="physicsRow">要获取的历史记录行</param>
        /// <returns></returns>
        public abstract bool TryGetHistory(int physicsRow, out VTHistoryLine historyLine);

        /// <summary>
        /// 创建一个新行
        /// </summary>
        /// <param name="textLine">历史行对应的文本行</param>
        /// <returns></returns>
        protected abstract VTHistoryLine CreateHistory(VTextLine textLine);

        protected abstract void UpdateHistory(VTextLine textLine, VTHistoryLine historyLine);
    }

    /// <summary>
    /// 所有的历史记录都存储在内存里
    /// </summary>
    public class VTMemoryScrollback : VTScrollback
    {
        private Dictionary<int, VTHistoryLine> historyLines;

        public override bool TryGetHistory(int physicsRow, out VTHistoryLine historyLine)
        {
            return this.historyLines.TryGetValue(physicsRow, out historyLine);
        }

        public override int Initialize()
        {
            this.historyLines = new Dictionary<int, VTHistoryLine>();

            return ResponseCode.SUCCESS;
        }

        public override void Release()
        {
            this.historyLines.Clear();
        }

        protected override VTHistoryLine CreateHistory(VTextLine textLine)
        {
            VTHistoryLine historyLine = VTHistoryLine.Create(textLine);
            this.historyLines[textLine.PhysicsRow] = historyLine;
            return historyLine;
        }

        protected override void UpdateHistory(VTextLine textLine, VTHistoryLine historyLine)
        {
            historyLine.PhysicsRow = textLine.PhysicsRow;
            VTUtils.CopyCharacter(textLine.Characters, historyLine.Characters);
        }
    }

    /// <summary>
    /// 使用内存映射文件实现的可以存储大容量历史记录
    /// </summary>
    public class VTFileMappingScrollback : VTScrollback
    {
        public override int Initialize()
        {
            throw new NotImplementedException();
        }

        public override void Release()
        {
            throw new NotImplementedException();
        }

        public override bool TryGetHistory(int physicsRow, out VTHistoryLine historyLine)
        {
            throw new NotImplementedException();
        }

        protected override VTHistoryLine CreateHistory(VTextLine textLine)
        {
            throw new NotImplementedException();
        }

        protected override void UpdateHistory(VTextLine textLine, VTHistoryLine historyLine)
        {
            throw new NotImplementedException();
        }
    }
}
