using ModengTerm.Document.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document
{
    /// <summary>
    /// 对历史行数据进行管理（增删改查）
    /// 历史记录的第一行的PhysicsRow永远是0，PhysicsRow依次往下排列
    /// </summary>
    public class VTHistory
    {
        #region 实例变量

        private VTHistoryList historyList;

        #endregion

        #region 属性

        /// <summary>
        /// 历史记录的第一行
        /// </summary>
        public VTHistoryLine FirstLine { get; protected set; }

        /// <summary>
        /// 历史记录的最后一行
        /// </summary>
        public VTHistoryLine LastLine { get; protected set; }

        /// <summary>
        /// 一共多少行
        /// </summary>
        public int Lines { get; protected set; }

        /// <summary>
        /// 最多可以存储多少历史记录
        /// </summary>
        public int MaxHistory { get; set; }

        #endregion

        #region 公开接口

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public int Initialize() 
        {
            this.historyList = new VTMemoryHistoryList();
            this.historyList.Initialize();

            return 0;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Release()
        {
            this.historyList.Release();
        }

        /// <summary>
        /// 根据物理行号移除一行
        /// </summary>
        /// <param name="physicsRow">要移除的物理行号</param>
        /// <returns></returns>
        public void RemoveAt(int physicsRow)
        {
            // 要删除的物理行号比总行数多，无法删除
            if (physicsRow >= this.Lines)
            {
                return;
            }

            this.historyList.RemoveAt(physicsRow);

            if (physicsRow == 0)
            {
                // 删除的是第一行
                VTHistoryLine newFirstLine;
                this.TryGetHistory(physicsRow, out newFirstLine);
                this.FirstLine = newFirstLine;
            }
            else if (physicsRow == this.Lines - 1)
            {
                // 删除的是最后一行
                VTHistoryLine newLastLine;
                this.TryGetHistory(physicsRow, out newLastLine);
                this.LastLine = newLastLine;
            }
            else
            {
                // 删除的是第一行和最后一行之间的行，不需要做特殊操作
            }

            this.Lines--;
        }

        /// <summary>
        /// 增加一个历史行
        /// </summary>
        /// <param name="historyLine">要增加的历史行</param>
        /// <returns></returns>
        public void Add(VTHistoryLine historyLine)
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

            if (this.Lines == this.MaxHistory)
            {
                this.RemoveAt(0);
            }

            this.historyList.Add(historyLine);

            this.LastLine = historyLine;

            this.Lines++;
        }

        /// <summary>
        /// 获取指定的历史记录行
        /// </summary>
        /// <param name="physicsRow">要获取的行索引</param>
        /// <returns></returns>
        public bool TryGetHistory(int physicsRow, out VTHistoryLine historyLine)
        {
            historyLine = null;

            if (physicsRow < 0)
            {
                return false;
            }

            if (physicsRow >= this.Lines)
            {
                return false;
            }

            historyLine = this.historyList.ElementAt(physicsRow);

            return true;
        }

        /// <summary>
        /// 获取指定的历史记录行
        /// </summary>
        /// <param name="startPhysicsRow">要获取的起始行（返回的行列表里包含该行）</param>
        /// <param name="endPhysicsRow">要获取的结束行（返回的行列表里包含该行）</param>
        /// <param name="historyLines">获取的行列表</param>
        /// <returns>是否获取成功</returns>
        public bool TryGetHistories(int startPhysicsRow, int endPhysicsRow, out IEnumerable<VTHistoryLine> historyLines)
        {
            historyLines = null;

            if (this.Lines == 0) 
            {
                return false;
            }

            if (startPhysicsRow < 0) 
            {
                return false;
            }

            if (endPhysicsRow >= this.Lines)
            {
                return false;
            }

            historyLines = this.historyList.ElementsAt(startPhysicsRow, endPhysicsRow);

            return true;
        }

        #endregion
    }

    /// <summary>
    /// 维护历史记录列表
    /// </summary>
    public abstract class VTHistoryList
    {
        public abstract int Initialize();

        public abstract void Release();

        public abstract void Add(VTHistoryLine historyLine);

        public abstract void RemoveAt(int physicsRow);

        /// <summary>
        /// 获取指定的历史记录行
        /// </summary>
        /// <param name="physicsRow">要获取的行索引</param>
        /// <returns></returns>
        public abstract VTHistoryLine ElementAt(int physicsRow);

        /// <summary>
        /// 获取指定的历史记录行
        /// </summary>
        /// <param name="startPhysicsRow">要获取的起始行（返回的行列表里包含该行）</param>
        /// <param name="endPhysicsRow">要获取的结束行（返回的行列表里包含该行）</param>
        /// <param name="historyLines">获取的行列表</param>
        /// <returns>是否获取成功</returns>
        public abstract IEnumerable<VTHistoryLine> ElementsAt(int startPhysicsRow, int endPhysicsRow);
    }

    /// <summary>
    /// 所有的历史记录都存储在内存里
    /// </summary>
    public class VTMemoryHistoryList : VTHistoryList
    {
        private List<VTHistoryLine> historyLines;

        public override int Initialize()
        {
            this.historyLines = new List<VTHistoryLine>();

            return 0;
        }

        public override void Release()
        {
            historyLines.Clear();
        }

        public override void Add(VTHistoryLine historyLine)
        {
            this.historyLines.Add(historyLine);
        }

        public override void RemoveAt(int physicsRow)
        {
            this.historyLines.RemoveAt(physicsRow);
        }

        public override VTHistoryLine ElementAt(int rowIndex)
        {
            return this.historyLines[rowIndex];
        }

        public override IEnumerable<VTHistoryLine> ElementsAt(int startPhysicsRow, int endPhysicsRow)
        {
            int count = endPhysicsRow - startPhysicsRow + 1;

            return this.historyLines.Skip(startPhysicsRow).Take(count);
        }
    }

    ///// <summary>
    ///// 使用内存映射文件实现的可以存储大容量历史记录
    ///// </summary>
    //public class VTFileMappingHistory : VTHistory
    //{
    //    public override int Initialize()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override void Release()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override bool Contains(int physicsRow)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override bool TryGetHistory(int physicsRow, out VTHistoryLine historyLine)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override bool TryGetHistories(int startRowIndex, int endRowIndex, out IEnumerable<VTHistoryLine> historyLines)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override void AddHistory(VTHistoryLine historyLine)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override IEnumerable<VTHistoryLine> GetAllHistoryLines()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
