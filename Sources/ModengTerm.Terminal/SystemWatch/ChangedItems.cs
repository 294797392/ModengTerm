using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Watch
{
    /// <summary>
    /// 存储集合被修改之后的数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ChangedItems<T>
    {
        /// <summary>
        /// 所有数据项
        /// </summary>
        public List<T> Items { get; private set; }

        /// <summary>
        /// 新建的数据项
        /// </summary>
        public List<T> AddItems { get; private set; }

        /// <summary>
        /// 被删除的数据项
        /// </summary>
        public List<T> RemoveItems { get; private set; }

        public ChangedItems() 
        {
            this.Items = new List<T>();
            this.AddItems = new List<T>();
            this.RemoveItems = new List<T>();
        }
    }
}
