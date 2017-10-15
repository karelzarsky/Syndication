using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using IntrinioConsole;
using SyndicateLogic;
using Timer = System.Timers.Timer;

namespace IntrinioService
{
    public partial class IntrinioService : ServiceBase
    {
        public IntrinioService()
        {
            using (Process p = Process.GetCurrentProcess())
                p.PriorityClass = ProcessPriorityClass.Idle;
            InitializeComponent();
            DataLayer.LogMessage(LogLevel.Service, $"I Intrinio service start.");
        }

        private Thread _thread;
        private readonly ManualResetEvent _shutdownEvent = new ManualResetEvent(false);
        private readonly ManualResetEvent _scheduleEvent = new ManualResetEvent(false);
        private readonly Timer _scheduleTimer = new Timer();

        protected override void OnStart(string[] args)
        {
            // DataLayer.LogMessage(LogLevel.Service, $"On start.");
            // Configure the timer.
            _scheduleTimer.AutoReset = false;
            _scheduleTimer.Interval = 24000 * 3600 / 400; // in milliseconds
            _scheduleTimer.Elapsed += delegate { _scheduleEvent.Set(); };
            PerformScheduledWork(null);
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
            { IsBackground = true, Name = "Intrinio" };
            _thread.Start();
        }

        protected override void OnStop()
        {
            DataLayer.LogMessage(LogLevel.Service, "I Service stop.");
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
            try
            {
                using (var context = new Db())
                {
                    var ctx = new Db();
                    var instrument = ctx.Instruments.OrderBy(x => x.LastPriceUpdate).FirstOrDefault();
                    instrument.LastPriceUpdate = DateTime.Now;
                    ctx.SaveChanges();
                    Intrinio.UpdatePricesForTicker(instrument.Ticker);
                }
            }
            catch (Exception e)
            {
                DataLayer.LogException(e);
            }
            _scheduleTimer.Start();
        }
    }
}
