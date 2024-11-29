using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModengTerm.ViewModels.Terminals;

namespace ModengTerm.ViewModels
{
    public class PanelContent : IContextMenuData
    {
        /// <summary>
        /// 该菜单所关联的Panel分组
        /// </summary>
        public string Group { get; private set; }

        /// <summary>
        /// 该菜单所关联的Panel类型
        /// </summary>
        public PanelContentTypeEnum Type { get; private set; }

        /// <summary>
        /// 界面入口类名
        /// </summary>
        public string EntryClass { get; private set; }

        /// <summary>
        /// Panel所属的
        /// </summary>
        public PanelVM OwnerPanel { get; set; }

        public PanelContent(string groupName, string entryClass, PanelContentTypeEnum type)
        {
            this.Group = groupName;
            this.EntryClass = entryClass;
            this.Type = type;
        }
    }
}
