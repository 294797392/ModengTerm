using ModengTerm.ViewModels.Session;
using ModengTerm.ViewModels.Sessions;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using WPFToolkit.MVVM;
using static ModengTerm.Windows.SessionListWindow;

namespace ModengTerm.UserControls.SessionListUserControls
{
    /// <summary>
    /// DataGridUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class DataGridUserControl : SessionListView
    {
        private GobackGroupVM gobackVM;
        private SessionListViewEnum listViewEnum;

        public DataGridUserControl()
        {
            InitializeComponent();

            this.InitializeUserControl();
        }

        private void InitializeUserControl()
        {
        }

        private void DataGridSessionList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SessionTreeNodeVM selectedItem = this.GetSelectedItem();
            if (selectedItem == null) 
            {
                return;
            }

            switch (selectedItem.NodeType)
            {
                case SessionTreeNodeTypeEnum.Session:
                    {
                        this.NotifyOpenSessionEvent(selectedItem as XTermSessionVM);
                        break;
                    }

                case SessionTreeNodeTypeEnum.Group:
                    {
                        this.SwitchGroup(selectedItem);
                        break;
                    }

                case SessionTreeNodeTypeEnum.GobackGroup:
                    {
                        this.CurrentGroup.Children.Remove(this.gobackVM);

                        SessionGroupVM targetGroup = this.CurrentGroup.Parent as SessionGroupVM;
                        if (targetGroup != null)
                        {
                            targetGroup.Children.Insert(0, this.gobackVM);
                            this.SetDataSource(targetGroup.Children);
                            this.CurrentGroup = targetGroup;
                        }
                        else
                        {
                            // 此时说明显示的是根节点
                            this.SetDataSource(this.SessionTreeVM.Roots);
                            this.CurrentGroup = null;
                        }

                        break;
                    }

                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        public override void OnLoad()
        {
            if (this.gobackVM == null)
            {
                this.gobackVM = new GobackGroupVM(this.SessionTreeVM.Context);
                this.gobackVM.ID = Guid.Empty;
                this.gobackVM.Name = "...";
            }

            if (this.CurrentGroup != null)
            {
                this.CurrentGroup.Children.Insert(0, this.gobackVM);
                this.SetDataSource(this.CurrentGroup.Children);
            }
            else
            {
                this.SetDataSource(this.SessionTreeVM.Roots);
            }
        }

        public override void OnUnload()
        {
            // 移除临时的返回上一级目录的节点
            if (this.CurrentGroup != null)
            {
                this.CurrentGroup.Children.Remove(this.gobackVM);
            }
        }

        /// <summary>
        /// 显示指定的分组列表下的内容
        /// </summary>
        /// <param name="groupVM">要显示的分组列表</param>
        private void SwitchGroup(SessionTreeNodeVM groupVM)
        {
            if (this.CurrentGroup != null)
            {
                this.CurrentGroup.Children.Remove(this.gobackVM);
            }

            this.SetDataSource(groupVM.Children);
            groupVM.Children.Insert(0, this.gobackVM);
            this.CurrentGroup = groupVM as SessionGroupVM;
        }

        private void SetDataSource(ObservableCollection<TreeNodeViewModel> itemsSource)
        {
            switch (this.listViewEnum)
            {
                case SessionListViewEnum.DataGrid: DataGridSessionList.ItemsSource = itemsSource; break;
                case SessionListViewEnum.DataList: ListBoxSessionList.ItemsSource = itemsSource; break;
                default:
                    throw new NotImplementedException();
            }
        }

        private SessionTreeNodeVM GetSelectedItem()
        {
            switch (this.listViewEnum)
            {
                case SessionListViewEnum.DataGrid: return DataGridSessionList.SelectedItem as SessionTreeNodeVM;
                case SessionListViewEnum.DataList: return ListBoxSessionList.SelectedItem as SessionTreeNodeVM;
                default:
                    throw new NotImplementedException();
            }
        }



        public void SetMode(SessionListViewEnum listViewEnum)
        {
            this.listViewEnum = listViewEnum;

            switch (listViewEnum)
            {
                case SessionListViewEnum.DataList:
                    {
                        DataGridSessionList.Visibility = System.Windows.Visibility.Collapsed;
                        ListBoxSessionList.Visibility = System.Windows.Visibility.Visible;
                        break;
                    }

                case SessionListViewEnum.DataGrid:
                    {
                        DataGridSessionList.Visibility = System.Windows.Visibility.Visible;
                        ListBoxSessionList.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
