using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OnScreenKeyboard.Helpers
{
    class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public Action OnClickAction { get; private set; }

        public RelayCommand(Action action)
        {
            OnClickAction = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            OnClickAction();
        }
    }
}
