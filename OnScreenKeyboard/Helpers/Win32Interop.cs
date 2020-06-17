using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OnScreenKeyboard.Helpers
{
    static class Win32Interop
    {
        #region Structs
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public int dwFlags;
        }

        internal enum AccentState
        {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLE_GRADIENT = 1,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_ENABLE_ACRYLICBLURBEHIND = 4, // RS4 1803
            ACCENT_ENABLE_HOSTBACKDROP = 5, // RS5 1809
            ACCENT_INVALID_STATE = 6
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct AccentPolicy
        {
            public AccentState AccentState;
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        internal enum WindowCompositionAttribute
        {
            // ...
            WCA_ACCENT_POLICY = 19
            // ...
        }

        #endregion

        #region Constants
        public const int WS_EX_TRANSPARENT = 0x00000020;
        public const int WS_EX_NOACTIVATE = 0x08000000;
        public const int GWL_EXSTYLE = -20;
        public const int WM_NCLBUTTONDOWN = 0x00A1;
        public const int HT_CAPTION = 0x0002;

        public const int WM_SYSCOMMAND = 0x112;
        public const int WM_SYSCHAR = 0x0106;
        public const int WM_CHAR = 0x0102;
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;
        public const int MF_BYPOSITION = 0x400;
        public const int MF_SEPARATOR = 0x800;

        public const uint TPM_LEFTALIGN = 0x0000;
        public const uint TPM_RETURNCMD = 0x0100;

        public const int MONITOR_DEFAULTTONULL = 0x00000000;
        public const int MONITOR_DEFAULTTOPRIMARY = 0x00000001;
        public const int MONITOR_DEFAULTTONEAREST = 0x00000002;
        public const int MONITORINFOF_PRIMARY = 0x00000001;

        public const uint SWP_NOACTIVATE = 0x0010;
        #endregion

        #region DLL Imports
        [DllImport("user32", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hwnd, int nIndex);

        [DllImport("user32", SetLastError = true)]
        public static extern int SetWindowLong(IntPtr hwnd, int nIndex, int dwNewValue);

        //................................
        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32", SetLastError = true)]
        public static extern bool InsertMenu(IntPtr hMenu, Int32 wPosition, Int32 wFlags, Int32 wIDNewItem, string lpNewItem);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool AppendMenu(IntPtr hMenu, int uFlags, int uIDNewItem, string lpNewItem);

        [DllImport("user32.dll")]
        public static extern int TrackPopupMenuEx(IntPtr hmenu, uint fuFlags, int x, int y, IntPtr hwnd, IntPtr lptpm);

        //................................
        [DllImport("user32.dll", SetLastError =true, CharSet = CharSet.Unicode)]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromRect(IntPtr lpRect, int dwFlags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hWnd, IntPtr lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, IntPtr lpmi);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();


        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);
        #endregion

        public static IntPtr MonitorFromRectManaged(RECT rect, int dwFlags)
        {
            var rectPtr = Marshal.AllocHGlobal(Marshal.SizeOf<RECT>());
            try
            {
                Marshal.StructureToPtr(rect, rectPtr, false);

                return MonitorFromRect(rectPtr, dwFlags);
            }
            finally
            {
                Marshal.FreeHGlobal(rectPtr);
            }
        }

        public static RECT GetWindowRectManaged(IntPtr hWnd)
        {
            var rectPtr = Marshal.AllocHGlobal(Marshal.SizeOf<RECT>());
            try
            {
                if (!GetWindowRect(hWnd, rectPtr))
                    throw new Win32Exception();

                return Marshal.PtrToStructure<RECT>(rectPtr);
            }
            finally
            {
                Marshal.FreeHGlobal(rectPtr);
            }
        }

        public static MONITORINFO GetMonitorInfoManaged(IntPtr hMonitor)
        {
            MONITORINFO mi = new MONITORINFO { cbSize = Marshal.SizeOf<MONITORINFO>() };
            var infoPtr = Marshal.AllocHGlobal(Marshal.SizeOf<MONITORINFO>());

            try
            {
                Marshal.StructureToPtr(mi, infoPtr, false);
                if (!GetMonitorInfo(hMonitor, infoPtr))
                    throw new Win32Exception();

                return Marshal.PtrToStructure<MONITORINFO>(infoPtr);
            }
            finally
            {
                Marshal.FreeHGlobal(infoPtr);
            }
        }

        public static void EnableBlurBackground(IntPtr handle)
        {
            var accent = new AccentPolicy();
            accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;

            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            try
            {
                Marshal.StructureToPtr(accent, accentPtr, false);

                var data = new WindowCompositionAttributeData();
                data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
                data.SizeOfData = accentStructSize;
                data.Data = accentPtr;

                SetWindowCompositionAttribute(handle, ref data);
            }
            finally
            {
                Marshal.FreeHGlobal(accentPtr);
            }
        }

        public static uint ConstructKeystrokeMessage(int repeatCount, int scanCode, bool isExtended, bool isAlt, bool wasDown, bool isReleased)
        {
            return (uint)(((isReleased ? 1 : 0) << 31) |
                ((wasDown ? 1 : 0) << 30) |
                ((isAlt ? 1 : 0) << 29) |
                ((isExtended ? 1 : 0) << 24) |
                ((scanCode & 0xf) << 16) |
                ((repeatCount & 0xff)));
        }
    }
}
