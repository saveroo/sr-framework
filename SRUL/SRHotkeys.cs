using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SRUL
{
    public class SRHotkeys
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        
        private int key;
        private IntPtr hWnd;
        private int id;

        // constructor
        public SRHotkeys(Keys key, Form form)
        {
            this.key = (int)key;
            this.hWnd = form.Handle;
            id = this.GetHashCode();
        }

        public override int GetHashCode()
        {
            return key ^ hWnd.ToInt32();
        }
        
        // register hot key
        public void Register()
        {
            RegisterHotKey(hWnd, id, 0, key);
        }
        
        // unregister hot key
        public void Unregister()
        {
            UnregisterHotKey(hWnd, id);
        }
    }
}