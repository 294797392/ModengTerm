using ModengTerm.Base.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon.Controls
{
    /// <summary>
    /// 定义预选项Panel要实现的接口
    /// </summary>
    public interface IPreferencePanel
    {
        /// <summary>
        /// 获取预选项
        /// </summary>
        /// <returns></returns>
        Dictionary<string, object> GetOptions();

        /// <summary>
        /// 设置预选项
        /// </summary>
        /// <param name="options"></param>
        void SetOptions(Dictionary<string, object> options);
    }
}
