using System;
using System.Windows.Forms;

namespace FileTimeViewer
{
    public partial class DateTimePickerEx : DateTimePicker
    {
        private readonly TextBox m_msec;

        public DateTimePickerEx()
        {
            InitializeComponent();
            m_msec = new TextBox();
            //Controls.Add(m_msec);
            m_msec.Name = "Cmb";
            m_msec.Top = 0;
            m_msec.Left = 120;
            m_msec.Width = 50;
            m_msec.BringToFront();
            int ms = DateTime.Now.Millisecond;
            m_msec.Text = ms.ToString();
            //m_msec.ReadOnly = true;
        }

        public int Msec
        {
            get { return int.Parse(m_msec.Text); }
            set { m_msec.Text = value.ToString(); }
        }
    }
}
