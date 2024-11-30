using ModengTerm.Base.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Watch
{
    public abstract class AbstractWatcher
    {
        public AbstractWatcher(XTermSession session) { }

        public abstract void Initialize();

        public abstract void Release();

        public abstract SystemInfo GetSystemInfo();


        private void UpdateItems<TSystemModel, TDataModel>(ChangedItems<TDataModel> toUpdate, IEnumerable<TSystemModel> systemModels, Sync<TDataModel, TSystemModel> sync)
            where TDataModel : UpdatableModel, new()
        {
            IList<TDataModel> items = toUpdate.Items;
            IList<TDataModel> addItems = toUpdate.AddItems;
            IList<TDataModel> removeItems = toUpdate.RemoveItems;
            addItems.Clear();
            removeItems.Clear();

            // 判断是否需要新建项
            foreach (TSystemModel systemModel in systemModels)
            {
                TDataModel found = null;

                bool exist = false;

                foreach (TDataModel dataModel in items)
                {
                    if (sync.Compare(dataModel, systemModel))
                    {
                        found = dataModel;
                        exist = true;
                        break;
                    }
                }

                if (!exist)
                {
                    found = new TDataModel();
                    items.Add(found);
                    addItems.Add(found);
                }

                sync.Update(found, systemModel);
            }

            // 判断是否需要删除项
            if (items.Count != systemModels.Count())
            {
                // 查询每个DataModel是否存在于systemModels里

                foreach (TDataModel dataModel in items)
                {
                    bool exist = false;

                    foreach (TSystemModel systemModel in systemModels)
                    {
                        if (sync.Compare(dataModel, systemModel))
                        {
                            exist = true;
                            break;
                        }
                    }

                    // DataModel不存在于SystemModels里，说明要删除
                    if (!exist)
                    {
                        removeItems.Add(dataModel);
                    }
                }

                foreach (TDataModel remove in removeItems)
                {
                    items.Remove(remove);
                }
            }
        }
    }
}
