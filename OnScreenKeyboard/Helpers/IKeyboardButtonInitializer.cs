using OnScreenKeyboard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OnScreenKeyboard.Helpers
{
    public interface IKeyboardButtonInitializer
    {
        bool CanInitialize(KeyboardButton button);
        void Initialize(KeyboardButton button);
    }

    public class DefaultKeyboardButtonInitializer : IKeyboardButtonInitializer
    {
        public ICommand Command { get; private set; }

        public DefaultKeyboardButtonInitializer(ICommand clickCommand)
        {
            Command = clickCommand;
        }

        public virtual bool CanInitialize(KeyboardButton button)
        {
            return button.Command != Command;
        }

        public virtual void Initialize(KeyboardButton button)
        {
            button.Command = Command;
        }
    }

    public class SpecialButtonInitializer : DefaultKeyboardButtonInitializer
    {
        public string SpecialButtonId { get; private set; }
        public KeyboardButton ButtonTemplate { get; private set; }
        public Func<string> DisplayCharFactory { get; set; }

        public SpecialButtonInitializer(ICommand clickCommand, KeyboardButton template, string specialButtonId) : base(clickCommand)
        {
            SpecialButtonId = specialButtonId.Trim().ToLower();
            ButtonTemplate = template;
        }

        public override bool CanInitialize(KeyboardButton button)
        {
            return !string.IsNullOrEmpty(button.PredefinedKey) &&
                button.PredefinedKey.Trim().ToLower() == SpecialButtonId;
        }

        public override void Initialize(KeyboardButton button)
        {
            base.Initialize(button);

            button.Char = ButtonTemplate.Char;
            button.KeyCode = ButtonTemplate.KeyCode;
            if (DisplayCharFactory != null)
                button.DisplayChar = DisplayCharFactory();
            else
                button.DisplayChar = ButtonTemplate.DisplayChar;
        }
    }
}
