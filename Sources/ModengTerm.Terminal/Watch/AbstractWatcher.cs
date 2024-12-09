using ModengTerm.Base.DataModels;
using ModengTerm.Terminal.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Watch
{
    public abstract class AbstractWatcher
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="session">要watch的Session</param>
        /// <param name="driver">要watch的Driver</param>
        public AbstractWatcher(XTermSession session, SessionDriver driver)
        {
        }

        public abstract void Initialize();

        public abstract void Release();

        public abstract SystemInfo GetSystemInfo();

        /// <summary>
        /// 把source集合拷贝到target集合里
        /// </summary>
        /// <typeparam name="TSource">被拷贝的集合里的元素类型</typeparam>
        /// <typeparam name="TTarget">拷贝到的集合里的元素类型</typeparam>
        /// <param name="copyTo"></param>
        /// <param name="copyFrom"></param>
        /// <param name="copy"></param>
        protected void Copy<TSource, TTarget>(ChangedItems<TTarget> copyTo, IEnumerable<TSource> copyFrom, ObjectCopy<TTarget, TSource> copy)
            where TTarget : class, new()
        {
            IList<TTarget> items = copyTo.Items;
            IList<TTarget> addItems = copyTo.AddItems;
            IList<TTarget> removeItems = copyTo.RemoveItems;
            addItems.Clear();
            removeItems.Clear();

            // 判断是否需要新建项
            foreach (TSource source in copyFrom)
            {
                TTarget found = null;
                foreach (TTarget item in items)
                {
                    if (copy.Compare(item, source))
                    {
                        found = item;
                        break;
                    }
                }

                if (found == null)
                {
                    found = new TTarget();
                    items.Add(found);
                    addItems.Add(found);
                }

                copy.CopyTo(found, source);
            }

            // 判断是否需要删除项
            if (items.Count != copyFrom.Count())
            {
                // 查询每个DataModel是否存在于systemModels里

                foreach (TTarget target in items)
                {
                    bool exist = false;

                    foreach (TSource systemModel in copyFrom)
                    {
                        if (copy.Compare(target, systemModel))
                        {
                            exist = true;
                            break;
                        }
                    }

                    // DataModel不存在于SystemModels里，说明要删除
                    if (!exist)
                    {
                        removeItems.Add(target);
                    }
                }

                foreach (TTarget remove in removeItems)
                {
                    items.Remove(remove);
                }
            }
        }
    }
}
