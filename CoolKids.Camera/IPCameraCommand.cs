using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CoolKids.Common.IPCamera
{
    public class IPCameraCommand : ICommand
    {
        readonly Action<object> execute;
        readonly Predicate<object> canExecute;


        private IPCameraCommand() {}

        public IPCameraCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        public IPCameraCommand(Action<object> execute, Predicate<object> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }


        public bool CanExecute(object parameter)
        {
            return canExecute == null ? true : canExecute(parameter);
        }


        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null) CanExecuteChanged(this, EventArgs.Empty);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }
}
