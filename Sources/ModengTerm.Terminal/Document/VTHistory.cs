using ModengTerm.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base;

namespace ModengTerm.Terminal.Document
{
    /// <summary>
    /// 对历史行数据进行管理（增删改查）
    /// </summary>
    public abstract class VTHistory
    {
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
        /// 增加一个历史行
        /// </summary>
        /// <param name="historyLine">要增加的历史行</param>
        /// <returns></returns>
        public abstract void AddHistory(VTHistoryLine historyLine);

        public abstract void RemoveHistory(VTHistoryLine historyLine);

        /// <summary>
        /// 更新一个历史行
        /// </summary>
        /// <param name="textLine">历史行对应的文本行</param>
        /// <param name="historyLine">要更新的历史行</param>
        public abstract void UpdateHistory(VTextLine textLine, VTHistoryLine historyLine);

        /// <summary>
        /// 获取指定的历史记录行
        /// </summary>
        /// <param name="physicsRow">要获取的历史记录行</param>
        /// <returns></returns>
        public abstract bool TryGetHistory(int physicsRow, out VTHistoryLine historyLine);
    }

    /// <summary>
    /// 所有的历史记录都存储在内存里
    /// </summary>
    public class VTMemoryHistory : VTHistory
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

        public override void AddHistory(VTHistoryLine historyLine)
        {
            this.historyLines[historyLine.PhysicsRow] = historyLine;
        }

        public override void UpdateHistory(VTextLine textLine, VTHistoryLine historyLine)
        {
            historyLine.PhysicsRow = textLine.PhysicsRow;
            VTUtils.CopyCharacter(textLine.Characters, historyLine.Characters);
        }

        public override void RemoveHistory(VTHistoryLine historyLine)
        {
            this.historyLines.Remove(historyLine.PhysicsRow);
        }
    }

    /// <summary>
    /// 使用内存映射文件实现的可以存储大容量历史记录
    /// </summary>
    public class VTFileMappingHistory : VTHistory
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

        public override void RemoveHistory(VTHistoryLine historyLine)
        {
            throw new NotImplementedException();
        }

        public override void AddHistory(VTHistoryLine historyLine)
        {
            throw new NotImplementedException();
        }

        public override void UpdateHistory(VTextLine textLine, VTHistoryLine historyLine)
        {
            throw new NotImplementedException();
        }
    }
}
