using System;
using System.Linq;
using System.Threading;
using SyndicateLogic;
using SyndicateLogic.Entities;

namespace SyndicationConsole
{
    internal class SyndicationConsole
    {
        private static void Main()
        {
            DateTime started = DateTime.Now;
            RssLogic.AddNewFeedsFromResource(new Db());
            RssLogic.UpdateServerConnection();
            using (var context = new Db())
            {
                Feed f = RssLogic.GetNextFeed(context);
                while (context.Feeds.Where(x => x.Active).Min(x => x.LastCheck) < started)
                {
                    f = RssLogic.GetNextFeed(context);
                    if (f != null)
                    {
                        DataLayer.LogMessage(LogLevel.Info, $"N Next feed {f.ID} {f.Url}");
                        RssLogic.ProcessFeed(f, context);
                    }
                    else
                    {
                        Console.Write(".");
                        Thread.Sleep(2000);
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}