using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace HiddenCmdRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                string cmdFileName = args[0];
                Process p = new Process();
                p.StartInfo.FileName = cmdFileName;
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.Arguments = string.Join(" ", args.Skip(1));
                p.Start();
            }
        }
    }
}
