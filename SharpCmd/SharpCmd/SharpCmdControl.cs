using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FrameworkEx;

namespace SharpCmd
{
    public partial class SharpCmdControl : UserControl
    {
        private readonly HistoryBrowser m_history = new HistoryBrowser();
        private IObserver<string> m_observer;

        public SharpCmdControl()
        {
            InitializeComponent();
        }

        public RichTextBox RichTextBox => richTextBox1;
        public TextBox TextBox => textBox1;
        public Button Button => button1;

        public void CtrlC()
        {
            m_observer?.OnNext(SpecialCommands.CtrlC);
        }
        public void AttachToProcess(IObserver<string> inputFeed)
        {
            m_observer = inputFeed;
        }
        public void StartCmdPromptProcess(string processName)
        {
            var subject = new Subject<string>();
            m_observer = subject;
            var syncContext = SynchronizationContext.Current;
            Task.Factory.StartNew(() =>
            {
                CommandPromptRunner.Run
                (
                    processName,
                    subject,
                    outputFeedSubscribeAction: o => o.Buffer(TimeSpan.FromMilliseconds(50), 10).ObserveOn(syncContext)
                        .Subscribe(Observer.Create<IList<string>>(x => richTextBox1.AppendText(string.Join(string.Empty, x))))
                );
            });
        }

        private void OnTextBox1KeyDown(object sender, KeyEventArgs e)
        {
            var text = textBox1.Text;
            e.Handled = e.KeyCode.In(Keys.Down, Keys.Up);
            textBox1.Text = e.KeyCode == Keys.Up ? (m_history.Prev() ?? text)
                : e.KeyCode == Keys.Down ? (m_history.Next() ?? text)
                : e.KeyCode == Keys.Enter ? string.Empty
                : text;
            if (e.KeyCode == Keys.Enter)
            {
                m_history.Add(text);
                if (text == "cls")
                {
                    richTextBox1.Clear();
                }
                else
                {
                    m_observer?.OnNext(text);
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            m_observer?.OnNext(SpecialCommands.CtrlC);
        }
    }
}
