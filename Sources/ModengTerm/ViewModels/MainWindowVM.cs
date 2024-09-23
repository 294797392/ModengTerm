using ModengTerm.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels
{
    public class MainWindowVM : ViewModelBase
    {
        private bool shellCommandPanelVisibility;

        public bool ShellCommandPanelVisiblity
        {
            get { return this.shellCommandPanelVisibility; }
            set
            {
                if (this.shellCommandPanelVisibility != value)
                {
                    this.shellCommandPanelVisibility = value;
                    this.NotifyPropertyChanged("ShellCommandPanelVisiblity");
                }
            }
        }

        /// <summary>
        /// 右侧窗格列表
        /// </summary>
        public MenuVM RightPanelMenu { get; private set; }

        public MainWindowVM()
        {
            this.RightPanelMenu = new MenuVM();
            this.RightPanelMenu.Initialize(new List<MenuDefinition>()
            {
                new MenuDefinition()
                {
                    Name = "快捷命令",
                    ClassName = "ModengTerm.UserControls.Terminals.ShellCommandUserControl, ModengTerm",
                }
            });
            this.RightPanelMenu.SelectedMenu = this.RightPanelMenu.MenuItems.FirstOrDefault();
        }
    }
}
