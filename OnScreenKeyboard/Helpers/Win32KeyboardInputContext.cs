using OnScreenKeyboard.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace OnScreenKeyboard.Helpers
{
    class Win32KeyboardInputContext : IKeyboardInputContext
    {
        private WindowInteropHelper thisWindow = null;
        private Window window = null;
        private Config _cfg;

        public Win32KeyboardInputContext(Window window, Config cfg)
        {
            this.window = window;
            thisWindow = new WindowInteropHelper(window);
            _cfg = cfg;
            SetWindowFlags();
        }

        private void SetWindowFlags()
        {
            var style = Win32Interop.GetWindowLong(thisWindow.Handle, Win32Interop.GWL_EXSTYLE);
            Win32Interop.SetWindowLong(thisWindow.Handle, Win32Interop.GWL_EXSTYLE, style |
                Win32Interop.WS_EX_NOACTIVATE);
        }

        public void PlaceWindow()
        {

            var rect = Win32Interop.GetWindowRectManaged(thisWindow.Handle);
            var monitor = Win32Interop.MonitorFromRectManaged(rect, Win32Interop.MONITOR_DEFAULTTOPRIMARY);
            var monitorInfo = Win32Interop.GetMonitorInfoManaged(monitor);

            var positions = GetPosition(monitorInfo);

            Win32Interop.SetWindowPos(thisWindow.Handle, new IntPtr(-1),
                positions.Item1, positions.Item2, positions.Item3, positions.Item4,
                Win32Interop.SWP_NOACTIVATE);
        }

        public void DockWindowAtBottom()
        {
            var rect = Win32Interop.GetWindowRectManaged(thisWindow.Handle);
            var monitor = Win32Interop.MonitorFromRectManaged(rect, Win32Interop.MONITOR_DEFAULTTOPRIMARY);
            var monitorInfo = Win32Interop.GetMonitorInfoManaged(monitor);

            var positions = GetPosition(monitorInfo);
            positions.Item2 = monitorInfo.rcMonitor.bottom - monitorInfo.rcMonitor.top - positions.Item4;

            Win32Interop.SetWindowPos(thisWindow.Handle, new IntPtr(-1),
                positions.Item1, positions.Item2, positions.Item3, positions.Item4,
                Win32Interop.SWP_NOACTIVATE);
        }

        private (int, int, int, int) GetPosition(Win32Interop.MONITORINFO mi)
        {
            int x, y, cx, cy;
            if (_cfg.Top.HasValue)
                y = (int)_cfg.Top.Value.TranslateToPixelPoint(mi.rcMonitor.top, mi.rcMonitor.bottom - mi.rcMonitor.top);
            else
                y = mi.rcMonitor.top + (mi.rcMonitor.bottom - mi.rcMonitor.top) / 4;

            if (_cfg.Bottom.HasValue)
                cy = (int)_cfg.Bottom.Value.TranslateToPixelPoint(mi.rcMonitor.bottom, mi.rcMonitor.top - mi.rcMonitor.bottom) - y;
            else
                cy = (mi.rcMonitor.bottom - mi.rcMonitor.top) / 2;

            if (_cfg.Left.HasValue)
                x = (int)_cfg.Left.Value.TranslateToPixelPoint(mi.rcMonitor.left, mi.rcMonitor.right - mi.rcMonitor.left);
            else
                x = mi.rcMonitor.left;

            if (_cfg.Right.HasValue)
                cx = (int)_cfg.Right.Value.TranslateToPixelPoint(mi.rcMonitor.right, mi.rcMonitor.left - mi.rcMonitor.right) - x;
            else
                cx = mi.rcMonitor.right;

            return (x, y, cx, cy);
        }

        public bool IsFocused()
        {
            return Win32Interop.GetForegroundWindow() != IntPtr.Zero;
        }

        public void PushChar(char c)
        {
            var targetWindow = Win32Interop.GetForegroundWindow();
            if (targetWindow == IntPtr.Zero)
                return;

            var utf16Char = Encoding.Unicode.GetBytes(new char[] { c });
            var utf16CharPacked = (uint)(utf16Char[1] << 8 | utf16Char[0]);

            Debug.WriteLine("Char Code 0x" + utf16CharPacked.ToString("X"));

            //Win32Interop.PostMessage(targetWindow, Win32Interop.WM_KEYDOWN, new IntPtr(utf16CharPacked),
            //    new IntPtr(0));
            //Win32Interop.PostMessage(targetWindow, Win32Interop.WM_KEYUP, new IntPtr(utf16CharPacked),
            //    new IntPtr(0));
            Win32Interop.PostMessage(targetWindow, Win32Interop.WM_CHAR, new IntPtr(utf16CharPacked),
                new IntPtr(0));
        }

        public void PushKeyCode(int keyCode)
        {
            var targetWindow = Win32Interop.GetForegroundWindow();
            if (targetWindow == IntPtr.Zero)
                return;

            Win32Interop.PostMessage(targetWindow, Win32Interop.WM_KEYDOWN, new IntPtr(keyCode),
                new IntPtr(0));
            Win32Interop.PostMessage(targetWindow, Win32Interop.WM_KEYUP, new IntPtr(keyCode),
                new IntPtr(0));
        }

        public void EnableBlurredBackground()
        {
            Win32Interop.EnableBlurBackground(thisWindow.Handle);
        }
    }
}
