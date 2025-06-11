using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations.Terminal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.Addon.Official.QuickInput
{
    /// <summary>
    /// 快捷命令ViewModel
    /// </summary>
    public class CommandVM : ItemViewModel
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
                return command;
            }
            set
            {
                if (command != value)
                {
                    command = value;
                    NotifyPropertyChanged("Command");
                }
            }
        }

        public CommandTypeEnum Type
        {
            get
            {
                return type;
            }
            set
            {
                if (type != value)
                {
                    type = value;
                    NotifyPropertyChanged("Type");
                }
            }
        }

        /// <summary>
        /// 快捷命令所关联的会话
        /// </summary>
        public string SessionId
        {
            get { return sessionId; }
            set
            {
                if (sessionId != value)
                {
                    sessionId = value;
                    NotifyPropertyChanged("SessionId");
                }
            }
        }

        public CommandVM()
        {
        }

        public CommandVM(ShellCommand command)
        {
            ID = command.ID;
            Name = command.Name;
            Description = command.Description;
            Command = command.Command;
            Type = (CommandTypeEnum)command.Type;
            SessionId = command.SessionId;
        }

        public ShellCommand GetShellCommand()
        {
            return new ShellCommand()
            {
                ID = ID.ToString(),
                Command = Command,
                Name = Name,
                Type = (int)Type,
                SessionId = SessionId
            };
        }
    }
}
