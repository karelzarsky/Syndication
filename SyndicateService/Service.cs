using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using SyndicateLogic;
using SyndicateLogic.Entities;
using System.Collections.Generic;
using System.Linq;
using IntrinioConsole;
using System.Globalization;

// instalace:
// c:\windows\microsoft.net\framework\v4.0.30319\installutil.exe c:\GIT\trade\SyndicateService\SyndicateService\bin\Debug\SyndicateService.exe 
// c:\windows\microsoft.net\framework\v4.0.30319\installutil.exe C:\Users\Karel\Source\Repos\Syndication\bin\SyndicateService.exe
//

namespace SyndicateService
{
    public partial class Service : ServiceBase
    {
        public Service()
        {
            using (Process p = Process.GetCurrentProcess())
                p.PriorityClass = ProcessPriorityClass.BelowNormal;
			var culture = new CultureInfo("en-US");
			CultureInfo.DefaultThreadCurrentCulture = culture;
			CultureInfo.DefaultThreadCurrentUICulture = culture;
			InitializeComponent();
            DataLayer.LogMessage(LogLevel.Service, $"Syndication service start.");
        }

        private SynThread thDownload;
        private SynThread thProcessArticles;
        private SynThread thProcessShingles;
        private SynThread thIntrinio;

        private readonly ManualResetEvent shutdownEvent = new ManualResetEvent(false);

        public void Start()
        {
            OnStart(new string[0]);
        }

        protected override void OnStart(string[] args)
        {
            thDownload = new SynThread("Download", 5000, PerformDownload);
            thProcessArticles = new SynThread("ProcessingArticles", 1001, PerformArticleProcessing);
            thProcessShingles = new SynThread("ProcessingShingles", 1111, PerformShingleProcessing);
            //thIntrinio = new SynThread("Intrinio", 24000 * 3600 / 450, PerformIntrinio);
            thIntrinio = new SynThread("Intrinio", 24000 * 3600 / 45, PerformIntrinio);

            RssLogic.UpdateServerConnection();
            using (var ctx = new Db())
            {
                RssLogic.AddNewFeedsFromResource(ctx);
            }
            StartThread(thDownload);
            StartThread(thProcessShingles);
            StartThread(thProcessArticles);
            StartThread(thIntrinio);
        }

        private void StartThread(SynThread th)
        {
            th.thread = new Thread(delegate ()
            {
                // Create the WaitHandle array.
                var handles = new WaitHandle[] { shutdownEvent, th.scheduleEvent };
                // Start the timer.
                th.timer.Start();
                // Wait for one of the events to occur.
                while (!shutdownEvent.WaitOne(0))
                {
                    switch (WaitHandle.WaitAny(handles))
                    {
                        case 0: // Shutdown Event
                            break;
                        case 1: // Schedule Event
                            th.timer.Stop();
                            th.scheduleEvent.Reset();
                            ThreadPool.QueueUserWorkItem(th.performWork.Invoke, null);
                            break;
                        default:
                            shutdownEvent.Set(); // should never occur
                            break;
                    }
                }
            })
            { IsBackground = true, Name = th.name};
            th.thread.Start();
            DataLayer.LogMessage(LogLevel.Service, $"Thread {th.name} started.");
        }

        protected override void OnStop()
        {
            DataLayer.LogMessage(LogLevel.Service, "Service stop.");
            // Signal the thread to shutdown.
            shutdownEvent.Set();
            // Give the thread 10 seconds to terminate.
            if (!thIntrinio.thread.Join(2000))
            {
                thIntrinio.thread.Abort(); // not perferred, but the service is closing anyway
            }
            if (!thProcessShingles.thread.Join(2000))
            {
                thProcessShingles.thread.Abort();
            }
            if (!thIntrinio.thread.Join(2000))
            {
                thIntrinio.thread.Abort();
            }
        }

        private void PerformDownload(object state)
        {
            //DataLayer.LogMessage(LogLevel.Service, "Download Invoked");
            using (var context = new Db())
            {
                Feed f = RssLogic.GetNextFeed(context);
                if (f != null)
                {
                    //DataLayer.LogMessage(LogLevel.Feed, $"N Next feed {f.ID} {f.Url}");
                    RssLogic.ProcessFeed(f, context);
                    //DataLayer.LogMessage(LogLevel.Feed, $"Completed feed {f.ID} {f.Url}");
                    context.SaveChanges();
                }
            }
            thDownload.timer.Start();
        }

        private static List<int> ShinglesToProcess;

        private void PerformShingleProcessing(object state)
        {
            //DataLayer.LogMessage(LogLevel.Service, "Processing Invoked");
            try
            {
                //DataLayer.LogMessage(LogLevel.Info, "New processing run scheduled.");
                using (var ctx = new Db())
                {
                    ctx.Database.CommandTimeout = 120;
                    ShinglesToProcess = ShingleLogic.GetNextShingleList(ctx);
                    if (ShinglesToProcess == null || ShinglesToProcess.Count == 0)
                        System.Threading.Tasks.Task.Delay(20000);
                    else
                    {
                        foreach (var s in ShinglesToProcess)
                        {
                            ShingleLogic.AnalyzeShingle(s);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                DataLayer.LogException(e);
            }
            finally
            {
            thProcessShingles.timer.Start();
            }
        }

        private static List<int> ArticlesToProcess;

        private void PerformArticleProcessing(object state)
        {
            try
            {
                using (var ctx = new Db())
                {
                    ctx.Database.CommandTimeout = 120;
                    ArticlesToProcess = ShingleLogic.GetNextArticles(ctx);
                    if (ArticlesToProcess == null || ArticlesToProcess.Count == 0)
                        System.Threading.Tasks.Task.Delay(20000);
                    else
                    {
                        foreach (var a in ArticlesToProcess)
                        {
                            var sw = Stopwatch.StartNew();
                            ShingleLogic.ProcessArticle(a);
                            RssLogic.ScoreArticle(a);
                            var ea = ctx.Articles.Include("Feed").Single(x => x.ID == a);
                            string ticker = string.IsNullOrEmpty(ea.Ticker) ? "" : "Ticker:" + ea.Ticker + " ";
                            DataLayer.LogMessage(LogLevel.Article, $"A {sw.ElapsedMilliseconds}ms ID:{a} Score:{100*(ea.ScoreMin + ea.ScoreMax)} {ticker}{ea.Title}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                DataLayer.LogException(e);
            }
            finally
            {
                thProcessArticles.timer.Start();
            }
        }

        private void PerformIntrinio(object state)
        {
            //DataLayer.LogMessage(LogLevel.Service, "Intrinio Invoked");
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
            finally
            {
                thIntrinio.timer.Start();
            }
        }
    }
}