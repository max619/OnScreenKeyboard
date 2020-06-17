using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnScreenKeyboard.Helpers
{
    class DebugKeyboardInputContext : IKeyboardInputContext
    {
        public IKeyboardInputContext TargetContext { get; set; }

        public DebugKeyboardInputContext(IKeyboardInputContext context)
        {
            TargetContext = context;
        }

        public bool IsFocused()
        {
            var res = TargetContext != null ? TargetContext.IsFocused() : true;
            Debug.WriteLine($"IsFocused returned {res}");
            return res;
        }

        public void PushChar(char c)
        {
            Debug.WriteLine($"Pushing char {c} to the context");
            TargetContext?.PushChar(c);
        }

        public void PushKeyCode(int keyCode)
        {
            Debug.WriteLine($"Pushing keycode {keyCode} to the context");
            TargetContext?.PushKeyCode(keyCode);
        }
    }
}
