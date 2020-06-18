using OnScreenKeyboard.Helpers;
using OnScreenKeyboard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace OnScreenKeyboard.Controls
{
    class KeyboardControl : Control
    {
        public KeyboardLayout CurrentLayout
        {
            get { return (KeyboardLayout)GetValue(CurrentLayoutProperty); }
            set { SetValue(CurrentLayoutProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentLayoutProperty =
            DependencyProperty.Register("CurrentLayout", typeof(KeyboardLayout), typeof(KeyboardControl),
                new FrameworkPropertyMetadata(OnKeyboardLayoutChangedInternal));



        public List<KeyboardLayout> AvailibleLayouts
        {
            get { return (List<KeyboardLayout>)GetValue(AvailibleLayoutsProperty); }
            set { SetValue(AvailibleLayoutsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AvailibleLayouts.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AvailibleLayoutsProperty =
            DependencyProperty.Register("AvailibleLayouts", typeof(List<KeyboardLayout>), typeof(KeyboardControl),
                new PropertyMetadata(null));



        public IKeyboardInputContext KeyboardInputContext
        {
            get { return (IKeyboardInputContext)GetValue(KeyboardInputContextProperty); }
            set { SetValue(KeyboardInputContextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for KeyboardInputContext.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyboardInputContextProperty =
            DependencyProperty.Register("KeyboardInputContext", typeof(IKeyboardInputContext), typeof(KeyboardLayout),
                new PropertyMetadata(null, OnKeyboardInputContextChangedInternal));

        public bool IsShifted { get; set; }
        public bool IsCapsLocked { get; set; }

        private List<IKeyboardButtonInitializer> _initializers;

        public KeyboardControl()
        {
        }

        private static void OnKeyboardInputContextChangedInternal(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is KeyboardControl)
            {
                ((KeyboardControl)(d)).OnKeyboardInputContextChanged((IKeyboardInputContext)e.NewValue);
            }
        }

        private static void OnKeyboardLayoutChangedInternal(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is KeyboardControl)
            {
                ((KeyboardControl)(d)).OnKeyboardLayoutChanged((KeyboardLayout)e.OldValue, (KeyboardLayout)e.NewValue);
            }
        }

        private Popup _popup;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _popup = (Popup)GetTemplateChild("_popup");

            if (KeyboardInputContext != null)
            {
                InitCommands(KeyboardInputContext);
                InitInitializers();
                CurrentLayout?.InitButtons(_initializers);
            }
        }

        protected virtual void OnKeyboardLayoutChanged(KeyboardLayout oldValue, KeyboardLayout newValue)
        {
            if (_initializers != null)
                CurrentLayout?.InitButtons(_initializers);

            if (_popup != null)
                _popup.IsOpen = false;
        }

        private void OnKeyboardInputContextChanged(IKeyboardInputContext newValue)
        {
            InitCommands(newValue);
            InitInitializers();
            CurrentLayout?.InitButtons(_initializers);
        }

        private void InitInitializers()
        {
            _initializers = new List<IKeyboardButtonInitializer>();
            _initializers.Add(new SpecialButtonInitializer(_changeLangCommand, new KeyboardButton
            {
                DisplayChar = "LANG"
            }, "change_lang")
            {
                DisplayCharFactory = () => CurrentLayout.LanguageNameShort
            });
            _initializers.Add(new SpecialButtonInitializer(_capsKeyClick, new KeyboardButton
            {
                DisplayChar = "Caps Lock"
            }, "caps_lock"));
            _initializers.Add(new SpecialButtonInitializer(_shiftKeyClick, new KeyboardButton
            {
                DisplayChar = "Shift"
            }, "shift"));
            _initializers.Add(new SpecialButtonInitializer(_keyCodeKeyClick, new KeyboardButton
            {
                DisplayChar = "Backspace",
                KeyCode = 0x08
            }, "backspace"));
            _initializers.Add(new DefaultKeyboardButtonInitializer(_defaultKeyClick));
        }


        #region Commands
        ICommand _changeLangCommand;
        ICommand _defaultKeyClick;
        ICommand _shiftKeyClick;
        ICommand _capsKeyClick;
        ICommand _keyCodeKeyClick;

        private void InitCommands(IKeyboardInputContext context)
        {
            _shiftKeyClick = new RelayCommand(ShiftCommandExec);
            _capsKeyClick = new RelayCommand(CapsCommandExec);
            _changeLangCommand = new RelayCommand(ChangeLangExec);
            _defaultKeyClick = new PushCharKeyboardButtonClickCommand(context, this);
            _keyCodeKeyClick = new PushKeyCodeKeyboardButtonClickCommand(context);
        }

        private void ChangeLangExec()
        {
            if (_popup != null)
                _popup.IsOpen = !_popup.IsOpen;
        }

        private void ShiftCommandExec()
        {
            IsShifted = !IsShifted;
        }
        private void CapsCommandExec()
        {
            IsCapsLocked = !IsCapsLocked;
        }
        #endregion
    }
}
