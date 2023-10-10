using ModengTerm.Base.DataModels;
using ModengTerm.Terminal.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal
{
    /// <summary>
    /// 提供访问终端服务的代理
    /// </summary>
    public interface ITerminalAgent
    {
        #region 收藏夹管理

        List<Favorites> GetFavorites(string sessionId);

        int AddFavorites(Favorites favorites);

        int DeleteFavorites(Favorites favorites);

        #endregion
    }
}
