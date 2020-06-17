using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnScreenKeyboard.Helpers
{
    interface IKeyboardInputContext
    {
        bool IsFocused();
        void PushChar(char c);
        void PushKeyCode(int keyCode);
    }
}
