using System;
using System.Diagnostics;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpCmd
{
    internal class CommandPrompt : IDisposable
    {
        private readonly bool m_loadedFromConsole;
        private readonly Process m_cmdProcess;

        [DllImport("User32.dll")]private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        [DllImport("user32.dll")]private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private readonly Subject<string> m_subject = new Subject<string>();

        public IObservable<string> Observable => m_subject;

        public CommandPrompt(string processName, bool loadedFromConsole = false)
        {
            m_loadedFromConsole = loadedFromConsole;
            m_cmdProcess = new Process
            {
                StartInfo =
                {
                    FileName = processName,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    CreateNoWindow = false,
                    StandardOutputEncoding = Encoding.GetEncoding(866),
                }
            };
        }

        public void Start()
        {
            m_cmdProcess.Start();
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    var buffer = new char[1];
                    var read = m_cmdProcess.StandardOutput.Read(buffer, 0, buffer.Length);
                    if (read == 0)
                    {
                        break;
                    }
                    var data = new string(buffer);
                    try
                    {
                        m_subject.OnNext(data);
                    }
                    catch (Exception ex)
                    {
                        m_subject.OnError(ex);
                        break;
                    }
                }
                m_subject.OnCompleted();
            });
            if (!m_loadedFromConsole)
            {
                while (m_cmdProcess.MainWindowHandle == IntPtr.Zero)
                {
                    //wait (do not thread.sleep here, it will auto release on !IntPtr.Zero(when it gets the handle))
                }
                m_cmdProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                ShowWindow(m_cmdProcess.MainWindowHandle, 0);
                m_cmdProcess.PriorityClass = ProcessPriorityClass.Idle;
            }            
        }
        public void WriteInput(string cmd)
        {
            switch (cmd)
            {
                case SpecialCommands.CtrlC:
                    SendCtrlC();
                    break;
                case SpecialCommands.ShutDown:
                    Dispose();
                    break;
                default:
                    m_cmdProcess.StandardInput.WriteLine(cmd);
                    break;
            }
        }

        public void Dispose()
        {
            if (m_cmdProcess != null && !m_cmdProcess.HasExited)
            {
                SendCtrlC();
                m_cmdProcess.StandardInput.Close();
                if (!m_cmdProcess.WaitForExit(1000))
                {
                    m_cmdProcess.CancelOutputRead();
                }
                if (!m_cmdProcess.WaitForExit(2000))
                {                    
                    m_cmdProcess.Kill();
                }
            }
            m_subject.Dispose();
        }
        private void SendCtrlC()
        {
            if (!m_cmdProcess.HasExited)
            {
                while (m_cmdProcess.MainWindowHandle == IntPtr.Zero)
                {
                    //wait (do not thread.sleep here, it will auto release on !IntPtr.Zero(when it gets the handle))
                }
                m_cmdProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                ShowWindow(m_cmdProcess.MainWindowHandle, 1);
            }
            if (!m_cmdProcess.HasExited)
            {
                while (m_cmdProcess.MainWindowHandle == IntPtr.Zero)
                {
                    //wait (do not thread.sleep here, it will auto release on !IntPtr.Zero(when it gets the handle))
                }
                const uint keyeventfKeyup = 2;
                const byte vkControl = 0x11;
                //hWnd == handle to console window
                //set it to foreground or u can not send commands
                SetForegroundWindow(m_cmdProcess.MainWindowHandle);
                //sending keyboard event Ctrl+C
                keybd_event(vkControl, 0, 0, 0);
                keybd_event(0x43, 0, 0, 0);
                keybd_event(0x43, 0, keyeventfKeyup, 0);
                keybd_event(vkControl, 0, keyeventfKeyup, 0);
            }
            if (!m_cmdProcess.HasExited)
            {
                while (m_cmdProcess.MainWindowHandle == IntPtr.Zero)
                {
                    //wait (do not thread.sleep here, it will auto release on !IntPtr.Zero(when it gets the handle))
                }
                //dont forget to also set it to hidden now
                m_cmdProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                ShowWindow(m_cmdProcess.MainWindowHandle, 0);
            }
        }
    }
}