using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace flashing_window
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }



        [DllImport("user32.dll")]
        private static extern Int32 FlashWindowEx(ref FLASHWINFO pwfi);

        [StructLayout(LayoutKind.Sequential)]
        public struct FLASHWINFO
        {
            public UInt32 cbSize;
            public IntPtr hwnd;
            public Int32 dwFlags;
            public UInt32 uCount;
            public Int32 dwTimeout;
        }

        public bool Flash()
        {
            FLASHWINFO fw = new FLASHWINFO
                                {
                                    cbSize = Convert.ToUInt32(Marshal.SizeOf(typeof (FLASHWINFO))),
                                    hwnd = Handle,
                                    dwFlags = 2,
                                    uCount = UInt32.MaxValue
                                };
            FlashWindowEx(ref fw);
            return true;
        }

        private void Form1Load(object sender, EventArgs e)
        {
            Text = "DONE";
            Flash();
            GotFocus += (o, args) =>Close();
        }
    }
}
    