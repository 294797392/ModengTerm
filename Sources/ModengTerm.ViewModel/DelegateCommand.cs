using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModengTerm.ViewModels
{
    public class DelegateCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (this.canExecute != null) 
            {
                return this.canExecute(parameter);
            }

            return true;
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }

        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }
    }
}
