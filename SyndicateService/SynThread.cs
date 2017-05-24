using System;
using System.Threading;
using Timer = System.Timers.Timer;

namespace SyndicateService
{
    public class SynThread
    {
        public Thread thread;
        public readonly ManualResetEvent scheduleEvent = new ManualResetEvent(false);
        public readonly Timer timer;
        public readonly string name;
        public readonly Action<object> performWork;

        public SynThread(string name, double interval, Action<object> work)
        {
            this.name = name;
            performWork = work;
            scheduleEvent = new ManualResetEvent(false);
            timer = new Timer(interval) { AutoReset = false };
            timer.Elapsed += delegate { scheduleEvent.Set(); };
        }
    }
}
