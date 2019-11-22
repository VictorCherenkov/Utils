using System;
using System.Linq;
using System.Reactive.Linq;
using NUnit.Framework;
using SharpCmd;

namespace UnitTests
{
    [TestFixture]
    public class Sandbox
    {
        [Test]
        public void TestSimpleExecute()
        {
            var result = CommandLineExecutor.ExecuteCmd("ipconfig");
            Console.WriteLine(result);
        }
        [Test]
        public void TestSimpleExecuteDifferentProcess()
        {
            var result = CommandLineExecutor.ExecuteCleartool("lsview -host HS7PW12 -quick -long");
            Console.WriteLine(result);
        }
        [Test]
        public void TestRunWithInputAndCancellation()
        {
            var commands = new[]
            {
                new { Number = 1, Text = "date"},
                new { Number = 2, Text = "10-06-2015"},
                new { Number = 3, Text = "date"},
                new { Number = 4, Text = "09-06-2015"},
                new { Number = 10, Text = "ping -t 127.0.0.1"},
                new { Number = 17, Text = SpecialCommands.CtrlC},
                new { Number = 20, Text = "exit"},
            };
            var inputFeed = Observable
                .Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1))
                .Select(x => commands.FirstOrDefault(c => c.Number == x)?.Text)
                .Where(x => x!= null);
            var result = CommandPromptRunner.Run("cmd", inputFeed, outputFeedSubscribeAction: o => o.Subscribe(x => { if (x == "\n") Console.Write("."); }));
            Console.WriteLine();
            Console.WriteLine(result);
        }
    }
}
