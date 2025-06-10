using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.ViewModel.Terminal
{
    /// <summary>
    /// 封装自动完成数据源模块
    /// 对自动完成列表进行维护
    /// </summary>
    public class AutoCompletionSource
    {
        private List<string> items;
        private HashSet<string> strings;

        public AutoCompletionSource()
        {
            strings = new HashSet<string>();
            items = new List<string>();
        }

        /// <summary>
        /// 向自动完成列表里新建一个备选项目
        /// </summary>
        public void AddItem(string keyword)
        {
            strings.Add(keyword);
        }

        /// <summary>
        /// 从自动完成列表里删除一个备选项目
        /// </summary>
        /// <param name="keyword"></param>
        public void RemoveItem(string keyword)
        {
            strings.Remove(keyword);
        }

        /// <summary>
        /// 根据关键字获取要显示的自动完成列表
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <returns></returns>
        public List<string> SearchItems(string keyword)
        {
            items.Clear();
            items.AddRange(strings.Where(v => v.StartsWith(keyword)));
            return items;
        }
    }
}
