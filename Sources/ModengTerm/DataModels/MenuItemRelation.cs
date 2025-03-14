using ModengTerm.Base.Definitions;
using ModengTerm.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.DataModels
{
    public class MenuItemRelation
    {
        /// <summary>
        /// MenuDefinition的父节点
        /// </summary>
        public string ParentID { get; private set; }

        public MenuItemDefinition MenuDefinition { get; private set; }

        public MenuItemRelation(string parentID, MenuItemDefinition menuDefinition)
        {
            this.ParentID = parentID;
            this.MenuDefinition = menuDefinition;
        }
    }
}
