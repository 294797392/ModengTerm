using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels.Session
{
    /// <summary>
    /// 会话列表树形列表
    /// </summary>
    public class SessionTreeVM : TreeViewModel<TreeViewModelContext>
    {
        private IEnumerator<TreeNodeViewModel> enumerator;
        private string keyword;

        public string Keyword
        {
            get { return this.keyword; }
            set
            {
                if (this.keyword != value)
                {
                    this.keyword = value;
                    this.NotifyPropertyChanged("Keyword");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>是否搜索结束</returns>
        public bool PerformMatch()
        {
            if (string.IsNullOrEmpty(this.Keyword))
            {
                return false;
            }

            if (this.enumerator == null)
            {
                this.enumerator = this.Context.NodeList.GetEnumerator();
            }

            while (this.enumerator.MoveNext())
            {
                if (this.enumerator.Current.Name.Contains(this.Keyword))
                {
                    this.enumerator.Current.IsExpanded = true;
                    this.enumerator.Current.IsSelected = true;
                    return false;
                }
            }

            this.enumerator.Reset();

            return true;
        }

        public void ResetMatch()
        {
            this.enumerator = null;
        }

        /// <summary>
        /// 删除选中的分组
        /// </summary>
        /// <returns>如果删除成功返回被删除的分组ViewModel，否则返回null</returns>
        public SessionGroupVM DeleteSelectedGroup() 
        {
            SessionGroupVM selectedGroup = this.Context.SelectedItem as SessionGroupVM;

            if (selectedGroup == null)
            {
                MTMessageBox.Info("请选择要删除的分组");
                return null;
            }

            if (selectedGroup.IsRoot)
            {
                MTMessageBox.Info("根节点无法删除");
                return null;
            }

            // 判断分组下是否有子分组
            if (selectedGroup.Children.Count > 0)
            {
                MTMessageBox.Info("分组下存在子分组或者会话");
                return null;
            }

            if (!MTMessageBox.Confirm("确定删除{0}吗?", selectedGroup.Name))
            {
                return null;
            }

            int code = MTermApp.Context.ServiceAgent.DeleteSessionGroup(selectedGroup.ID.ToString());
            if (code != ResponseCode.SUCCESS)
            {
                MTMessageBox.Info("删除分组失败, {0}", code);
                return null;
            }

            SessionGroupVM parentGroup = selectedGroup.Parent as SessionGroupVM;
            if (parentGroup != null)
            {
                parentGroup.Children.Remove(selectedGroup);
            }

            return selectedGroup;
        }
    }
}
