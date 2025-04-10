﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModengTerm.ViewModels;
using WPFToolkit.MVVM;

namespace ModengTerm.Addons.QuickInput
{
    public class QuickInputVM : SessionPanelContentVM
    {
        #region 属性

        public BindableCollection<CommandVM> CommandList { get; private set; }

        #endregion

        #region 公开接口

        public void ReloadCommandList() 
        {
            this.CommandList.Clear();
            this.CommandList.AddRange(this.ServiceAgent.GetShellCommands(this.Session.ID).Select(v => new CommandVM(v)));
        }

        #endregion

        #region SessionPanelContentVM

        public override void OnInitialize()
        {
            base.OnInitialize();

            this.CommandList = new BindableCollection<CommandVM>();
            this.CommandList.AddRange(this.ServiceAgent.GetShellCommands(this.Session.ID).Select(v => new CommandVM(v)));
        }

        public override void OnRelease()
        {
            this.CommandList.Clear();

            base.OnRelease();
        }

        public override void OnReady()
        {
        }

        #endregion
    }
}
