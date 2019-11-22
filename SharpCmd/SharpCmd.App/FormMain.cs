using System.Windows.Forms;

namespace SharpCmd.App
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            sharpCmd1.StartCmdPromptProcess("cmd");
        }
    }
}
