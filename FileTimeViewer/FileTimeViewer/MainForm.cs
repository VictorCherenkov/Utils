using System;
using System.Windows.Forms;

namespace FileTimeViewer
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            Render(dateTimePickerEx1.Value);
        }

        private void TextBox1TextChanged(object sender, EventArgs e)
        {
            string fileTimeStr = textBox1.Text.Replace(",", string.Empty).Replace(".", string.Empty);
            long fileTime = long.Parse(fileTimeStr);
            Render(fileTime);
        }

        private void DateTimePickerEx1ValueChanged(object sender, EventArgs e)
        {
            Render(dateTimePickerEx1.Value);
        }

        private void Render(DateTime dateTime)
        {
            Render(dateTime.ToFileTime());
        }

        private void Render(long fileTime)
        {
            textBox1.Text = fileTime.ToString();
            dateTimePickerEx1.Value = DateTime.FromFileTime(fileTime);
        }
    }
}