using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Hooks.AutoCompleted
{
    /// <summary>
    /// 封装自动完成数据源模块
    /// 对自动完成列表进行维护
    /// </summary>
    public class AutoCompletedSource
    {
        private HashSet<string> strings;

        public AutoCompletedSource() 
        {
            this.strings = new HashSet<string>();
        }

        /// <summary>
        /// 向自动完成列表里新建一个备选项目
        /// </summary>
        public void AddItem(string keyword)
        {
            this.strings.Add(keyword);
        }

        /// <summary>
        /// 从自动完成列表里删除一个备选项目
        /// </summary>
        /// <param name="keyword"></param>
        public void RemoveItem(string keyword) 
        {
            this.strings.Remove(keyword);
        }

        /// <summary>
        /// 根据关键字获取要显示的自动完成列表
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <returns></returns>
        public IEnumerable<string> GetItems(string keyword)
        {
            return strings.Where(v => v.StartsWith(keyword));
        }
    }
}
