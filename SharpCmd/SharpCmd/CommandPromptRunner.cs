using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading;

namespace SharpCmd
{
    public static class CommandPromptRunner
    {
        public static string Run(string processName, 
            IObservable<string> inputFeed, 
            bool callFromConsole = false, 
            Func<IObservable<string>, IDisposable> outputFeedSubscribeAction = null, 
            Action<IObserver<string>> processAttachAction = null)
        {
            var result = string.Empty;
            var completed = new AutoResetEvent(false);
            using (var cmd = new CommandPrompt(processName, callFromConsole))
            using (outputFeedSubscribeAction?.Invoke(cmd.Observable))
            using (cmd.Observable.Aggregate(new StringBuilder(), (b, s) => b.Append(s), b => b.ToString()).Subscribe(x => result = x, () => completed.Set()))
            {
                var observer = Observer.Create<string>(cmd.WriteInput);
                processAttachAction?.Invoke(observer);
                cmd.Start();
                using (inputFeed.Subscribe(observer))
                {
                    completed.WaitOne();
                }
            }
            return result;
        }
    }
}