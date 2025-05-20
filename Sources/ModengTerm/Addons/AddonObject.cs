using ModengTerm.Terminal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addons
{
    /// <summary>
    /// 用来存储插件的自定义对象
    /// 每个session，每个插件对应一个对象
    /// 假设一个session有5个插件，那么会创建5个IAddonObject实例
    /// </summary>
    public abstract class AddonObject
    {
    }
}
