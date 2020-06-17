using OnScreenKeyboard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnScreenKeyboard.Helpers
{
    class PushKeyCodeKeyboardButtonClickCommand : KeyboardButtonClickCommand
    {
        public PushKeyCodeKeyboardButtonClickCommand(IKeyboardInputContext context) : base(context)
        {

        }
        protected override void OnButtonClicked(KeyboardButton button)
        {
            Context.PushKeyCode(button.KeyCode);
        }
    }
}
