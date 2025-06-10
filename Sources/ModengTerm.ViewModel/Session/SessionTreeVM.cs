using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel.Session
{
    /// <summary>
    /// 会话列表树形列表
    /// </summary>
    public class SessionTreeVM : TreeViewModel<TreeViewModelContext>
    {
        #region 实例变量

        private IEnumerator<TreeNodeViewModel> enumerator;
        private string keyword;

        #endregion

        #region 属性

        public string Keyword
        {
            get { return keyword; }
            set
            {
                if (keyword != value)
                {
                    keyword = value;
                    NotifyPropertyChanged("Keyword");
                }
            }
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 
        /// </summary>
        /// <param name="match">搜索到的节点</param>
        /// <returns>是否搜索结束</returns>
        public bool PerformMatch(out TreeNodeViewModel match)
        {
            match = null;

            if (string.IsNullOrEmpty(Keyword))
            {
                return false;
            }

            if (enumerator == null)
            {
                enumerator = Context.NodeList.GetEnumerator();
            }

            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Name.Contains(Keyword))
                {
                    enumerator.Current.IsExpanded = true;
                    enumerator.Current.IsSelected = true;
                    match = enumerator.Current;
                    return false;
                }
            }

            enumerator.Reset();

            return true;
        }

        public void ResetMatch()
        {
            enumerator = null;
        }

        /// <summary>
        /// 删除选中的分组
        /// </summary>
        /// <returns>如果删除成功返回被删除的分组ViewModel，否则返回null</returns>
        public SessionGroupVM DeleteSelectedGroup()
        {
            SessionGroupVM selectedGroup = Context.SelectedItem as SessionGroupVM;

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

            throw new RefactorImplementedException();
            //int code = MTermApp.Context.ServiceAgent.DeleteSessionGroup(selectedGroup.ID.ToString());
            //if (code != ResponseCode.SUCCESS)
            //{
            //    MTMessageBox.Info("删除分组失败, {0}", code);
            //    return null;
            //}

            SessionGroupVM parentGroup = selectedGroup.Parent as SessionGroupVM;
            if (parentGroup != null)
            {
                parentGroup.Children.Remove(selectedGroup);
            }

            return selectedGroup;
        }

        #endregion
    }
}
