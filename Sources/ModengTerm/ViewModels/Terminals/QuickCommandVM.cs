using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations.Terminal;
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
    public class QuickCommandVM : ItemViewModel
    {
        private string command;
        private CommandTypeEnum type;
        private string sessionId;

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

        public CommandTypeEnum Type
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

        /// <summary>
        /// 快捷命令所关联的会话
        /// </summary>
        public string SessionId 
        {
            get { return this.sessionId; }
            set
            {
                if (this.sessionId != value)
                {
                    this.sessionId = value;
                    this.NotifyPropertyChanged("SessionId");
                }
            }
        }

        public QuickCommandVM()
        {
        }

        public QuickCommandVM(ShellCommand command)
        {
            this.ID = command.ID;
            this.Name = command.Name;
            this.Description = command.Description;
            this.Command = command.Command;
            this.Type = (CommandTypeEnum)command.Type;
            this.AutoCRLF = command.AutoCRLF;
            this.SessionId = command.SessionId;
        }

        public ShellCommand GetShellCommand()
        {
            return new ShellCommand()
            {
                ID = this.ID.ToString(),
                Command = this.Command,
                Name = this.Name,
                Type = (int)this.Type,
                AutoCRLF = this.AutoCRLF,
                SessionId = this.SessionId
            };
        }
    }
}
