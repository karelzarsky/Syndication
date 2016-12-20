using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using SyndicateLogic;

namespace SyndicateProcessing
{
    public partial class Processing : ServiceBase
    {
        public Processing()
        {
            InitializeComponent();
            DataLayer.LogMessage(LogLevel.Service, "Processing start.");
        }

        private Thread _thread;
        private readonly ManualResetEvent _shutdownEvent = new ManualResetEvent(false);
        private readonly ManualResetEvent _scheduleEvent = new ManualResetEvent(false);
        private readonly System.Timers.Timer _scheduleTimer = new System.Timers.Timer();

        protected override void OnStart(string[] args)
        {
            using (Process p = Process.GetCurrentProcess())
                p.PriorityClass = ProcessPriorityClass.Idle;
            _scheduleTimer.AutoReset = false;
            _scheduleTimer.Interval = 5000; // in milliseconds
            _scheduleTimer.Elapsed += delegate { _scheduleEvent.Set(); };
            _thread = new Thread(delegate ()
            {
                // Create the WaitHandle array.
                var handles = new WaitHandle[] { _shutdownEvent, _scheduleEvent };
                // Start the timer.
                _scheduleTimer.Start();
                // Wait for one of the events to occur.
                while (!_shutdownEvent.WaitOne(0))
                {
                    switch (WaitHandle.WaitAny(handles))
                    {
                        case 0: // Shutdown Event
                            break;
                        case 1: // Schedule Event
                            _scheduleTimer.Stop();
                            _scheduleEvent.Reset();
                            ThreadPool.QueueUserWorkItem(PerformScheduledWork, null);
                            break;
                        default:
                            _shutdownEvent.Set(); // should never occur
                            break;
                    }
                }
            })
            { IsBackground = true };
            _thread.Start();
        }

        protected override void OnStop()
        {
            DataLayer.LogMessage(LogLevel.Service, "Processing stop.");
            // Signal the thread to shutdown.
            _shutdownEvent.Set();
            // Give the thread 10 seconds to terminate.
            if (!_thread.Join(5000))
            {
                _thread.Abort(); // not perferred, but the service is closing anyway
            }
        }

        private static List<int> ShinglesToProcess;

        private void PerformScheduledWork(object state)
        {
            // Perform your work here, but be mindful of the _shutdownEvent in case
            // the service is shutting down.
            //
            // Reschedule the work to be performed.

            using (var ctx = new Db())
            {
                ctx.Database.CommandTimeout = 120;
                if (ShinglesToProcess == null || ShinglesToProcess.Count == 0)
                {
                    ShinglesToProcess = ShingleLogic.GetNextShingleList(ctx);
                }
                if (ShinglesToProcess == null || ShinglesToProcess.Count == 0)
                    Thread.Sleep(20000);
                else
                {
                    int next = ShinglesToProcess.First();
                    ShingleLogic.AnalyzeShingle(next);
                    ShinglesToProcess.Remove(next);
                }
            }
            _scheduleTimer.Start();
        }
    }
}
