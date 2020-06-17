using OnScreenKeyboard.Helpers;
using OnScreenKeyboard.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace OnScreenKeyboard
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var cfg = Config.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.xml"));
            Resources["Config"] = cfg;
            var debugContext = new DebugKeyboardInputContext(null);

            Resources["InputContext"] = debugContext;
            
            var window = new MainWindow();
            window.ShowInTaskbar = false;
            window.Show();
            window.Hide();
            var inputContext = new Win32KeyboardInputContext(window, cfg);
            debugContext.TargetContext = inputContext;

            if (cfg.DockAtBottom)
                inputContext.DockWindowAtBottom();
            else
                inputContext.PlaceWindow();

            window.Background.Opacity = cfg.WindowOpacity;
            if (cfg.BlurBackground)
                inputContext.EnableBlurredBackground();

            window.Show();
        }
    }
}
