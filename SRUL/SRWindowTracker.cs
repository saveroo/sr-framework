using System;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using DevExpress.XtraEditors;
using SRUL.Types;
using MessageBox = System.Windows.Forms.MessageBox;

namespace SRUL
{

    public class WindowTracker
    {

        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        static extern IntPtr 
            SetWinEventHook(
                uint eventMin,
                uint eventMax,
                IntPtr hmodWinEventProc,
                WinEventDelegate lpfnWinEventProc,
                uint idProcess,
                uint idThread,
                uint dwFlags);

        [DllImport("user32.dll")]
        static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32.dll")]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsIconic(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool IsZoomed(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd);

        const uint EVENT_OBJECT_LOCATIONCHANGE = 0x800B;
        const uint WINEVENT_OUTOFCONTEXT = 0;

        static WinEventDelegate procDelegate = new WinEventDelegate(WinEventProc);
        private static IntPtr hhook;
        private static Form f = null;
        static Process p = null;
        public static int userX = 0;
        public static int userY = 0;

        internal static bool IsFullscreen(IntPtr wndHandle, Screen screen)
        {
            RECT r = new RECT();
            GetWindowRect(wndHandle, out r);
            return new Rectangle(r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top)
                .Contains(screen.Bounds);
        }

        public static void 
            HookChildWindow(
                Process gameProcess, 
                XtraForm parentWindow, 
                XtraForm childWindow)
        {
            f = childWindow;
            p = gameProcess;
            if (p == null)
            {
                XtraMessageBox.Show("Game is not running, please start the game first!");
                return;
            }
            
            
            // f.Show(new SimpleWindow(p.MainWindowHandle));
            f?.Show();
            if(f != null) 
                f.TopMost = true;
            
            // This check fullscreen based on resolution of the window, not actually according to game.
            // if (!IsFullscreen(p.MainWindowHandle, Screen.FromHandle(p.MainWindowHandle)))
            // {
            //     f?.Show();
            //     if(f != null) 
            //         f.TopMost = true;
            // } 

            hhook = SetWinEventHook(
                EVENT_OBJECT_LOCATIONCHANGE, 
                EVENT_OBJECT_LOCATIONCHANGE, 
                IntPtr.Zero, 
                procDelegate, 
                (uint)p.Id, 
                0, 
                WINEVENT_OUTOFCONTEXT);
        }

        private class SimpleWindow : System.Windows.Forms.IWin32Window
        {
            IntPtr h = IntPtr.Zero;
            public SimpleWindow(IntPtr ptr)
            {
                h = ptr;
            }
            public IntPtr Handle
            {
                get { return h; }
            }
        }

        public static void Unhook()
        {
            UnhookWinEvent(hhook);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public static RECT r;
        public static int h;
        public static int x;
        public static int y;

        static void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (idObject != 0 || idChild != 0)
                return;

            // RECT r = new RECT();
            // GetWindowRect(p.MainWindowHandle, out r);
            // int h = r.Bottom - r.Top;
            // // int x = r.Right - f.Width;
            // int x = r.Right - (f.Width * 3);
            // int y = r.Top + (h - f.Height);
            // int y = r.Top + ((h - f.Height) / 4);
            // f.Location = new System.Drawing.Point(x, y);
            WindowTracker.r = new RECT();
            GetWindowRect(p.MainWindowHandle, out WindowTracker.r);
            WindowTracker.h = WindowTracker.r.Bottom - WindowTracker.r.Top;
            var w = WindowTracker.r.Right - WindowTracker.r.Left;
            WindowTracker.x = WindowTracker.r.Right - (w - userX);
            WindowTracker.y = WindowTracker.r.Bottom - (h - userY);
            f.Location = new System.Drawing.Point(WindowTracker.x, WindowTracker.y);
        }
    }
}