using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using SyndicateLogic;
using SyndicateLogic.Entities;
using Timer = System.Timers.Timer;

// instalace:
// c:\windows\microsoft.net\framework\v4.0.30319\installutil.exe c:\GIT\trade\SyndicateService\SyndicateService\bin\Debug\SyndicateService.exe 
//

namespace SyndicateService
{
    public partial class Service : ServiceBase
    {
        public Service()
        {
            using (Process p = Process.GetCurrentProcess())
                p.PriorityClass = ProcessPriorityClass.BelowNormal;
            InitializeComponent();
            DataLayer.LogMessage(LogLevel.Service, $"Syndication service start.");
        }

        private Thread _thread;
        private readonly ManualResetEvent _shutdownEvent = new ManualResetEvent(false);
        private readonly ManualResetEvent _scheduleEvent = new ManualResetEvent(false);
        private readonly Timer _scheduleTimer = new Timer();

        protected override void OnStart(string[] args)
        {
            // Configure the timer.
            _scheduleTimer.AutoReset = false;
            _scheduleTimer.Interval = 5000; // in milliseconds
            _scheduleTimer.Elapsed += delegate { _scheduleEvent.Set(); };
            RssLogic.UpdateServerConnection();
            using (var ctx = new Db())
            {
                RssLogic.AddNewFeedsFromResource(ctx);
            }
            // Create the thread using anonymous method.
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
            DataLayer.LogMessage(LogLevel.Service, "Service stop.");
            // Signal the thread to shutdown.
            _shutdownEvent.Set();
            // Give the thread 10 seconds to terminate.
            if (!_thread.Join(5000))
            {
                _thread.Abort(); // not perferred, but the service is closing anyway
            }
        }

        private void PerformScheduledWork(object state)
        {
            // Perform your work here, but be mindful of the _shutdownEvent in case
            // the service is shutting down.
            //
            // Reschedule the work to be performed.

            //DataLayer.LogMessage(LogLevel.Info, "New run scheduled.");
            using (var context = new Db())
            {
                Feed f = RssLogic.GetNextFeed(context);
                if (f != null)
                {
                    //DataLayer.LogMessage(LogLevel.Info, $"N Next feed {f.ID} {f.Url}");
                    RssLogic.ProcessFeed(f, context);
                    //DataLayer.LogMessage(LogLevel.Info, $"Completed feed {f.ID} {f.Url}");
                    context.SaveChanges();
                }
            }
            _scheduleTimer.Start();
        }
    }
}