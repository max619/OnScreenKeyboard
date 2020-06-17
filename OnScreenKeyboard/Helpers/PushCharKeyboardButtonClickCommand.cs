using OnScreenKeyboard.Controls;
using OnScreenKeyboard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnScreenKeyboard.Helpers
{
    class PushCharKeyboardButtonClickCommand : KeyboardButtonClickCommand
    {
        KeyboardControl _control;
        public PushCharKeyboardButtonClickCommand(IKeyboardInputContext context, KeyboardControl control) : base(context)
        {
            _control = control;
        }

        protected override void OnButtonClicked(KeyboardButton button)
        {
            if (button.Char == null)
                return;

            var charToPush = button.Char[0];
            if (_control.IsShifted || _control.IsCapsLocked)
                charToPush = char.ToUpper(charToPush);
            if (_control.IsShifted)
                _control.IsShifted = false;

            Context.PushChar(charToPush);
        }
    }
}
