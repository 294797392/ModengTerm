using ModengTerm.Terminal.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModengTerm.Terminal
{
    /// <summary>
    /// 剪贴板历史记录功能
    /// </summary>
    public class VTClipboard
    {
        #region 实例变量

        /// <summary>
        /// 最后一次保存的剪贴板数据
        /// </summary>
        private VTParagraph lastData;

        /// <summary>
        /// 存储剪贴板历史记录
        /// </summary>
        private List<VTParagraph> historyList;

        #endregion

        #region 属性

        /// <summary>
        /// 最多保存多少条历史记录
        /// </summary>
        public int MaximumHistory { get; set; }

        /// <summary>
        /// 剪贴板历史记录
        /// </summary>
        public List<VTParagraph> HistoryList { get { return this.historyList; } }

        #endregion

        #region 构造方法

        public VTClipboard()
        {
            this.historyList = new List<VTParagraph>();
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 设置剪贴板内容
        /// </summary>
        /// <param name="data">要设置的内容</param>
        public void SetData(VTParagraph data)
        {
            if (this.historyList.Count >= this.MaximumHistory)
            {
                // 移除最后一个元素
                this.historyList.RemoveAt(this.historyList.Count - 1);
            }

            // 最新的历史记录放到最上面
            this.historyList.Insert(0, data);

            this.lastData = data;
        }

        /// <summary>
        /// 获取最后一次设置的剪贴板内容
        /// </summary>
        /// <returns>最后一次设置的剪贴板内容</returns>
        public VTParagraph GetData()
        {
            return this.lastData;
        }

        /// <summary>
        /// 清除剪贴板历史数据
        /// </summary>
        public void ClearHistory()
        {

        }

        /// <summary>
        /// 释放剪贴板占用的资源
        /// </summary>
        public void Release()
        {
            this.ClearHistory();
        }

        #endregion
    }
}
