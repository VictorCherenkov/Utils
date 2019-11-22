using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using ProcessKeeper.Properties;

namespace ProcessKeeper
{
    /// <summary>
    /// This form is invisible for service emulation.
    /// </summary>
    public partial class InvisibleForm : Form
    {
        #region Init & Hiding
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr window, int index, int value);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr window, int index);

        const int c_gwlExstyle = -20;
        const int c_wsExToolwindow = 0x00000080;

        public InvisibleForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            HideWindowTotal();
            timer1.Interval = 1000 * Settings.Default.RestartIntervalSec;
            timer1.Start();
        }

        private void HideWindowTotal()
        {
            Visible = false;
            int windowStyle = GetWindowLong(Handle, c_gwlExstyle);
            SetWindowLong(Handle, c_gwlExstyle, windowStyle | c_wsExToolwindow);
        }

        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            string processfileName = Settings.Default.ProcessExePath;
            new ProcessStartInfo(processfileName).WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(processfileName);            
        }
    }
}