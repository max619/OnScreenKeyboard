using OnScreenKeyboard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OnScreenKeyboard.Helpers
{
    abstract class KeyboardButtonClickCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public IKeyboardInputContext Context { get; set; }

        public KeyboardButtonClickCommand(IKeyboardInputContext context)
        {
            if (context == null)
                throw new NullReferenceException();

            Context = context;
        }

        public bool CanExecute(object parameter)
        {
            return Context.IsFocused();
        }

        public void Execute(object parameter)
        {
            if (!(parameter is KeyboardButton))
                throw new InvalidOperationException("Command parameter is not KeyboardButton");

            OnButtonClicked((KeyboardButton)parameter);
        }

        protected abstract void OnButtonClicked(KeyboardButton button);
    }
}
