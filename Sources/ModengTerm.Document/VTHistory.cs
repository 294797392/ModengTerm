using ModengTerm.Document.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document
{
    /// <summary>
    /// 对历史行数据进行管理（增删改查）
    /// 历史记录的第一行的PhysicsRow永远是0，PhysicsRow依次往下排列
    /// </summary>
    public abstract class VTHistory
    {
        /// <summary>
        /// 历史记录的第一行
        /// </summary>
        public VTHistoryLine FirstLine { get; protected set; }

        /// <summary>
        /// 历史记录的最后一行
        /// </summary>
        public VTHistoryLine LastLine { get; protected set; }

        /// <summary>
        /// 一共多少航
        /// </summary>
        public int Lines { get; protected set; }

        /// <summary>
        /// 最多可以存储多少历史记录
        /// </summary>
        public int MaxHistory { get; set; }

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
        /// <param name="physicsRow">要获取的行索引</param>
        /// <returns></returns>
        public abstract bool TryGetHistory(int physicsRow, out VTHistoryLine historyLine);

        /// <summary>
        /// 获取指定的历史记录行
        /// </summary>
        /// <param name="startPhysicsRow">要获取的起始行（返回的行列表里包含该行）</param>
        /// <param name="endPhysicsRow">要获取的结束行（返回的行列表里包含该行）</param>
        /// <param name="historyLines">获取的行列表</param>
        /// <returns>是否获取成功</returns>
        public abstract bool TryGetHistories(int startPhysicsRow, int endPhysicsRow, out IEnumerable<VTHistoryLine> historyLines);

        /// <summary>
        /// 获取所有的历史记录
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<VTHistoryLine> GetAllHistoryLines();
    }

    /// <summary>
    /// 所有的历史记录都存储在内存里
    /// </summary>
    public class VTMemoryHistory : VTHistory
    {
        private List<VTHistoryLine> historyLines;

        public override bool TryGetHistory(int rowIndex, out VTHistoryLine historyLine)
        {
            historyLine = null;

            if (rowIndex >= this.historyLines.Count)
            {
                return false;
            }

            historyLine = this.historyLines[rowIndex];

            return true;
        }

        public override bool TryGetHistories(int startRowIndex, int endRowIndex, out IEnumerable<VTHistoryLine> historyLines)
        {
            historyLines = null;

            int count = endRowIndex - startRowIndex + 1;

            historyLines = this.historyLines.Skip(startRowIndex).Take(count);

            return true;
        }

        public override int Initialize()
        {
            historyLines = new List<VTHistoryLine>();

            return 0;
        }

        public override void Release()
        {
            historyLines.Clear();
        }

        public override void AddHistory(VTHistoryLine historyLine)
        {
            // 最多可以保存0行历史记录，什么都不做
            if (this.MaxHistory == 0)
            {
                return;
            }

            if (this.FirstLine == null)
            {
                this.FirstLine = historyLine;
            }

            if (this.historyLines.Count == this.MaxHistory)
            {
                this.historyLines.RemoveAt(0);
                this.FirstLine = this.historyLines[0];
            }

            this.historyLines.Add(historyLine);

            this.LastLine = historyLine;

            this.Lines = this.historyLines.Count;
        }

        public override void UpdateHistory(VTextLine textLine, VTHistoryLine historyLine)
        {
        }

        public override void RemoveHistory(VTHistoryLine historyLine)
        {
            historyLines.Remove(historyLine);
        }

        public override IEnumerable<VTHistoryLine> GetAllHistoryLines()
        {
            return this.historyLines;
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

        public override bool TryGetHistories(int startRowIndex, int endRowIndex, out IEnumerable<VTHistoryLine> historyLines)
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

        public override IEnumerable<VTHistoryLine> GetAllHistoryLines()
        {
            throw new NotImplementedException();
        }
    }
}
