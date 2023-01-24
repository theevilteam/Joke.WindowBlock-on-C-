using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlockWindow
{
    class Program
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        static void Main(string[] args)
        {
            while (true)
            {
                IntPtr handle = GetForegroundWindow();
                EnableWindow(handle, false);
                SetWindowPos(handle, HWND_BOTTOM, 0, 0, 1, 1, 0x0002);
                Thread.Sleep(100);
            }
        }
    }
}
