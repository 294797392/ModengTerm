using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels.Terminals
{
    /// <summary>
    /// 快捷命令ViewModel
    /// </summary>
    public class ShellCommandVM : ItemViewModel
    {
        private string command;
        private ShellCommandTypeEnum type;

        /// <summary>
        /// 要执行的命令
        /// </summary>
        public string Command 
        {
            get
            {
                return this.command;
            }
            set
            {
                if (this.command != value)
                {
                    this.command = value;
                    this.NotifyPropertyChanged("Command");
                }
            }
        }

        public ShellCommandTypeEnum Type
        {
            get
            {
                return this.type;
            }
            set
            {
                if (this.type != value)
                {
                    this.type = value;
                    this.NotifyPropertyChanged("Type");
                }
            }
        }

        /// <summary>
        /// 是否自动输入回车键
        /// </summary>
        public bool AutoCRLF { get; set; }

        public ShellCommandVM(ShellCommand command)
        {
            this.ID = command.ID;
            this.Name = command.Name;
            this.Description = command.Description;
            this.Command = command.Command;
            this.Type = (ShellCommandTypeEnum)command.Type;
        }
    }
}
