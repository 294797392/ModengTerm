using DotNEToolkit;
using ModengTerm.Base;
using ModengTerm.Base.Enumerations;
using System.Collections.Generic;
using System.Linq;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels.CreateSession
{
    public class OptionMenuItemVM : MenuItemVM
    {
        private static readonly List<int> AllSessionTypes = VTBaseUtils.GetEnumValues<SessionTypeEnum>().Select(v => (int)v).ToList();

        public OptionMenuItemVM()
        {
        }

        /// <summary>
        /// 获取哪些会话类型使用这个选项
        /// </summary>
        public List<int> GetTargetTypes()
        {
            // 默认支持所有会话类型
            return this.Parameters.GetValue<List<int>>("sessionTypes", AllSessionTypes);
        }
    }
}
