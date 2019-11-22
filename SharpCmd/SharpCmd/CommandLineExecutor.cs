using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using MoreLinq;

namespace SharpCmd
{
    public static class CommandLineExecutor
    {
        public static string ExecuteCmd(string command,
            Func<IObservable<string>, IDisposable> outputFeedSubscribeAction = null,
            Action<IObserver<string>> processAttachAction = null)
        {
            return Execute("cmd", command, outputFeedSubscribeAction, processAttachAction);
        }
        public static string ExecuteCmd(string[] commands,
            Func<IObservable<string>, IDisposable> outputFeedSubscribeAction = null,
            Action<IObserver<string>> processAttachAction = null)
        {
            return Execute("cmd", commands, outputFeedSubscribeAction, processAttachAction);
        }
        public static string ExecuteCleartool(string command,
            Func<IObservable<string>, IDisposable> outputFeedSubscribeAction = null,
            Action<IObserver<string>> processAttachAction = null)
        {
            return Execute("cleartool", command, outputFeedSubscribeAction, processAttachAction);
        }
        public static string ExecuteCleartool(string[] commands,
            Func<IObservable<string>, IDisposable> outputFeedSubscribeAction = null,
            Action<IObserver<string>> processAttachAction = null)
        {
            return Execute("cleartool", commands, outputFeedSubscribeAction, processAttachAction);
        }

        private static string Execute(string processName, 
            IEnumerable<string> commands, 
            Func<IObservable<string>, IDisposable> outputFeedSubscribeAction,
            Action<IObserver<string>> processAttachAction)
        {
            return CommandPromptRunner.Run(processName, commands.Concat("exit").ToObservable(), IsConsole(), outputFeedSubscribeAction, processAttachAction);
        }
        private static string Execute(string processName, 
            string command, 
            Func<IObservable<string>, IDisposable> outputFeedSubscribeAction, 
            Action<IObserver<string>> processAttachAction)
        {
            return Execute(processName, new[] { command }, outputFeedSubscribeAction, processAttachAction);
        }

        private static bool IsConsole()
        {
            return Console.In != StreamReader.Null;
        }
    }
}