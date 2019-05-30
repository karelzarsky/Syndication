using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using SyndicateLogic;
using SyndicateLogic.Entities;

namespace ShingleConsole
{
    class ShingleConsole
    {
        private static void Main(string[] args)
        {
            using (Process p = Process.GetCurrentProcess())
                p.PriorityClass = ProcessPriorityClass.Idle;
            while (true)
            {
                try
                {
                    using (var ctx = new Db())
                    {
                        var haveWork = false;
                        Article a;
                        if ((a = ctx.Articles.FirstOrDefault(x => x.Processed == ProcessState.Waiting)) != null)
                        {
                            ShingleLogic.ProcessArticle(a.ID);
                            haveWork = true;
                        }
                        if (haveWork) continue;
                        Console.WriteLine("................");
                        System.Threading.Tasks.Task.Delay(60000);
                    }
                }
                catch (Exception e)
                {
                    DataLayer.LogException(e);
                }
            }
        }
    }
}
