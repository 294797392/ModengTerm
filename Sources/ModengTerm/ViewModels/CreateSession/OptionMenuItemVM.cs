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
        public OptionMenuItemVM()
        {
        }

        /// <summary>
        /// 获取哪些会话类型使用这个选项
        /// 如果没有指定TargetTypes，那么默认支持所有类型的会话
        /// </summary>
        public List<SessionTypeEnum> GetSupportedSessionTypes()
        {
            // 默认支持所有会话类型
            return this.Parameters.GetValue<List<int>>("sessionTypes", new List<int>()).Cast<SessionTypeEnum>().ToList();
        }
    }
}
